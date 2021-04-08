using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

public class ConexionWS : MonoBehaviour {
	private const string DIRECCION_PETICIONES = "wss://risk-servidor.herokuapp.com/crearSala";
	private const int POSICION_TIPO_MENSAJE = 17; // El elemento 17 de el mensaje es siempre un caracter que indica el tipo de mensaje
	private WebSocket ws;
	private Mutex mtx = new Mutex();
	private enum Estado{menuPrincipal, salaEspera, partida};
	private Estado estadoActual;
	public static ConexionWS instance;
	
	private void Awake() {
		instance = this;
	}

	private class Recibido{
		public int Num;
		
	}

	async void Start() {
		#if !UNITY_WEBGL || UNITY_EDITOR
			InvokeRepeating("LeerMensajesWS", 0, 1f);
		#endif

		ws = new WebSocket(DIRECCION_PETICIONES);
		
		ws.OnOpen += () =>
		{
			print("Websocket conectado");
		};

		ws.OnClose += (e) =>
		{
			print("Websocket cerrado: " + e);
		};
		
		ws.OnError += (e) =>
		{
			ControladorPrincipal.instance.PantallaError(e);
		};
		
		ws.OnMessage += (bytes) =>
		{
			string mensaje = System.Text.Encoding.UTF8.GetString(bytes);
			print("Mensaje WebSocket Recibido: " + mensaje);
			GestionarMensaje(mensaje);
		};
		
		await ws.Connect();
		
	}
	
	// Realiza una petición WebSocket con el formulario en formato de string json
	public async Task EnviarWS(string direccion, string form){
		ControladorPrincipal.instance.PantallaCarga(true);
		if (ws == null ||ws.State != WebSocketState.Open){
			ControladorPrincipal.instance.PantallaError("No se ha podido realizar la conexión con el servidor");
			return;
		}
		await ws.SendText(form);
		ControladorPrincipal.instance.PantallaCarga(false);
	}

	public async Task ConexionWebSocket(string direccion){
		ws = new WebSocket(DIRECCION_PETICIONES);
		
		ws.OnOpen += () =>
		{
			print("Websocket conectado");
		};

		ws.OnClose += (e) =>
		{
			print("Websocket cerrado: " + e);
		};
		
		ws.OnError += (e) =>
		{
			//print("Error de websocket: " + e);
			ControladorPrincipal.instance.PantallaError(e);
		};
		
		ws.OnMessage += (bytes) =>
		{
		};
		
		await ws.Connect();
		print("ok");
	}

	// Recibe un mensaje de websocket en formato JSON y lo gestiona dependiendo del estado actual
	private void GestionarMensaje(string mensaje){
		mtx.WaitOne();
		ControladorPrincipal cui = ControladorPrincipal.instance;
		string tipoMensaje = mensaje[POSICION_TIPO_MENSAJE].ToString();
		switch(estadoActual){
			case(Estado.menuPrincipal):
				switch(tipoMensaje){
					case("d"):
						// Datos de sala: Unirse a sala de espera
						// TODO: Guardar los datos de la sala en la informacion de la partida
						estadoActual = Estado.salaEspera;
						cui.AbrirPantalla("SalaEspera");
						break;
					case("e"):
						// Error
						cui.PantallaErrorWS(mensaje);
						break;
					default:
						// Ignorar resto de mensajes
						break;
				}
				break;
			case(Estado.salaEspera):
				switch(tipoMensaje){
					case("d"):
						// Datos de sala: Actualizar datos
						// TODO: Guardar nuevos datos y recargar datos mostrados
						break;
					case("e"):
						// Error
						cui.PantallaErrorWS(mensaje);
						break;
					case("p"):
						// Datos de partida completa: Empezar partida
						// TODO: Guardar nuevos datos
						estadoActual = Estado.partida;
						cui.AbrirPantalla("Partida");
						break;
					default:
						// Ignorar resto de mensajes
						break;
				}
				break;
			case(Estado.partida):
				switch(tipoMensaje){
					case("p"):
						// Todos datos de partida: Actualizar datos de partida
						// TODO: Guardar nuevos datos
						cui.AbrirPantalla("Partida");
						break;
					case("a"):
						// Accion: Mostrar Cambios
						// TODO
						break;
					case("e"):
						// Error
						cui.PantallaErrorWS(mensaje);
						break;
					case("f"):
						// Fin de partida, terminar partida
						// TODO
						estadoActual = Estado.menuPrincipal;
						break;
					default:
						// Ignorar resto de mensajes
						break;
				}
				break;
			default:
				ControladorPrincipal.instance.PantallaError("Estado desconocido");
				break;
		}
		mtx.ReleaseMutex();
	}
	
	private void LeerMensajesWS(){
		print("eh");
		if(ws != null){
			ws.DispatchMessageQueue();
		}
	}
	
	
	
}
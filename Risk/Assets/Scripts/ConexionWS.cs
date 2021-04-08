using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using NativeWebSocket;

public class ConexionWS : MonoBehaviour {
	private const string DIRECCION_PETICIONES = "wss://risk-servidor.herokuapp.com/";
	private const int POSICION_TIPO_MENSAJE = 17; // El elemento 17 de el mensaje es siempre un caracter que indica el tipo de mensaje
	private const float FRECUENCIA_MENSAJES = 1; // Frecuencia de recolección de mensajes de la cola de mensajes de websockets
	private const float MAX_TIME = 10; // Tiempo máximo en segundos que puede tardar una conexión
	private WebSocket ws;
	private Mutex mtx = new Mutex();
	private enum Estado{menuPrincipal, salaEspera, partida};
	private Estado estadoActual;
	[SerializeField]
	private ControladorSalaEspera controladorEspera;
	public static ConexionWS instance;
	
	private void Awake() {
		instance = this;
	}

	private void Start() {
		#if !UNITY_WEBGL || UNITY_EDITOR
			InvokeRepeating("LeerMensajesWS", 0, FRECUENCIA_MENSAJES);
		#endif
	}
	
	// Realiza una petición WebSocket con el formulario en formato de string json
	public async Task EnviarWS(string form) {
		if (ws == null ||ws.State != WebSocketState.Open){
			ControladorPrincipal.instance.PantallaError("No se ha podido realizar la conexión con el servidor");
			return;
		}
		await ws.SendText(form);
	}

	public async Task<bool> ConexionWebSocket(string direccion) {
		
		if (ws != null && ws.State == WebSocketState.Open) {
			// Conexión websocket ya existe, no es necesario crear una nueva
			return true;
		}

		ws = new WebSocket(DIRECCION_PETICIONES + direccion);
		
		// Código a ejecutar cuando se abre la conexión del websocket
		ws.OnOpen += () => {
			print("Websocket conectado");
		};
		// Código a ejecutar cuando se cierra la conexión del websocket
		ws.OnClose += (e) => {
			print("Websocket cerrado: " + e);
		};
		// Código a ejecutar cuando se produce un error de websocket
		ws.OnError += (e) => {
			ControladorPrincipal.instance.PantallaError(e);
		};
		// Código a ejecutar cuando se recibe un mensaje
		ws.OnMessage += (bytes) => {
			string mensaje = System.Text.Encoding.UTF8.GetString(bytes);
			print("Mensaje WebSocket Recibido: " + mensaje);
			GestionarMensaje(mensaje);
		};
		
		ws.Connect();
		// Esperar a que se realice la conexión
		for (float i = 0; i < MAX_TIME; i += 0.5f) {
			await Task.Delay(500);
			if (ws.State == WebSocketState.Open) {
				return true;
			}
		}
		ws.CancelConnection();
		return false;
	}
	
	public async void CerrarConexionWebSocket(){
		if(ws != null && (ws.State != WebSocketState.Closed || ws.State != WebSocketState.Closing)){
			await ws.Close();
		}
		mtx.WaitOne();
		estadoActual = Estado.menuPrincipal;
		mtx.ReleaseMutex();
	}

	// Recibe un mensaje de websocket en formato JSON y lo gestiona dependiendo del estado actual
	private void GestionarMensaje(string mensaje){
		mtx.WaitOne();
		ControladorPrincipal cui = ControladorPrincipal.instance;
		string tipoMensaje = mensaje[POSICION_TIPO_MENSAJE].ToString();
		switch (estadoActual) {
			case (Estado.menuPrincipal):
				switch (tipoMensaje) {
					case ("d"):
						// Datos de sala: Unirse a sala de espera
						estadoActual = Estado.salaEspera;
						controladorEspera.ActualizarDatosSalaEspera(mensaje);
						cui.AbrirPantalla("SalaEspera");
						break;
					case ("e"):
						// Error
						cui.PantallaErrorWS(mensaje);
						break;
					default:
						// Ignorar resto de mensajes
						break;
				}
				break;
			case (Estado.salaEspera):
				switch (tipoMensaje) {
					case ("d"):
						// Datos de sala: Actualizar datos
						controladorEspera.ActualizarDatosSalaEspera(mensaje);
						break;
					case ("e"):
						// Error
						cui.PantallaErrorWS(mensaje);
						break;
					case ("p"):
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
			case (Estado.partida):
				switch (tipoMensaje) {
					case ("p"):
						// Todos datos de partida: Actualizar datos de partida
						// TODO: Guardar nuevos datos
						cui.AbrirPantalla("Partida");
						break;
					case ("a"):
						// Accion: Mostrar Cambios
						// TODO
						break;
					case ("e"):
						// Error
						cui.PantallaErrorWS(mensaje);
						break;
					case ("f"):
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
	
	private void LeerMensajesWS() {
		if (ws != null) {
			ws.DispatchMessageQueue();
		}
	}
	
	
	
}
using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using NativeWebSocket;

public class ControladorConexiones : MonoBehaviour
{
	private const float MAX_TIME = 10; // Tiempo máximo en segundos que puede tardar una conexión
	private const float FRECUENCIA_MENSAJES = 1; // Frecuencia de recolección de mensajes de la cola de mensajes de websockets
	private const string DIRECCION_PETICIONES_HTTP = "https://risk-servidor.herokuapp.com/";
	private const string DIRECCION_PETICIONES_WS = "ws://risk-servidor.herokuapp.com/";
	public static ControladorConexiones instance; // Referencia estática a si mismo para usar como singleton
	private WebSocket ws;
	private Mutex mtxM = new Mutex();
	private int estadoActual = 0;
	private enum Estados{menuPrincipal, salaEspera, partida};
	
	private void Awake() {
		// Es necesario utilizar un singleton porque no se puede invocar a un corrutina desde una clase estática
		instance = this;
		#if !UNITY_WEBGL || UNITY_EDITOR
			InvokeRepeating("LeerMensajesWS", 1, 1);
		#endif
	}
	
	// Devuelve el string recibido tras hacer una petición HTTP con el formulario
	public async Task<string> RequestHTTP(string direccion, WWWForm form){
		ControladorUI.instance.PantallaCarga(true);
		string temp = null;
		// Realizamos la petición HTTP a traves de la corrutina
		StartCoroutine(SendRequest<string>(direccion, form, (returned) => {
			temp = returned;
		}));
		// Esperamos a recibir la respuesta de la corrutina
		float totalTime = 0;
		while(temp == null){
			await Task.Delay(500);
			totalTime += 0.5f;
			if(totalTime >= MAX_TIME){
				// Se ha esperado el tiempo máximo, devolver error
				temp = "ERROR";
				break;
			}
			// Si ha tardado mas de medio segundo activar la pantalla de carga (no importa si lo hace más de 1 vez)
			ControladorUI.instance.PantallaCarga(true);
		}
		ControladorUI.instance.PantallaCarga(false);
		return temp;
	}
	
	// Realiza una petición WebSocket con el formulario en formato de string json
	public async Task EnviarWS(string direccion, string form){
		ControladorUI.instance.PantallaCarga(true);
		if (ws == null){
			await ConexionWebSocket(direccion);
			if(ws == null){
				ControladorUI.instance.PantallaError("No se ha podido realizar la conexión con el servidor");
				return;
			}
		}
		await ws.SendText(form.ToString());
		ControladorUI.instance.PantallaCarga(false);
	}
	
	// Cifra el el string de entrada con el algoritmo SHA256
	public static string Cifrar(string entrada){
		SHA256 hash = SHA256Managed.Create();
		byte[] datos = hash.ComputeHash(Encoding.ASCII.GetBytes(entrada));
		StringBuilder sBuilder = new StringBuilder();
		foreach(byte b in datos){
			sBuilder.Append(b.ToString("x2"));
		}
		return sBuilder.ToString();
	}
	
	// Envia una petición HTTP con el formulario y devuelve el resultado en texto a la función de callback
	public IEnumerator SendRequest<T>(string direccion, WWWForm form, Action<T> callback) {
		UnityWebRequest www = UnityWebRequest.Post(DIRECCION_PETICIONES_HTTP + direccion, form);
		yield return www.SendWebRequest();
		if(www.isNetworkError || www.isHttpError) {
			print("Error de conexión HTTP");
		} else {
			// Convertimos el texto recibido a tipo generico para poder devolverselo al callback
			T t = (T)Convert.ChangeType(www.downloadHandler.text, typeof(T));
			if(callback != null)
			{
				callback(t);
			}

		}
	}
	
	public async Task ConexionWebSocket(string direccion){
		ws = new WebSocket(DIRECCION_PETICIONES_WS + direccion);
		
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
			ControladorUI.instance.PantallaError(e);
		};
		
		ws.OnMessage += (bytes) =>
		{
			var message = System.Text.Encoding.UTF8.GetString(bytes);
			print("Mensaje WebSocket Recibido: " + message);
		};
		
		await ws.Connect();
	}
	
	private void LeerMensajesWS(){
		if(ws != null){
			ws.DispatchMessageQueue();
		}
	}
	
}

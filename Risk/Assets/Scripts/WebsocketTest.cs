using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using TMPro;

public class WebsocketTest : MonoBehaviour
{
	public TextMeshProUGUI textoContador;
	private WebSocket ws;
	
	private class Recibido{
		public int Num;
		
	}

	async void Start() {
		ws = new WebSocket("ws://fathomless-ridge-74437.herokuapp.com/ws");
		
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
			print("Error de websocket: " + e);
		};
		
		ws.OnMessage += (bytes) =>
		{
			var message = System.Text.Encoding.UTF8.GetString(bytes);
			print("Recibido: " + message);
			Recibido rcb = JsonUtility.FromJson<Recibido>(message);
			textoContador.text = rcb.Num.ToString();
		};
		
		await ws.Connect();
	}

	void Update(){
		#if !UNITY_WEBGL || UNITY_EDITOR
			ws.DispatchMessageQueue();
		#endif
	}
	public async void Empezar(){
		if(ws.State == WebSocketState.Open){
			print("Enviando inicio");
			await ws.SendText("{\"Cosa\":\"inicio\"}");
		}
	}
	
	public async void Reiniciar(){
		if(ws.State == WebSocketState.Open){
			print("Enviando reinicio");
			await ws.SendText("{\"Cosa\":\"reset\"}");
		}
	}
}

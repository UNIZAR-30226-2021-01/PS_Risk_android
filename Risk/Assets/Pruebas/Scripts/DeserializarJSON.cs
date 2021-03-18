using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NativeWebSocket;
using TMPro;

public class DeserializarJSON : MonoBehaviour
{
	public TextMeshProUGUI texto;
	private WebSocket ws;
	
	[System.Serializable]
	private class Cadenas
	{
		public string cadena;
	}
	
	[System.Serializable]
	private class CadenaNum
	{
		public string cadena;
		public int num;
	}
	
	[System.Serializable]
	private class Recibido
	{
		public int[] array;
		public List<Cadenas> arrayClase;
		public CadenaNum clase;

	}
	
	async void Start(){
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
			try {
				Recibido rcb = JsonUtility.FromJson<Recibido>(message);
				rcb.array[0].ToString();
				texto.text = "[";
				foreach(int i in rcb.array){
					texto.text += i + ", ";
				}
				texto.text += "]\n\n";
				foreach(Cadenas c in rcb.arrayClase){
					texto.text += c.cadena + "\n";
				}
				texto.text += "\n" + rcb.clase.num + "\n" + rcb.clase.cadena;
			} catch {
				print("Se ha intentado castear a Recibido pero no se ha podido porque era un " + message);
			}
		};
		
		await ws.Connect();
	}

	void Update(){
		#if !UNITY_WEBGL || UNITY_EDITOR
			ws.DispatchMessageQueue();
		#endif
	}

	public async void Pedir(){
		if(ws.State == WebSocketState.Open){
			string toSend = "{\"Cosa\":\"prueba\"}";
			print("Enviando: " + toSend);
			await ws.SendText(toSend);
		}
	}

}

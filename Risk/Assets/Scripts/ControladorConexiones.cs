using System;
using System.Collections;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ControladorConexiones : MonoBehaviour
{
	private const float MAX_TIME = 10; // Tiempo máximo en segundos que puede tardar una conexión
	private const string DIRECCION_PETICIONES = "https://risk-servidor.herokuapp.com/";
	public static ControladorConexiones instance; // Referencia estática a si mismo para usar como singleton
	
	private void Awake() {
		// Es necesario utilizar un singleton porque no se puede invocar a un corrutina desde una clase estática
		instance = this;
	}
	
	// Devuelve el string recibido tras hacer una petición HTTP con el formulario
	public async Task<string> RequestHTTP(string direccion, WWWForm form){
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
		}
		return temp;
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
		UnityWebRequest www = UnityWebRequest.Post(DIRECCION_PETICIONES + direccion, form);
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
	
	
}

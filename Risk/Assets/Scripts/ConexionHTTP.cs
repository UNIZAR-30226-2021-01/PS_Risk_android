using System;
using System.Collections;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

/// <summary>Script que gestiona la conexión por HTTP con backend.</summary>
public class ConexionHTTP : MonoBehaviour
{
	/// <summary>Referencia estática a si mismo para usar como singleton.</summary>
	public static ConexionHTTP instance;

	private const float MAX_TIME = 10; // Tiempo máximo en segundos que puede tardar una conexión
	private const string DIRECCION_PETICIONES = "https://risk-servidor.herokuapp.com/";
	
	private void Awake() {
		// Es necesario utilizar un singleton en vez de crear la clase como estática
		// porque no se puede invocar a un corrutina desde una clase estática
		instance = this;
	}
	
	/// <summary>Realiza una petición HTTP con un formulario.</summary>
	/// <param name="direccion">Dirección del servidor</param>
	/// <param name="form">Formulario a enviar</param>
	/// <returns>Respuesta JSON del servidor</returns>
	public async Task<string> RequestHTTP(string direccion, WWWForm form){
		ControladorPrincipal.instance.PantallaCarga(true);
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
			ControladorPrincipal.instance.PantallaCarga(true);
		}
		ControladorPrincipal.instance.PantallaCarga(false);
		return temp;
	}

	/// <summary>Realiza una petición HTTP con un formulario, sin mostrar la pantalla de carga</summary>
	/// <param name="direccion">Dirección del servidor</param>
	/// <param name="form">Formulario a enviar</param>
	/// <returns>Respuesta JSON del servidor</returns>
	// Se repite codigo, hay que juntarlo con la funcion de arriba
	public async Task<string> RequestHTTPFantasma(string direccion, WWWForm form) {
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
		}
		return temp;
	}
	
	/// <summary>Cifra el el string de entrada con el algoritmo SHA256.</summary>
	/// <param name="entrada">Texto a cifrar</param>
	public static string Cifrar(string entrada){
		SHA256 hash = SHA256Managed.Create();
		byte[] datos = hash.ComputeHash(Encoding.ASCII.GetBytes(entrada));
		StringBuilder sBuilder = new StringBuilder();
		foreach(byte b in datos){
			sBuilder.Append(b.ToString("x2"));
		}
		return sBuilder.ToString();
	}
	
	/// <summary>Envia una petición HTTP con el formulario y devuelve el resultado en texto a la función de callback.</summary>
	/// <param name="direccion">Dirección del servidor</param>
	/// <param name="form">Formulario a enviar</param>
	/// <param name="callback">Callback</param>
	/// <returns>IEnumerator</returns>
	public IEnumerator SendRequest<T>(string direccion, WWWForm form, Action<T> callback) {
		UnityWebRequest www = UnityWebRequest.Post(DIRECCION_PETICIONES + direccion, form);
		yield return www.SendWebRequest();
		if(!(www.isNetworkError || www.isHttpError)) {
			// Convertimos el texto recibido a tipo generico para poder devolverselo al callback
			T t = (T)Convert.ChangeType(www.downloadHandler.text, typeof(T));
			if(callback != null)
			{
				callback(t);
			}

		}
	}
	
}

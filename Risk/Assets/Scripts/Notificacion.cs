using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class Notificacion : MonoBehaviour {
	//public Image icono; //TODO: Icono identificativo del tipo de notificación
	public TextMeshProUGUI nombre; //Texto de información de la notificación

	private ClasesJSON.Notificacion datos; //Datos de la notificación, como idEnvio, extraInfo y tipo

	private const string TIPO_AMISTAD = "amistad";
	private const string TIPO_TURNO = "turno";
	private const string TIPO_INVICATION = "invitacion";
	
	//Llamado por el botón de rechazar
	public void Rechazar() {
		switch(datos.tipo) {
			case TIPO_AMISTAD:
				Gestionar_Amistad("Rechazar");
				break;

			case TIPO_TURNO:
				//TODO
				break;

			case TIPO_INVICATION:
				//TODO
				break;

			default:
				Debug.LogError("[Notificacion] No se puede rechazar, tipo de notificacion '" + datos.tipo + "' desconocido");
				break;
		}
	}

	//Llamado por el botón de aceptar
	public void Aceptar() {
		switch(datos.tipo) {
			case TIPO_AMISTAD:
				Gestionar_Amistad("Aceptar");
				break;

			case TIPO_TURNO:
				//TODO
				break;

			case TIPO_INVICATION:
				//TODO
				break;

			default:
				Debug.LogError("[Notificacion] No se puede rechazar, tipo de notificacion '" + datos.tipo + "' desconocido");
				break;
		}
	}

	//Actualiza los datos de la clase
	public void Actualizar(ClasesJSON.Notificacion n) {
		datos = n;

		//Actualizar texto
		Actualizar_Texto();
	}

	//Actualiza el texto del gameobject
	private void Actualizar_Texto() {
		switch(datos.tipo) {
			case TIPO_AMISTAD:
				nombre.text = "¡<b>" + datos.infoExtra + "</b> quiere ser tu amigo!";
				break;

			case TIPO_TURNO:
				nombre.text = "Notificación Turno";
				break;

			case TIPO_INVICATION:
				nombre.text = "Notificación Invitación";
				break;

			default:
				nombre.text = "Notificación Desconocida";
				break;
		}
	}

	//Acepta o rechaza una petición de amistad
	private async void Gestionar_Amistad(string modo) {
		Debug.Log("[Notificacion] Gestionando petición de amistad (" + modo + ")");

		//Crear formulario
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorUI.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorUI.instance.usuarioRegistrado.clave);
		form.AddField("idAmigo", datos.idEnvio);
		form.AddField("decision", modo);

		//Obtener respuesta del servidor
		string respuesta = await ControladorConexiones.instance.RequestHTTP("gestionAmistad", form);

		//Procesar respuesta
		try {
			//Error, ir a la pantalla de error
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(respuesta);

			if(error.code != 0) //Mostrar pantalla de error solo si la respuesta no es error 0
				ControladorUI.instance.PantallaError(error.err);
			else { //Si no, se puede borrar la notificacion
				Destroy(gameObject);
				ControladorNotificaciones.notificaciones.Remove(datos);
			}
		} catch {
			//Respuesta desconocida, ¿El servidor esta mandando una respuesta?
			Debug.LogError("[Controlador Notificaciones] Respuesta del servidor desconocida\nRespuesta: " + respuesta);
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

/*
	Script para el prefab de notificacion
*/
public class Notificacion : MonoBehaviour {
	//public Image icono; //TODO: Icono identificativo del tipo de notificación
	public TextMeshProUGUI nombre; //Texto de información de la notificación

	private ClasesJSON.Notificacion datos; //Datos de la notificación, como idEnvio, extraInfo y tipo

	private const string TIPO_AMISTAD = "Peticion de amistad";
	private const string TIPO_TURNO = "turno";
	private const string TIPO_INVICATION = "Invitacion";
	
	//Llamado por el botón de rechazar
	public void Rechazar() {
		switch(datos.tipo) {
			case TIPO_AMISTAD:
				GestionarAmistad("Rechazar");
				break;

			case TIPO_TURNO:
				//TODO
				break;

			case TIPO_INVICATION:
				Destroy(gameObject);
				break;

			default:
				Debug.LogError("[Notificacion] No se puede rechazar, tipo de notificacion '" + datos.tipo + "' desconocido");
				break;
		}
		ControladorNotificaciones.instance.ActualizarNotificaciones();
	}

	//Llamado por el botón de aceptar
	public void Aceptar() {
		switch(datos.tipo) {
			case TIPO_AMISTAD:
				GestionarAmistad("Aceptar");
				break;

			case TIPO_TURNO:
				//TODO
				break;

			case TIPO_INVICATION:
				//TODO
				AceptarInvitacion();
				break;

			default:
				Debug.LogError("[Notificacion] No se puede rechazar, tipo de notificacion '" + datos.tipo + "' desconocido");
				break;
		}
		ControladorNotificaciones.instance.ActualizarNotificaciones();
	}

	//Actualiza los datos de la clase
	public void Actualizar(ClasesJSON.Notificacion n) {
		datos = n;

		//Actualizar texto
		ActualizarTexto();
	}

	//Actualiza el texto del gameobject
	private void ActualizarTexto() {
		switch(datos.tipo) {
			case TIPO_AMISTAD:
				nombre.text = "¡<b><color=#FF0000>" + datos.infoExtra + "</color></b> quiere ser tu amigo!";
				break;

			case TIPO_TURNO:
				nombre.text = "Notificación Turno";
				break;

			case TIPO_INVICATION:
				nombre.text = "<b><color=#FF0000>" + datos.infoExtra + "</color></b> te ha invitado a una sala";
				break;

			default:
				nombre.text = "Notificación Desconocida";
				break;
		}
	}

	//Acepta o rechaza una petición de amistad
	private async void GestionarAmistad(string modo) {
		Debug.Log("[Notificacion] Gestionando petición de amistad (" + modo + ")");

		//Crear formulario
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorPrincipal.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorPrincipal.instance.usuarioRegistrado.clave);
		form.AddField("idAmigo", datos.idEnvio);
		form.AddField("decision", modo);

		//Obtener respuesta del servidor
		string respuesta = await ConexionHTTP.instance.RequestHTTP("gestionAmistad", form);

		//Procesar respuesta
		try {
			//Error, ir a la pantalla de error
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(respuesta);

			if(error.code != 0) //Mostrar pantalla de error solo si la respuesta no es error 0
				ControladorPrincipal.instance.PantallaError(error.err);
			else { //Si no, se puede borrar la notificacion
				Destroy(gameObject);
				ControladorNotificaciones.notificaciones.Remove(datos);
			}
		} catch {
			//Respuesta desconocida, ¿El servidor esta mandando una respuesta?
			//Debug.LogError("[Controlador Notificaciones] Respuesta del servidor desconocida\nRespuesta: " + respuesta);
		}
		
	}
	private async void AceptarInvitacion(){
		Usuario usuario = ControladorPrincipal.instance.usuarioRegistrado;
		ClasesJSON.AceptarSala datosSala = new ClasesJSON.AceptarSala(usuario.id, usuario.clave, datos.idEnvio);
		string datosEnviar = JsonConvert.SerializeObject(datosSala);
		if(!(await ConexionWS.instance.ConexionWebSocket("aceptarSala"))){
			ControladorPrincipal.instance.PantallaError("No se ha podido realizar la conexión con el servidor");
		} else {
			await ConexionWS.instance.EnviarWS(datosEnviar);
			Destroy(gameObject);
		}
	}

} //VSCode, para, que no hay ningun error aquí ;_;
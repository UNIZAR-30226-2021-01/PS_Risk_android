using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

/// <summary>
/// Script usado en el prefab de notificaciones.
/// Guarda la información de una notificación y la lógica para aceptarla o rechazarla.
/// </summary>
public class Notificacion : MonoBehaviour {
	/// <summary>
	///	Componente Texto del prefab que indica la información de la notificación al usuario.
	/// Iniciar en el prefab desde el editor de Unity
	/// </summary>
	public TextMeshProUGUI nombre; //Texto de información de la notificación

	/// <summary>
	///	Instancia del Controlador de Notificaciones del juego
	/// Iniciar en el prefab desde el editor de Unity
	/// </summary>
	public ControladorNotificaciones controladorNotificaciones;

	private ClasesJSON.Notificacion datos; //Datos de la notificación, como idEnvio, extraInfo y tipo

	private const string TIPO_AMISTAD = "Peticion de amistad";
	private const string TIPO_TURNO = "Notificacion de turno";
	private const string TIPO_INVICATION = "Invitacion";
	
	/// <summary>
	///	Envia a backend el rechazo de la notificación.
	/// La notificación es borrada del juego.
	/// Esta función es llamada por el botón rojo del prefab.
	/// </summary>
	/// Ver <see cref="Notificacion.Aceptar()"/> para aceptar una notificación
	public void Rechazar() {
		switch(datos.tipo) {
			case TIPO_AMISTAD:
				GestionarAmistad("Rechazar");
				break;

			case TIPO_TURNO:
				Destroy(gameObject);
				break;

			case TIPO_INVICATION:
				RechazarPartida();
				break;

			default:
				break;
		}
		controladorNotificaciones.ActualizarNotificaciones();
	}

	/// <summary>
	///	Envia a backend la aceptación de la notificación.
	/// La notificación es borrada del juego.
	/// Esta función es llamada por el botón verde del prefab.
	/// </summary>
	/// Ver <see cref="Notificacion.Rechazar()"/> para rechazar una notificación
	public void Aceptar() {
		switch(datos.tipo) {
			case TIPO_AMISTAD:
				GestionarAmistad("Aceptar");
				break;

			case TIPO_TURNO:
				AceptarTurno();
				break;

			case TIPO_INVICATION:
				AceptarInvitacion();
				break;

			default:
				break;
		}
		controladorNotificaciones.ActualizarNotificaciones();
	}

	//Actualiza los datos de la clase
	/// <summary>
	///	Actualizar los datos de la clase con nuevos datos
	/// También actualiza el texto que el usuario ve en el prefab
	/// </summary>
	/// <param name="n">Nuevos datos de la notificación</param>
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
				nombre.text = "Es tu turno en la partida <b><color=#FF0000>" + datos.infoExtra + "</color></b>";
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
				controladorNotificaciones.notificaciones.Remove(datos);
			}
		} catch {}
		
	}

	private async void AceptarInvitacion(){
		Usuario usuario = ControladorPrincipal.instance.usuarioRegistrado;
		ClasesJSON.AceptarSala datosSala = new ClasesJSON.AceptarSala(usuario.id, usuario.clave, datos.idEnvio);
		string datosEnviar = JsonConvert.SerializeObject(datosSala);
		if(!(await ConexionWS.instance.ConexionWebSocket("aceptarSala"))){
			ControladorPrincipal.instance.PantallaError("No se ha podido realizar la conexión con el servidor");
		} else {
			await ConexionWS.instance.EnviarWS(datosEnviar);
			try{
				Destroy(gameObject);
			} catch {}
		}
	}
	
	private async void AceptarTurno() {
		Usuario usuario = ControladorPrincipal.instance.usuarioRegistrado;
		ClasesJSON.AceptarSala datosSala = new ClasesJSON.AceptarSala(usuario.id, usuario.clave, datos.idEnvio);
		string datosEnviar = JsonConvert.SerializeObject(datosSala);
		if(!(await ConexionWS.instance.ConexionWebSocket("entrarPartida"))){
			ControladorPrincipal.instance.PantallaError("No se ha podido realizar la conexión con el servidor");
		} else {
			await ConexionWS.instance.EnviarWS(datosEnviar);
			try{
				Destroy(gameObject);
			} catch {}
		}
	}
	
	private async void RechazarPartida(){
		Usuario usuario = ControladorPrincipal.instance.usuarioRegistrado;
		ClasesJSON.AceptarSala datosSala = new ClasesJSON.AceptarSala(usuario.id, usuario.clave, datos.idEnvio);
		// Crear formulario a enviar petición al servidor
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", usuario.id);
		form.AddField("clave", usuario.clave);
		form.AddField("idSala", datos.idEnvio);
		string result = await ConexionHTTP.instance.RequestHTTP("rechazarPartida", form);
		try {
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(result);
			if (error.code == 0){
				ControladorPrincipal.instance.PantallaError(error.err);
				Destroy(gameObject);
			}
			ControladorPrincipal.instance.PantallaError(error.err);
		} catch {}
	}

}
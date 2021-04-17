using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

/// <summary>
/// Script usado para controlar el prefab de la lista de amigos
/// </summary>
public class Amigo : MonoBehaviour {

	/// <summary>Imagen con el icono del usuario</summary>
	public Image icono;

	/// <summary>Compontente de Texto que muestra el nombre del usuario</summary>
	public TextMeshProUGUI nombre;

	/// <summary>ID del usuario</summary>
	public int id;

	/// <summary>Instancia del Controlador de Amigos usado en la escena del juego</summary>
	public ControladorAmigos controladorAmigos;
	
	/// <summary>Envia petición al servidor con el id de este amigo para ser borrado de la lista de amigos del usuario</summary>
	public async void BorrarAmigo(){
		ControladorPrincipal.instance.PantallaCarga(true);
		// Crear formulario a enviar
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorPrincipal.instance.usuarioRegistrado.id);
		form.AddField("idAmigo", id);
		form.AddField("decision", "Borrar");
		form.AddField("clave", ControladorPrincipal.instance.usuarioRegistrado.clave);
		// Enviar petición al servidor
		string recibido = await ConexionHTTP.instance.RequestHTTP("gestionAmistad", form);
		try {
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido);
			if(error.code != 0) {
				// Error
				ControladorPrincipal.instance.PantallaError(error.err);
			} else {
				// Borrado del servidor efectuado correctamente borrar este usuario de la lista
				Destroy(gameObject);
			}
		} catch {
			ControladorPrincipal.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
		}
		controladorAmigos.RecargarAmigos();
		ControladorPrincipal.instance.PantallaCarga(false);
	}
	
	/// <summary>Invitar al amigo a una sala</summary>
	public async void InvitarAmigo(){
		ControladorPrincipal.instance.PantallaCarga(true);
		ClasesJSON.InvitacionSala invitacion = new ClasesJSON.InvitacionSala(id);
		string datos = JsonConvert.SerializeObject(invitacion);
		await ConexionWS.instance.EnviarWS(datos);
		ControladorPrincipal.instance.PantallaCarga(false);	
		ControladorPrincipal.instance.PantallaInfo("Invitación enviada exitosamente");
	}

}

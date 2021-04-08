using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

public class Amigo : MonoBehaviour {
	public Image icono;
	public TextMeshProUGUI nombre;
	public int id;
	public ControladorAmigos controladorAmigos;
	
	// Envia petición al servidor con el id de este amigo para ser borrado de la lista de amigos del usuario
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
	
	public async void InvitarAmigo(){
		ControladorPrincipal.instance.PantallaCarga(true);
		ClasesJSON.InvitacionSala invitacion = new ClasesJSON.InvitacionSala(id);
		string datos = JsonConvert.SerializeObject(invitacion);
		print(datos);
		await ConexionWS.instance.EnviarWS(datos);
		ControladorPrincipal.instance.PantallaCarga(false);
	}

}

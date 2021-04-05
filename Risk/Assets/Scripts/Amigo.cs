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
	
	// Envia petición al servidor con el id de este amigo para ser borrado de la lista de amigos del usuario
	public async void BorrarAmigo(){
		// Crear formulario a enviar
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorUI.instance.usuarioRegistrado.id);
		form.AddField("idAmigo", id);
		form.AddField("decision", "Borrar");
		form.AddField("clave", ControladorUI.instance.usuarioRegistrado.clave);
		// Enviar petición al servidor
		string recibido = await ControladorConexiones.instance.RequestHTTP("gestionAmistad", form);
		try {
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido);
			if(error.code != 0) {
				// Error
				ControladorUI.instance.PantallaError(error.err);
			} else {
				// Borrado del servidor efectuado correctamente borrar este usuario de la lista
				Destroy(gameObject);
			}
		} catch {
			ControladorUI.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
		}
		ControladorAmigos.instance.RecargarAmigos();
	}

}

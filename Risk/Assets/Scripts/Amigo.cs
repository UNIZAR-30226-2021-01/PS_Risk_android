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
	
	public async void BorrarAmigo(){
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorUI.instance.usuarioRegistrado.id);
		form.AddField("idAmigo", id);
		form.AddField("decision", "Borrar");
		form.AddField("clave", ControladorUI.instance.usuarioRegistrado.clave);
		string recibido = await ControladorConexiones.instance.RequestHTTP("gestionAmistad", form);
		try {
			// Error
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido);
			if(error.code != 0) {
				ControladorUI.instance.PantallaError(error.err);
			} else {
				Destroy(gameObject);
			}
		} catch {}
	}

}

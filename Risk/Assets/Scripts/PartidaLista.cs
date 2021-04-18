using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class PartidaLista : MonoBehaviour {
	public int id;
	[SerializeField]
	private TextMeshProUGUI textoNotificacion;

	public void ActualizarTexto(string texto){
		textoNotificacion.text = texto;
	}

	public async void Unirse(){
		Usuario usuario = ControladorPrincipal.instance.usuarioRegistrado;
		ClasesJSON.AceptarSala datosSala = new ClasesJSON.AceptarSala(usuario.id, usuario.clave, id);
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

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ControladorAmigos : MonoBehaviour {
	public GameObject amigoPrefab, noAmigoPrefab;
	public Transform padreAmigos;
	private List<ClasesJSON.Amigo> listaAmigos;
	private string amigoAgregar;
	
	private void OnEnable() {
		//RecargarAmigos();
	}
	
	public void ActualizarAmigoAgregar(string nombre){
		amigoAgregar = nombre;
	}

	public async void RecargarAmigos() {
		// Borrar todos los amigos de la lista y poner no amigo
		for(int i = 0; i < padreAmigos.childCount; i++) {
			Destroy(padreAmigos.GetChild(i).gameObject);
		}
		GameObject noAmigo = Instantiate(noAmigoPrefab, padreAmigos);
		// Crear formulario a enviar
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorUI.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorUI.instance.usuarioRegistrado.clave);
		// Enviar petición al servidor
		string recibido = await ControladorConexiones.instance.RequestHTTP("amigos", form);
		// En algunas circunstancias el servidor envia null en vez de un array, en ese caso no hay amigos
		if (!recibido.Contains("[")) {
			return;
		}
		try {
			listaAmigos = JsonConvert.DeserializeObject<ClasesJSON.ListaAmigos>(recibido, ClasesJSON.settings).amigos;
			if (listaAmigos.Count == 0) {
				return;
			}
			Destroy(noAmigo);
			foreach (var amigo in listaAmigos) {
				Amigo nuevoAmigo = Instantiate(amigoPrefab, padreAmigos).GetComponent<Amigo>();
				nuevoAmigo.id = amigo.id;
				nuevoAmigo.icono.sprite = ControladorUI.instance.iconos[amigo.icono];
				nuevoAmigo.nombre.text = amigo.nombre;
			}
		} catch {
			try {
				print(recibido);
				// Error, mostrar mensaje de error
				ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido, ClasesJSON.settings);
				print(error.code + ", " + error.err);
				ControladorUI.instance.PantallaError(error.err);
			} catch {
				// No hay error
				ControladorUI.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
			}
		}
	}
	
	public async void EnviarSolicitudAmistad(){
		// Crear formulario a enviar petición al servidor
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorUI.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorUI.instance.usuarioRegistrado.clave);
		form.AddField("nombreAmigo", amigoAgregar);
		string recibido = await ControladorConexiones.instance.RequestHTTP("enviarSolicitudAmistad", form);
		try {
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido, ClasesJSON.settings);
			if(error.code != 0) {
				// No hay error	
				ControladorUI.instance.PantallaError(error.err);
			}
		} catch {
			ControladorUI.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
		}
		
	}
}

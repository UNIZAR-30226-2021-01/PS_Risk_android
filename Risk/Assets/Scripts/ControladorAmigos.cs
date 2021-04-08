using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ControladorAmigos : MonoBehaviour {
	[SerializeField]
	private GameObject amigoPrefab, noAmigoPrefab;
	[SerializeField]
	private Transform padreAmigos;
	private List<ClasesJSON.Amigo> listaAmigos;
	private string amigoAgregar;

	public void ActualizarAmigoAgregar(string nombre){
		amigoAgregar = nombre;
	}

	public async void RecargarAmigos() {
		// Borrar todos los amigos de la lista y poner no amigo
		for(int i = 0; i < padreAmigos.childCount; i++) {
			Destroy(padreAmigos.GetChild(i).gameObject);
		}
		// Crear formulario a enviar
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorPrincipal.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorPrincipal.instance.usuarioRegistrado.clave);
		// Enviar petición al servidor
		string recibido = await ConexionHTTP.instance.RequestHTTP("amigos", form);
		// En algunas circunstancias el servidor no envía un array, en ese caso no hay amigos
		if (!recibido.Contains("[")) {
			Instantiate(noAmigoPrefab, padreAmigos);
			return;
		}
		try {
			listaAmigos = JsonConvert.DeserializeObject<ClasesJSON.ListaAmigos>(recibido, ClasesJSON.settings).amigos;
			if (listaAmigos.Count == 0) {
				Instantiate(noAmigoPrefab, padreAmigos);
				return;
			}
			foreach (var amigo in listaAmigos) {
				Amigo nuevoAmigo = Instantiate(amigoPrefab, padreAmigos).GetComponent<Amigo>();
				nuevoAmigo.controladorAmigos = this;
				nuevoAmigo.id = amigo.id;
				nuevoAmigo.icono.sprite = ControladorPrincipal.instance.iconos[amigo.icono];
				nuevoAmigo.nombre.text = amigo.nombre;
			}
		} catch {
			try {
				print(recibido);
				// Error, mostrar mensaje de error
				ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido, ClasesJSON.settings);
				print(error.code + ", " + error.err);
				ControladorPrincipal.instance.PantallaError(error.err);
			} catch {
				// No hay error
				ControladorPrincipal.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
			}
		}
	}
	
	public async void EnviarSolicitudAmistad(){
		// Crear formulario a enviar petición al servidor
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorPrincipal.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorPrincipal.instance.usuarioRegistrado.clave);
		form.AddField("nombreAmigo", amigoAgregar);
		string recibido = await ConexionHTTP.instance.RequestHTTP("enviarSolicitudAmistad", form);
		try {
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido, ClasesJSON.settings);
			if(error.code != 0) {
				// No hay error	
				ControladorPrincipal.instance.PantallaError(error.err);
			}
		} catch {
			ControladorPrincipal.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
		}
		
	}
}

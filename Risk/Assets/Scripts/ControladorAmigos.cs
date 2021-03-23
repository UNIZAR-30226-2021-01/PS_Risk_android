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
		RecargarAmigos();
	}
	
	public void ActualizarAmigoAgregar(string nombre){
		amigoAgregar = nombre;
	}

	public async void RecargarAmigos(){
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorUI.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorUI.instance.usuarioRegistrado.clave);
		for(int i = 0; i < padreAmigos.childCount; i++) {
			Destroy(padreAmigos.GetChild(i).gameObject);
		}
		string recibido = await ControladorConexiones.instance.RequestHTTP("amigos", form);
		try {
			// Error
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido);
			ControladorUI.instance.PantallaError(error.err);
		} catch {
			// No hay error
			listaAmigos = JsonConvert.DeserializeObject<List<ClasesJSON.Amigo>>(recibido);
			foreach (var amigo in listaAmigos) {
				Amigo nuevoAmigo = Instantiate(amigoPrefab, padreAmigos).GetComponent<Amigo>();
				nuevoAmigo.id = amigo.id;
				nuevoAmigo.icono.sprite = ControladorUI.instance.iconos[amigo.icono-1];
				nuevoAmigo.nombre.text = amigo.nombre;
			}
		}
		if(listaAmigos.Count == 0){
			Instantiate(noAmigoPrefab, padreAmigos);
		}
	}
	
	public async void EnviarSolicitudAmistad(){
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorUI.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorUI.instance.usuarioRegistrado.clave);
		form.AddField("nombreAmigo", amigoAgregar);
		string recibido = await ControladorConexiones.instance.RequestHTTP("enviarSolicitudAmistad", form);
		try {
			// Error
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido);
			if(error.code != 0) {
				ControladorUI.instance.PantallaError(error.err);
			}
		} catch {}
		
	}
}

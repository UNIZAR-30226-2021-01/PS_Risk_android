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
		if(ControladorUI.instance == null || ControladorUI.instance.usuarioRegistrado == null){
			// Por razones de orden de ejecución inicial de Unity, es posible que esto se llame antes
			// del Awake de ControladorUI por lo que la variable instance puede no estar asignada,
			// en ese caso terminar la ejecución de esta función (cuando esto ocurre no interesa el valor real)
			return;
		}
		// Crear formulario a enviar
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorUI.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorUI.instance.usuarioRegistrado.clave);
		// Borrar todos los amigos de la lista
		for(int i = 0; i < padreAmigos.childCount; i++) {
			Destroy(padreAmigos.GetChild(i).gameObject);
		}
		// Enviar petición al servidor
		string recibido = await ControladorConexiones.instance.RequestHTTP("amigos", form);
		try {
			// Error, mostrar mensaje de error
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
		// Lista de amigos vacía, mostrar mensaje que indica que no tiene amigos
		if(listaAmigos.Count == 0){
			Instantiate(noAmigoPrefab, padreAmigos);
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
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido);
			if(error.code != 0) {
				// No hay error	
				ControladorUI.instance.PantallaError(error.err);
			}
		} catch {
			ControladorUI.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
		}
		
	}
}

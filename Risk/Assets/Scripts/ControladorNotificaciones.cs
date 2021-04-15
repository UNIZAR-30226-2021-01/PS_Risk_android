using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Newtonsoft.Json;

/// <summary>
/// Script usado para controlar el menu de notificaciones
/// </summary>
public class ControladorNotificaciones : MonoBehaviour
{
	/// <summary>Lista/Cache de las notificaciones</summary>
	public List<ClasesJSON.Notificacion> notificaciones; //Cache de notificaciones

	[SerializeField]
	private GameObject prefabNotificacion; //Prefab de notificación
	[SerializeField]
	private GameObject prefabNada; //Prefab usado cuando no hay notificaciones ha mostrar
	[SerializeField]
	private Transform listaPadre; //Transform del objeto padre que tendra como hijos los elementos de la lista

	private const string ERROR_ACTUALIZARNOTIFICACIONES = "No se puede actualizar la lista de notificaciones";
	
	//Queremos actualizar las notificaciones en pantalla cada vez que se accede a la pantalla
	private void OnEnable() {
		ActualizarNotificaciones();
	}

	/// <summary>Actualiza las notificaciones a mostrar al usuario.</summary>
	public async void ActualizarNotificaciones() {

		//Borrar los gameobjects de las notificaciones anteriores
		for(int i = 0; i < listaPadre.childCount; i++) {
			Destroy(listaPadre.GetChild(i).gameObject);
		}

		//Retrivir las notificaciones del servidor
		await ObtenerNotificaciones();
		
		if(notificaciones != null) {
			foreach(ClasesJSON.Notificacion n in notificaciones) {
				Notificacion notificacion = Instantiate(prefabNotificacion, listaPadre).GetComponent<Notificacion>(); //Notificación anclada en el gameobject
				notificacion.Actualizar(n);
				notificacion.controladorNotificaciones = this;
			}

			//Si no hay notificaciones, mostrar prefab 'prefabNada'
			if(notificaciones.Count == 0) {
				Instantiate(prefabNada, listaPadre);
			}
		}
		else { //Indicar que no hay notificaciones que mostrar
			Instantiate(prefabNada, listaPadre);
		}
	}

	//Obtiene las notificaciones del servidor y las guarda en la variable 'notificaciones'
	//Las notificaciones son guardadas en un campo estatico para uso posterior desde donde sea
	private async Task ObtenerNotificaciones() {

		//Si el controlador ui no esta iniciado, borrar la lista de notificaciones
		if(ControladorPrincipal.instance == null) {
			notificaciones = null;
			return;
		}

		//Crear formulario
		WWWForm form = new WWWForm();
		//Comprobar que hay un usuario y su clave
		if(ControladorPrincipal.instance == null || ControladorPrincipal.instance.usuarioRegistrado == null) {
			notificaciones = null;
			return;
		}
		form.AddField("idUsuario", ControladorPrincipal.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorPrincipal.instance.usuarioRegistrado.clave);

		//Obtener respuesta del servidor
		string respuesta = await ConexionHTTP.instance.RequestHTTP("notificaciones", form);

		try { 
			try {
				//Petición realiza con exito, guardar notificaciones en la variable 'notificaciones'
				ClasesJSON.ListaNotificaciones p = JsonConvert.DeserializeObject<ClasesJSON.ListaNotificaciones>(respuesta, ClasesJSON.settings);
				notificaciones = p.notificaciones;
			} catch {}
		} catch {
			//Error, ir a la pantalla de error
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(respuesta);
			ControladorPrincipal.instance.PantallaError(error.err);
			notificaciones = null;
		}
	}
}

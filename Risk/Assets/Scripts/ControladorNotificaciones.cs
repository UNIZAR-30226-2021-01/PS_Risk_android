using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

using Newtonsoft.Json;

public class ControladorNotificaciones : MonoBehaviour
{
	public GameObject prefabNotificacion; //Prefab de notificación
	public GameObject prefabNada; //Prefab usado cuando no hay notificaciones ha mostrar
	public Transform listaPadre; //Transform del objeto padre que tendra como hijos los elementos de la lista

	public static List<ClasesJSON.Notificacion> notificaciones; //Cache de notificaciones

	private const string ERROR_ACTUALIZARNOTIFICACIONES = "No se puede actualizar la lista de notificaciones";
	public static ControladorNotificaciones instance;
	
	private void Awake() {
		instance = this;
	}

	//Queremos actualizar las notificaciones en pantalla cada vez que se accede a la pantalla
	private void OnEnable() {
		ActualizarNotificaciones();
	}

	//Actualiza las notificaciones a mostrar al usuario
	public async void ActualizarNotificaciones() {
		Debug.Log("[Controlador Notificaciones] Actualizando Lista de Notificaciones");

		//Borrar los gameobjects de las notificaciones anteriores
		for(int i = 0; i < listaPadre.childCount; i++) {
			Destroy(listaPadre.GetChild(i).gameObject);
		}

		//Errores de campos
		if(prefabNotificacion == null) {
			Debug.LogError(ERROR_ACTUALIZARNOTIFICACIONES + " ya que el prefab de notificación (prefabNotificacion) es nulo!");
		} else if(prefabNada == null) {
			Debug.LogError(ERROR_ACTUALIZARNOTIFICACIONES + " ya que el prefab de ninguna notificación (prefabNada) es nulo!");
		} else if(listaPadre == null) {
			Debug.LogError(ERROR_ACTUALIZARNOTIFICACIONES + " ya que el transform padre donde poner los prefabs (listaPadre) es nulo!");
		}

		//Retrivir las notificaciones del servidor
		await ObtenerNotificaciones();
		
		if(notificaciones != null) {
			foreach(ClasesJSON.Notificacion n in notificaciones) {
				Notificacion go_n = Instantiate(prefabNotificacion, listaPadre).GetComponent<Notificacion>(); //Notificación anclada en el gameobject
				go_n.Actualizar(n);
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
	public static async Task ObtenerNotificaciones() {
		Debug.Log("[Controlador Notificaciones] Obteniendo Notificaciones del Servidor");

		//Si el controlador ui no esta iniciado, borrar la lista de notificaciones
		if(ControladorUI.instance == null) {
			notificaciones = null;
			return;
		}

		//Crear formulario
		WWWForm form = new WWWForm();
		//Comprobar que hay un usuario y su clave
		if(ControladorUI.instance == null || ControladorUI.instance.usuarioRegistrado == null) {
			Debug.LogError("No se pueden obtener notificaciones: Usuario y clave desconocidos");
			notificaciones = null;
			return;
		}
		form.AddField("idUsuario", ControladorUI.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorUI.instance.usuarioRegistrado.clave);

		//Obtener respuesta del servidor
		string respuesta = await ControladorConexiones.instance.RequestHTTP("notificaciones", form);

		try { 
			try {
				//Petición realiza con exito, guardar notificaciones en la variable 'notificaciones'
				ClasesJSON.ListaNotificaciones p = JsonConvert.DeserializeObject<ClasesJSON.ListaNotificaciones>(respuesta, ClasesJSON.settings);
				notificaciones = p.notificaciones;
			} catch {
				//Respuesta desconocida, ¿El servidor esta mandando una respuesta?
				Debug.LogError("[Controlador Notificaciones] Respuesta del servidor desconocida\nRespuesta: " + respuesta);
			}
		} catch {
			//Error, ir a la pantalla de error
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(respuesta);
			ControladorUI.instance.PantallaError(error.err);
			notificaciones = null;
		}
	}
}

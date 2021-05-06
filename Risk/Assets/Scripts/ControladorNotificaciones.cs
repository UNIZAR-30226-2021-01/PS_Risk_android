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
	public static List<ClasesJSON.Notificacion> notificaciones; //Cache de notificaciones

	[SerializeField]
	private GameObject prefabNotificacion; //Prefab de notificación
	[SerializeField]
	private GameObject prefabNada; //Prefab usado cuando no hay notificaciones ha mostrar
	[SerializeField]
	private Transform listaPadre; //Transform del objeto padre que tendra como hijos los elementos de la lista

	private const string ERROR_ACTUALIZARNOTIFICACIONES = "No se puede actualizar la lista de notificaciones";
	
	//Queremos actualizar las notificaciones en pantalla cada vez que se accede a la pantalla
	private void OnEnable() {
		ActualizarNotificaciones(false);
		//'false' ya que las notificaciones habrán sido descargadas de backend desde BotonNotificaciones.cs
		//Si se quieren recargar las notificaciones explicicamente al entrar a la pantalla, poner ActualizarNotificaciones(true)
	}

	/// <summary>Actualiza las notificaciones a mostrar al usuario.</summary>
	/// <param name="obtenerNotificaciones">Si 'true', también se descargan las notificaciones de backend</param>
	public async void ActualizarNotificaciones(bool obtenerNotificaciones) {

		//Borrar los gameobjects de las notificaciones anteriores
		for(int i = 0; i < listaPadre.childCount; i++) {
			Destroy(listaPadre.GetChild(i).gameObject);
		}

		//Retrivir las notificaciones del servidor si se ha indicado
		if(obtenerNotificaciones)
			await ObtenerNotificaciones(true);
		
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

	/// <summary> Obtiene las notificaciones del servidor y las guarda en la variable 'notificaciones' </summary>
	/// <param name="mostrarPantallaCarga">Si 'true', se muestra la pantalla de carga mientras la funcion se ejecute</param>
	// Las notificaciones pueden ser accedidas desde cualquier sitio (Como el menu principal) al estar guardadas en un campo estatico
	public static async Task ObtenerNotificaciones(bool mostrarPantallaCarga) {

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
		string respuesta;
		if(mostrarPantallaCarga)
			respuesta = await ConexionHTTP.instance.RequestHTTP("notificaciones", form);
		else
			respuesta = await ConexionHTTP.instance.RequestHTTPFantasma("notificaciones", form);

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

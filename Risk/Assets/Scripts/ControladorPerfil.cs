using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class ControladorPerfil : MonoBehaviour {
	public static ControladorPerfil instance;

	public TextMeshProUGUI nombreUsuario, riskos;
	public Image icono, aspecto;
	private string nuevoNombre, nuevaClave, nuevoCorreo;
	private int nuevoIcono, nuevoAspecto;
	private bool nuevoRecibeCorreos;
	private Usuario usuario;

	[SerializeField]
	private GameObject panelTienda = null; //Panel de la tienda
	[SerializeField]
	private Transform tr_listaIconos = null; //Transform de la lista de iconos en la tienda
	[SerializeField]
	private Transform tr_listaAspectos = null; //Transform de la lista de iconos en la tienda
	[SerializeField]
	private GameObject panelTienda_prefab = null; //Prefab que se usara para mostrar los objetos en venta en la tienda
	[SerializeField]
	private GameObject panelTienda_confirmacion = null; //Panel de confirmación que se muestra cuando se va a comprar algo

	private Animator animatorTienda = null; //Animator de tienda
	private int tiendaAbierta = 0; //1 si la tienda esta abierta

	public static ObjetoCompra objetoAComprar; //Objeto que se va a comprar cuando sale la ventana de confirmacións
	
	// Actualiza los datos de usuario cuando se abre la pantalla de perfil
	private void OnEnable() {
		instance = this;

		if(panelTienda != null) { //Obtener animador
			animatorTienda = panelTienda.GetComponent<Animator>();
			if(animatorTienda == null)
				Debug.LogWarning("En Panel de la Tienda no es nulo, pero si animador si");
		} else {
			Debug.LogWarning("En Panel de la Tienda es nulo, no se puede obtener animacion");
		}


		usuario = ControladorUI.instance.usuarioRegistrado;
		ActualizarDatosRepresentados();
		ActualizarTienda();
	}
	
	public void ActualizarNombre(string nombre){
		nuevoNombre = nombre;
	}
	
	public void ActualizarCorreo(string correo){
		nuevoCorreo = correo;
	}

	public void ActualizarClave(string clave){
		nuevaClave = ControladorConexiones.Cifrar(clave);
	}

	public void ActualizarIcono(int icono){
		nuevoIcono = icono;
		try {
			this.icono.overrideSprite = ControladorUI.instance.iconos[icono];
		} catch {}
	}

	public void ActualizarAspecto(int aspecto){
		nuevoAspecto = aspecto;
		try {
			this.aspecto.overrideSprite = ControladorUI.instance.aspectos[aspecto];
		} catch {}
	}

	public void ActualizarRecibeCorreo(bool recibeCorreos){
		nuevoRecibeCorreos = recibeCorreos;
	}

	// Recibe el tipo de dato a ser actualizado y envia petición de personalización al servidor
	public async void PersonalizarUsuario(string elemento) {
		// Crear formulario a enviar petición al servidor
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", usuario.id);
		form.AddField("clave", usuario.clave);
		form.AddField("tipo", elemento);
		switch (elemento) {
			case "Nombre":
				form.AddField("nuevoDato", nuevoNombre);
				break;
			case "Clave":
				form.AddField("nuevoDato", nuevaClave);
				break;
			case "Correo":
				form.AddField("nuevoDato", nuevoCorreo);
				break;
			case "Icono":
				form.AddField("nuevoDato", nuevoIcono);
				break;
			case "Aspecto":
				form.AddField("nuevoDato", nuevoAspecto);
				break;
			case "RecibeCorreos":
				form.AddField("nuevoDato", nuevoRecibeCorreos ? 1 : 0);
				break;
			default:
				Debug.Log("Elemento a personalizar \"" + elemento + "\" no conocido");
				return;
		}
		string recibido = await ControladorConexiones.instance.RequestHTTP("personalizarUsuario", form);
		print(recibido);
		try {
			// Vemos si hay error
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido, ClasesJSON.settings);
			if(error.code != 0){
				ControladorUI.instance.PantallaError(error.err);
			} else {
				// No hay error, actualizar datos locales
				ControladorUI.instance.usuarioRegistrado.nombre = nuevoNombre;
				ControladorUI.instance.usuarioRegistrado.correo = nuevoCorreo;
				ControladorUI.instance.usuarioRegistrado.clave = nuevaClave;
				ControladorUI.instance.usuarioRegistrado.recibeCorreos = nuevoRecibeCorreos;
				ControladorUI.instance.usuarioRegistrado.aspecto = nuevoAspecto;
				ControladorUI.instance.usuarioRegistrado.icono = nuevoIcono;
				usuario = ControladorUI.instance.usuarioRegistrado;
				ActualizarDatosRepresentados();
			}
		} catch {
			ControladorUI.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
		}
	}
	
	// Actualiza los campos mostrados al usuario mediante la interfaz
	private void ActualizarDatosRepresentados(){
		nuevoNombre = usuario.nombre;
		nuevaClave = usuario.clave;
		nuevoCorreo = usuario.correo;
		nuevoIcono = usuario.icono;
		nuevoAspecto = usuario.aspecto;
		nuevoRecibeCorreos = usuario.recibeCorreos;
		nombreUsuario.text = usuario.nombre;
		riskos.text = usuario.riskos.ToString();
		icono.sprite = ControladorUI.instance.iconos[usuario.icono];
		aspecto.sprite = ControladorUI.instance.iconos[usuario.icono];
	}


	/*
		Lógica de la Tienda
	*/
	// Actualiza los gameobjects de la tienda
	public void ActualizarTienda() {
		Debug.Log("Actualizando Tienda...");
		//Borrar los gameobjects de las listas de iconos y aspectos
		for(int i = 0; i < tr_listaAspectos.childCount; i++) {
			Destroy(tr_listaAspectos.GetChild(i).gameObject);
		}
		for(int i = 0; i < tr_listaIconos.childCount; i++) {
			Destroy(tr_listaIconos.GetChild(i).gameObject);
		}

		//Abortar si no hay listas de aspectos o iconos en la tienda
		if(ControladorSesion.iconos_tienda == null || ControladorSesion.aspectos_tienda == null) {
			Debug.LogError("iconos_tienda y/o aspectos_tienda es nulo/s");
			return;
		}

		//Añadir prefabs
		//Iconos
		foreach (ClasesJSON.Icono i in ControladorSesion.iconos_tienda.tiendaIconos) {
				ObjetoCompra go_oc = Instantiate(panelTienda_prefab, tr_listaIconos).GetComponent<ObjetoCompra>();
				go_oc.Actualizar(i);
		}
		//Aspectos
		foreach (ClasesJSON.Aspecto i in ControladorSesion.aspectos_tienda.tiendaAspectos) {
				ObjetoCompra go_oc = Instantiate(panelTienda_prefab, tr_listaAspectos).GetComponent<ObjetoCompra>();
				go_oc.Actualizar(i);
		}
	}
	
	//Abre y cierra la tienda
	public void ToggleTienda() {
		if(animatorTienda != null)
		{
			animatorTienda.SetInteger("state", 1 - tiendaAbierta);
			tiendaAbierta = 1 - tiendaAbierta;
		}
	}

	//Abre la ventana de confirmación de compra
	public void AbrirConfirmacionCompra(ObjetoCompra oc) {
		objetoAComprar = oc;
		panelTienda_confirmacion.SetActive(true);
	}

	//Confirma la compra desde el menu de confirmación
	public void ConfirmarCompra() {
		objetoAComprar.Comprar();
		panelTienda_confirmacion.SetActive(false);
	}

	//Cambia el nuevo icono del usuario en la pantalla de perfil
	public void CambiarIcono(int direccion) {
		int orig = ControladorUI.instance.usuarioRegistrado.icono;
		int c = orig;
		int MAX_TRIES = 14;

		for(int i = 0; i < MAX_TRIES; i++) {
			c += direccion;
			bool end = false;

			foreach(var obj in ControladorSesion.iconos_comprados.iconos)
				if(obj.id == c) {
					ActualizarIcono(c);
					end = true;
					break;
				}
				
			if(end) break;
		}
	}

	//Cambia el nuevo aspecto del usuario en la pantalla de perfil
	public void CambiarAspecto(int direccion) {
		int orig = ControladorUI.instance.usuarioRegistrado.aspecto;
		int c = orig;
		int MAX_TRIES = 14;

		for(int i = 0; i < MAX_TRIES; i++) {
			c += direccion;
			bool end = false;

			foreach(var obj in ControladorSesion.aspectos_comprados.aspectos)
				if(obj.id == c) {
					ActualizarIcono(c);
					end = true;
					break;
				}

			if(end) break;
		}
	}
}

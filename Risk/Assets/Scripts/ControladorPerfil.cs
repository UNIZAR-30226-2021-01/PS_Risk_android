using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

/// <summary>
/// Script de control de la pantala de perfil
/// </summary>
public class ControladorPerfil : MonoBehaviour {

	/// <summary>Compontente de Texto que muestra el nombre del usuario</summary>
	public TextMeshProUGUI nombreUsuario;

	/// <summary>Compontente de Texto que muestra el número de riskos (Moneda del juego) que tiene el usuario</summary>
	public TextMeshProUGUI riskos;

	/// <summary>Imagen que muestra el icono seleccionado.</summary>
	public Image icono;

	/// <summary>Imagen que muestra el aspecto seleccionado.</summary>
	public Image aspecto;

	/// <summary>Guarda el cosmetico que se desea comprar cuando el usuario esta en la ventana de confirmación de venta.</summary>
	public static ObjetoCompra objetoAComprar; //Objeto que se va a comprar cuando sale la ventana de confirmacións


	private string nuevoNombre = "", nuevaClave = "", nuevoCorreo = "";
	private int nuevoIcono, nuevoAspecto;
	private bool nuevoRecibeCorreos;
	private Usuario usuario;
	[SerializeField]
	private GameObject panelTienda; //Panel de la tienda
	[SerializeField]
	private Transform listaIconos; //Transform de la lista de iconos en la tienda
	[SerializeField]
	private Transform listaAspectos; //Transform de la lista de iconos en la tienda
	[SerializeField]
	private GameObject prefabObjetoCompra; //Prefab que se usara para mostrar los objetos en venta en la tienda
	[SerializeField]
	private GameObject confirmacionPanelTienda; //Panel de confirmación que se muestra cuando se va a comprar algo
	[SerializeField]
	private Animator animatorTienda; //Animator de tienda
	[SerializeField]
	private Toggle toggleCorreo;
	[SerializeField]
	private RectTransform rectPanelTienda;
	private bool tiendaAbierta = false;
	
	// Actualiza los datos de usuario cuando se abre la pantalla de perfil
	
	private void Awake() {
		#if UNITY_EDITOR
			Camera mainCamera = Camera.main;
			rectPanelTienda.sizeDelta = new Vector2(mainCamera.pixelWidth, mainCamera.pixelHeight);
			rectPanelTienda.anchoredPosition = new Vector2(mainCamera.pixelWidth/2, 0);
		#else
			rectPanelTienda.sizeDelta = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height);
			rectPanelTienda.anchoredPosition = new Vector2(Screen.currentResolution.width/2, 0);
		#endif
		RectTransform rtMenu = ControladorPrincipal.instance.GetComponent<RectTransform>();
		rectPanelTienda.sizeDelta = rtMenu.sizeDelta;
		rectPanelTienda.anchoredPosition = new Vector2(rtMenu.sizeDelta.x/2, 0);
		rectPanelTienda.ForceUpdateRectTransforms();
	}

	private void OnEnable() {
		usuario = ControladorPrincipal.instance.usuarioRegistrado;
		ActualizarDatosRepresentados();
		ActualizarTienda();
		toggleCorreo.SetIsOnWithoutNotify(usuario.recibeCorreos);
		nuevoRecibeCorreos = usuario.recibeCorreos;
	}
	
	/// <summary>Guarda un nuevo nombre de usuario para actualizarlo en el futuro.</summary>
    /// Ver <see cref="ControladorPerfil.PersonalizarUsuario()"/> para actualizar el dato permanentemente
	/// <param name="nombre">Nuevo nombre de usuario</param>
	public void ActualizarNombre(string nombre){
		nuevoNombre = nombre;
	}
	
	/// <summary>Guarda un nuevo correo del usuario para actualizarlo en el futuro.</summary>
    /// Ver <see cref="ControladorPerfil.PersonalizarUsuario()"/> para actualizar el dato permanentemente
	/// <param name="correo">Nuevo correo</param>
	public void ActualizarCorreo(string correo){
		nuevoCorreo = correo;
	}

	/// <summary>Guarda una nueva contraseña del usuario, cifrandola, para actualizarlo en el futuro.</summary>
    /// Ver <see cref="ControladorPerfil.PersonalizarUsuario()"/> para actualizar el dato permanentemente
	/// <param name="clave">Nueva contraseña (Sin cifrar)</param>
	public void ActualizarClave(string clave){
		nuevaClave = ConexionHTTP.Cifrar(clave);
	}

	/// <summary>Guarda un nuevo icono de usuario para actualizarlo en el futuro.</summary>
    /// Ver <see cref="ControladorPerfil.PersonalizarUsuario()"/> para actualizar el dato permanentemente
	/// <param name="icono">Nuevo icono</param>
	public void ActualizarIcono(int icono){
		nuevoIcono = icono;
		try {
			this.icono.overrideSprite = ControladorPrincipal.instance.iconos[icono];
		} catch {}
	}

	/// <summary>Guarda un nuevo aspecto de usuario para actualizarlo en el futuro.</summary>
    /// Ver <see cref="ControladorPerfil.PersonalizarUsuario()"/> para actualizar el dato permanentemente
	/// <param name="aspecto">Nuevo aspecto</param>
	public void ActualizarAspecto(int aspecto){
		nuevoAspecto = aspecto;
		try {
			this.aspecto.overrideSprite = ControladorPrincipal.instance.aspectos[aspecto];
		} catch {}
	}

	/// <summary>Guarda la opción de recibir correos para actualizarlo en el futuro.</summary>
    /// Ver <see cref="ControladorPerfil.PersonalizarUsuario()"/> para actualizar el dato permanentemente
	/// <param name="recibeCorreos">'true' para recibir correos y viceversa</param>
	public void ActualizarRecibeCorreo(bool recibeCorreos){
		nuevoRecibeCorreos = recibeCorreos;
	}

	/// <summary>Recibe el tipo de dato a ser actualizado y envia petición de personalización al servidor.</summary>
	/// <param name="elemento">Elemento a actualizar: "Nombre", "Clave", "Correo", "Icono", "Aspecto" o "RecibeCorreos"</param>
	/// <seealso cref="ControladorPerfil.ActualizarNombre(string)"/>
	/// <seealso cref="ControladorPerfil.ActualizarCorreo(string)"/>
	/// <seealso cref="ControladorPerfil.ActualizarClave(string)"/>
	/// <seealso cref="ControladorPerfil.ActualizarIcono(int)"/>
	/// <seealso cref="ControladorPerfil.ActualizarAspecto(int)"/>
	/// <seealso cref="ControladorPerfil.ActualizarRecibeCorreo(bool)"/>
	public async void PersonalizarUsuario(string elemento) {
		// Crear formulario a enviar petición al servidor
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", usuario.id);
		form.AddField("clave", usuario.clave);
		form.AddField("tipo", elemento);
		switch (elemento) {
			case "Nombre":
				if(nuevoNombre == "" || nuevoNombre.Contains("@")){
					ControladorPrincipal.instance.PantallaError("Nuevo nombre inválido");
					return;
				}
				form.AddField("nuevoDato", nuevoNombre);
				break;
			case "Clave":
				if(nuevaClave == ""){
					ControladorPrincipal.instance.PantallaError("Nueva contraseña inválida");
					return;
				}
				form.AddField("nuevoDato", nuevaClave);
				break;
			case "Correo":
				if(nuevoCorreo == ""){
					ControladorPrincipal.instance.PantallaError("Nuevo Correo inválido");
					return;
				}
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
		string recibido = await ConexionHTTP.instance.RequestHTTP("personalizarUsuario", form);
		//print(recibido);
		try {
			// Vemos si hay error
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido, ClasesJSON.settings);
			if(error.code != 0){
				ControladorPrincipal.instance.PantallaError(error.err);
			} else {
				// No hay error, actualizar datos locales
				ControladorPrincipal.instance.usuarioRegistrado.nombre = nuevoNombre;
				ControladorPrincipal.instance.usuarioRegistrado.correo = nuevoCorreo;
				ControladorPrincipal.instance.usuarioRegistrado.clave = nuevaClave;
				ControladorPrincipal.instance.usuarioRegistrado.recibeCorreos = nuevoRecibeCorreos;
				ControladorPrincipal.instance.usuarioRegistrado.aspecto = nuevoAspecto;
				ControladorPrincipal.instance.usuarioRegistrado.icono = nuevoIcono;
				usuario = ControladorPrincipal.instance.usuarioRegistrado;
				ActualizarDatosRepresentados();
			}
		} catch {
			ControladorPrincipal.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
		}
	}
	
	/// <summary>Actualiza los campos mostrados al usuario mediante la interfaz.</summary>
	private void ActualizarDatosRepresentados(){
		nuevoNombre = usuario.nombre;
		nuevaClave = usuario.clave;
		nuevoCorreo = usuario.correo;
		ActualizarIcono(usuario.icono);
		ActualizarAspecto(usuario.aspecto);
		nuevoRecibeCorreos = usuario.recibeCorreos;
		nombreUsuario.text = usuario.nombre;
		icono.sprite = ControladorPrincipal.instance.iconos[usuario.icono];
		aspecto.sprite = ControladorPrincipal.instance.aspectos[usuario.icono];
	}

	/// <summary>Actualiza los gameobjects de la tienda.</summary>
	public void ActualizarTienda() {
		riskos.text = usuario.riskos.ToString(); //Actualizar cuantos riskos quedan (Riskos™)

		//Borrar los gameobjects de las listas de iconos y aspectos
		for(int i = 0; i < listaAspectos.childCount; i++) {
			Destroy(listaAspectos.GetChild(i).gameObject);
		}
		for(int i = 0; i < listaIconos.childCount; i++) {
			Destroy(listaIconos.GetChild(i).gameObject);
		}

		//Abortar si no hay listas de aspectos o iconos en la tienda
		if(ControladorPrincipal.instance.iconosTienda == null || ControladorPrincipal.instance.aspectosTienda == null) {
			return;
		}

		//Añadir prefabs
		//Iconos
		foreach (ClasesJSON.Icono i in ControladorPrincipal.instance.iconosTienda.tiendaIconos) {
				ObjetoCompra objCom = Instantiate(prefabObjetoCompra, listaIconos).GetComponent<ObjetoCompra>();
				objCom.controladorPerfil = this;
				objCom.Actualizar(i);
		}
		//Aspectos
		foreach (ClasesJSON.Aspecto i in ControladorPrincipal.instance.aspectosTienda.tiendaAspectos) {
				ObjetoCompra objCom = Instantiate(prefabObjetoCompra, listaAspectos).GetComponent<ObjetoCompra>();
				objCom.controladorPerfil = this;
				objCom.Actualizar(i);
		}
	}
	
	/// <summary>Abre y cierra la tienda.</summary>
	public void ToggleTienda() {
		if(animatorTienda != null)
		{
			animatorTienda.SetInteger("state", (tiendaAbierta ? 0 : 1));
			tiendaAbierta = !tiendaAbierta;
		}
	}

	/// <summary>Abre la ventana de confirmación de compra.</summary>
	/// <param name="oc">El cosmetico que se esta confirmando su compra</param>
	public void AbrirConfirmacionCompra(ObjetoCompra oc) {
		objetoAComprar = oc;
		confirmacionPanelTienda.SetActive(true);
	}

	/// <summary>Confirma la compra desde el menu de confirmación.</summary>
	public void ConfirmarCompra() {
		objetoAComprar.Comprar();
		confirmacionPanelTienda.SetActive(false);
	}

	/// <summary>Cambia el nuevo icono del usuario en la pantalla de perfil.</summary>
	/// <param name="direccion">Dirección de cambio de icono, '1' para el siguiente icono, '-1' para el icono anterior</param>
	public void CambiarIcono(int direccion) {
		int orig = nuevoIcono;
		int c = orig;
		int MAX_TRIES = ControladorPrincipal.instance.iconos.Length;

		for(int i = 0; i < MAX_TRIES; i++) {
			c += direccion;
			c = (c + MAX_TRIES) % MAX_TRIES; //Asegurar que no nos pasemos de busqueda
			bool end = false;

			foreach(var obj in ControladorPrincipal.instance.iconosComprados.iconos)
				if(obj.id == c) {
					ActualizarIcono(c);
					end = true;
					break;
				}
				
			if(end) break;
		}

		PersonalizarUsuario("Icono");
	}

	/// <summary>Cambia el nuevo aspecto del usuario en la pantalla de perfil.</summary>
	/// <param name="direccion">Dirección de cambio de aspecto, '1' para el siguiente aspecto, '-1' para el aspecto anterior</param>
	public void CambiarAspecto(int direccion) {
		int orig = nuevoAspecto;
		int c = orig;
		int MAX_TRIES = ControladorPrincipal.instance.aspectos.Length;

		for(int i = 0; i < MAX_TRIES; i++) {
			c += direccion;
			c = (c + MAX_TRIES) % MAX_TRIES; //Asegurar que no nos pasemos de busqueda
			bool end = false;

			foreach(var obj in ControladorPrincipal.instance.aspectosComprados.aspectos)
				if(obj.id == c) {
					ActualizarAspecto(c);
					end = true;
					break;
				}

			if(end) break;
		}

		PersonalizarUsuario("Aspecto");
	}
}

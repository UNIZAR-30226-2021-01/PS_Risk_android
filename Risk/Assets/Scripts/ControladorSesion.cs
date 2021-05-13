using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Script usado para controlar los datos de la Sesión del Usuario
/// </summary>
public class ControladorSesion : MonoBehaviour
{
	private string usuario = "", clave = "", correo = "";
	private bool recibeCorreos = true;
	
	/// <summary>
	/// Actualiza el nombre del usuario.
	/// </summary>
	/// <param name="nuevoUsuario">Nuevo nombre de usuario</param>
	public void ActualizarUsuario(string nuevoUsuario) {
		usuario = nuevoUsuario;
	}
	
	/// <summary>
	/// Actualiza la clave del usuario.
	/// </summary>
	/// <param name="nuevaClave">Nueva clave de usuario</param>
	public void ActualizarClave(string nuevaClave) {
		clave = nuevaClave;
	}
	
	/// <summary>
	/// Actualiza el correo del usuario.
	/// </summary>
	/// <param name="nuevoCorreo">Nuevo Correo del usuario</param>
	public void ActualizarCorreo(string nuevoCorreo) {
		correo = nuevoCorreo;
	}
	
	/// <summary>
	/// Actualiza si el usuario recibe correos o no.
	/// </summary>
	/// <param name="nuevoRecibir">Indica si el usuario recibe correos (true) o no (false)</param>
	public void ActualizarRecibeCorreo(bool nuevoRecibir) {
		recibeCorreos = nuevoRecibir;
	}
	
	/// <summary>
	/// Envia una petición de inicio de sesión al backend y recibe los datos del usuario si el usuario y contraseña son correctos
	/// </summary>
	public async void IniciarSesion() {
		if(usuario == "" || clave == "") {
			ControladorPrincipal.instance.PantallaError("Se deben rellenar todos los campos para hacer el registro");
			return;
		}
		// Crear formulario a enviar petición al servidor
		WWWForm form = new WWWForm();
		form.AddField("usuario", usuario);
		form.AddField("clave", ConexionHTTP.Cifrar(clave));
		string result = await ConexionHTTP.instance.RequestHTTP("iniciarSesion",form);
		LoggearUsuario(result);
	}
	
	/// <summary>
	/// Envia al backend una petición de registrar usuario si los datos dados son correctos. Recibe los datos básicos del usuario.
	/// </summary>
	public async void Registrarse() {
		if (usuario == "" || clave == "") {
			ControladorPrincipal.instance.PantallaError("Se deben rellenar todos los campos para hacer el registro");
			return;
		}
		if (usuario.Contains("@")) {
			ControladorPrincipal.instance.PantallaError("Caracter @ no permitido en nombre de usuario");
			return;
		}
		// Crear formulario a enviar petición al servidor
		WWWForm form = new WWWForm();
		form.AddField("nombre", usuario);
		form.AddField("correo", correo);
		form.AddField("clave", ConexionHTTP.Cifrar(clave));
		form.AddField("recibeCorreos", recibeCorreos ? 1 : 0);
		string result = await ConexionHTTP.instance.RequestHTTP("registrar", form);
		LoggearUsuario(result);
	}
	
	public async void OlvidoClave(){
		if (correo == "") {
			ControladorPrincipal.instance.PantallaError("Se debe introducir el correo de la cuenta");
			return;
		}
		WWWForm form = new WWWForm();
		form.AddField("correo", correo);
		string result = await ConexionHTTP.instance.RequestHTTP("olvidoClave", form);
		try {
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(result);
			if (error.code != 0) {
				ControladorPrincipal.instance.PantallaError(error.err);
			} else {
				ControladorPrincipal.instance.PantallaInfo("Restablecimiento solicitado con éxito, se ha enviado un correo con las instrucciones para restablecer la cuenta");
			}
		} catch {}
	}
	
	public async void RecargarUsuario() {
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorPrincipal.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorPrincipal.instance.usuarioRegistrado.clave);
		string result = await ConexionHTTP.instance.RequestHTTP("recargarUsuario", form);
		LoggearUsuario(result);
	}
	
	// Intenta acceder a la cuenta del usuario y en caso exitoso entra al menú principal
	private void LoggearUsuario(string recibido) {
		try {
			ClasesJSON.UsuarioCompleto usuarioCompleto = JsonConvert.DeserializeObject<ClasesJSON.UsuarioCompleto>(recibido, ClasesJSON.settings);
			usuarioCompleto.usuario.clave = ConexionHTTP.Cifrar(clave);
			ControladorPrincipal.instance.usuarioRegistrado = usuarioCompleto.usuario;

			ObtenerIconosAspectos(recibido);

			ControladorPrincipal.instance.AbrirPantalla("Principal");
		} catch {
			try {
				ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido);
				ControladorPrincipal.instance.PantallaError(error.err);
			} catch {
				ControladorPrincipal.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
			}
		}
	}

	//Guarda que aspectos/iconos tiene el usuario y cuales estan disponibles en la tienda
	private void ObtenerIconosAspectos(string recibido) {
		//Borrar aspectos e iconos anteriores
		ControladorPrincipal.instance.aspectosComprados = new ClasesJSON.ListaAspectosUsuario();
		ControladorPrincipal.instance.iconosComprados = new ClasesJSON.ListaIconosUsuario();
		ControladorPrincipal.instance.aspectosTienda = new ClasesJSON.ListaAspectosTienda();
		ControladorPrincipal.instance.iconosTienda = new ClasesJSON.ListaIconosTienda();

		//Cargar desde api
		try {
			//No usar las settings del parser de JSON aquí, no puede ser un error aquí si se llama desde LoggearUsuario()
			ControladorPrincipal.instance.aspectosComprados = JsonConvert.DeserializeObject<ClasesJSON.ListaAspectosUsuario>(recibido);
			ControladorPrincipal.instance.iconosComprados = JsonConvert.DeserializeObject<ClasesJSON.ListaIconosUsuario>(recibido);
			ControladorPrincipal.instance.aspectosTienda = JsonConvert.DeserializeObject<ClasesJSON.ListaAspectosTienda>(recibido);
			ControladorPrincipal.instance.iconosTienda = JsonConvert.DeserializeObject<ClasesJSON.ListaIconosTienda>(recibido);

		} catch {
		}
	}
	
}

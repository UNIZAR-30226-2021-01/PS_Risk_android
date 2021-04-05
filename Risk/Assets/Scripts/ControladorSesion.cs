using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ControladorSesion : MonoBehaviour
{
	private string usuario = "", clave = "", correo = "";
	private bool recibeCorreos = true;
	
	public void ActualizarUsuario(string nuevoUsuario) {
		usuario = nuevoUsuario;
	}
	
	public void ActualizarClave(string nuevaClave) {
		clave = nuevaClave;
	}
	
	public void ActualizarCorreo(string nuevoCorreo) {
		correo = nuevoCorreo;
	}
	
	public void ActualizarRecibeCorreo(bool nuevoRecibir) {
		recibeCorreos = nuevoRecibir;
	}
	
	public async void IniciarSesion() {
		if(usuario == "" || clave == "") {
			ControladorUI.instance.PantallaError("Se deben rellenar todos los campos para hacer el registro");
			return;
		}
		// Crear formulario a enviar petición al servidor
		WWWForm form = new WWWForm();
		form.AddField("usuario", usuario);
		form.AddField("clave", ControladorConexiones.Cifrar(clave));
		string result = await ControladorConexiones.instance.RequestHTTP("iniciarSesion",form);
		LoggearUsuario(result);
	}
	
	public async void Registrarse() {
		if(usuario == "" || clave == "" || correo == "") {
			ControladorUI.instance.PantallaError("Se deben rellenar todos los campos para hacer el registro");
			return;
		}
		if(usuario.Contains("@")) {
			ControladorUI.instance.PantallaError("Caracter @ no permitido en nombre de usuario");
			return;
		}
		// Crear formulario a enviar petición al servidor
		WWWForm form = new WWWForm();
		form.AddField("nombre", usuario);
		form.AddField("correo", correo);
		form.AddField("clave", ControladorConexiones.Cifrar(clave));
		form.AddField("recibeCorreos", recibeCorreos ? 1 : 0);
		string result = await ControladorConexiones.instance.RequestHTTP("registrar", form);
		LoggearUsuario(result);
	}
	
	// Intenta acceder a la cuenta del usuario y en caso exitoso entra al menú principal
	private void LoggearUsuario(string recibido) {
		try {
			ClasesJSON.UsuarioCompleto usuarioCompleto = JsonConvert.DeserializeObject<ClasesJSON.UsuarioCompleto>(recibido, ClasesJSON.settings);
			ControladorUI.instance.usuarioRegistrado = usuarioCompleto.usuario;

			ObtenerIconosAspectos(recibido);

			ControladorUI.instance.AbrirPantalla("Principal");
		} catch {
			try {
				ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido);
				ControladorUI.instance.PantallaError(error.err);
			} catch {
				ControladorUI.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
			}
		}
	}

	//Guarda que aspectos/iconos tiene el usuario y cuales estan disponibles en la tienda
	private void ObtenerIconosAspectos(string recibido) {
		//Borrar aspectos e iconos anteriores
		ControladorUI.aspectosComprados = new ClasesJSON.ListaAspectosUsuario();
		ControladorUI.iconosComprados = new ClasesJSON.ListaIconosUsuario();
		ControladorUI.aspectosTienda = new ClasesJSON.ListaAspectosTienda();
		ControladorUI.iconosTienda = new ClasesJSON.ListaIconosTienda();

		//Cargar desde api
		try {
			//No usar las settings del parser de JSON aquí, no puede ser un error aquí si se llama desde LoggearUsuario()
			ControladorUI.aspectosComprados = JsonConvert.DeserializeObject<ClasesJSON.ListaAspectosUsuario>(recibido);
			ControladorUI.iconosComprados = JsonConvert.DeserializeObject<ClasesJSON.ListaIconosUsuario>(recibido);
			ControladorUI.aspectosTienda = JsonConvert.DeserializeObject<ClasesJSON.ListaAspectosTienda>(recibido);
			ControladorUI.iconosTienda = JsonConvert.DeserializeObject<ClasesJSON.ListaIconosTienda>(recibido);

		} catch (System.Exception e) {
			Debug.LogError("Error al parsear los aspectos e iconos: " + e);
			throw new System.Exception("No se ha podido leer los aspectos e iconos");
		}
	}
	
}

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
	
	public async void Registrarse() {
		if(usuario == "" || clave == "" || correo == "") {
			ControladorPrincipal.instance.PantallaError("Se deben rellenar todos los campos para hacer el registro");
			return;
		}
		if(usuario.Contains("@")) {
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
	
	// Intenta acceder a la cuenta del usuario y en caso exitoso entra al menú principal
	private void LoggearUsuario(string recibido) {
		try {
			ClasesJSON.UsuarioCompleto usuarioCompleto = JsonConvert.DeserializeObject<ClasesJSON.UsuarioCompleto>(recibido, ClasesJSON.settings);
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

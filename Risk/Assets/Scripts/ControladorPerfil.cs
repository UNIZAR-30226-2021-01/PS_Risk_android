using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class ControladorPerfil : MonoBehaviour {
	public TextMeshProUGUI nombreUsuario, riskos;
	public Image icono, aspecto;
	public Sprite[] iconos, aspectos;
	private string nuevoNombre, nuevaClave, nuevoCorreo;
	private int nuevoIcono, nuevoAspecto;
	private bool nuevoRecibeCorreos;
	private Usuario usuario;
	
	// Actualiza los datos de usuario cuando se abre la pantalla de perfil
	private void OnEnable() {
		usuario = ControladorUI.instance.usuarioRegistrado;
		ActualizarDatosRepresentados();
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
	}

	public void ActualizarAspecto(int aspecto){
		nuevoAspecto = aspecto;
	}

	public void ActualizarRecibeCorreo(bool recibeCorreos){
		nuevoRecibeCorreos = recibeCorreos;
	}

	// Recibe el tipo de dato a ser actualizado
	public async void PersonalizarUsuario(string elemento) {
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
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido);
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
	}
	
	private void ActualizarDatosRepresentados(){
		nuevoNombre = usuario.nombre;
		nuevaClave = usuario.clave;
		nuevoCorreo = usuario.correo;
		nuevoIcono = usuario.icono;
		nuevoAspecto = usuario.aspecto;
		nuevoRecibeCorreos = usuario.recibeCorreos;
		nombreUsuario.text = usuario.nombre;
		riskos.text = "Riskos: " + usuario.riskos.ToString();
		icono.sprite = iconos[usuario.icono - 1];
		aspecto.sprite = iconos[usuario.icono - 1];
	}

}

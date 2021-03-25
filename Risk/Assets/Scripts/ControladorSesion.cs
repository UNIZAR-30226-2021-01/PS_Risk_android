﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ControladorSesion : MonoBehaviour
{
	private string usuario, clave, correo;
	private bool recibeCorreos = true;
	
	public void ActualizarUsuario(string nuevoUsuario)
	{
		usuario = nuevoUsuario;
	}
	
	public void ActualizarClave(string nuevaClave)
	{
		clave = nuevaClave;
	}
	
	public void ActualizarCorreo(string nuevoCorreo)
	{
		correo = nuevoCorreo;
	}
	
	public void ActualizarRecibeCorreo(bool nuevoRecibir){
		recibeCorreos = nuevoRecibir;
	}
	
	public async void IniciarSesion()
	{
		// Crear formulario a enviar petición al servidor
		WWWForm form = new WWWForm();
		form.AddField("usuario", usuario);
		form.AddField("clave", ControladorConexiones.Cifrar(clave));
		string result = await ControladorConexiones.instance.RequestHTTP("iniciarSesion",form);
		LoggearUsuario(result);
	}
	
	public async void Registrarse()
	{
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
	private void LoggearUsuario(string recibido){
		try {
			ClasesJSON.UsuarioCompleto usuarioCompleto = JsonConvert.DeserializeObject<ClasesJSON.UsuarioCompleto>(recibido, ClasesJSON.settings);
			ControladorUI.instance.usuarioRegistrado = usuarioCompleto.usuario;
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
}

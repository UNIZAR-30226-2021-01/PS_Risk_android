﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ControladorSesion : MonoBehaviour
{
	private string usuario, clave, correo;
	private bool recibeCorreos = true;

	public static ClasesJSON.ListaAspectosUsuario aspectos_comprados; //Lista de aspectos que el usuario tiene comprados
	public static ClasesJSON.ListaIconosUsuario iconos_comprados; //Lista de iconos que el usuario tiene comprados
	public static ClasesJSON.ListaAspectosTienda aspectos_tienda; //Lista de aspectos en la tienda
	public static ClasesJSON.ListaIconosTienda iconos_tienda; //Lista de iconos en la tienad
	
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
		aspectos_comprados = new ClasesJSON.ListaAspectosUsuario();
		iconos_comprados = new ClasesJSON.ListaIconosUsuario();
		aspectos_tienda = new ClasesJSON.ListaAspectosTienda();
		iconos_tienda = new ClasesJSON.ListaIconosTienda();

		//Cargar desde api
		try {
			//No usar las settings del parser de JSON aquí, no puede ser un error aquí si se llama desde LoggearUsuario()
			aspectos_comprados = JsonConvert.DeserializeObject<ClasesJSON.ListaAspectosUsuario>(recibido);
			iconos_comprados = JsonConvert.DeserializeObject<ClasesJSON.ListaIconosUsuario>(recibido);
			aspectos_tienda = JsonConvert.DeserializeObject<ClasesJSON.ListaAspectosTienda>(recibido);
			iconos_tienda = JsonConvert.DeserializeObject<ClasesJSON.ListaIconosTienda>(recibido);

		} catch (System.Exception e) {
			Debug.LogError("Error al parsear los aspectos e iconos: " + e);
			throw new System.Exception("No se ha podido leer los aspectos e iconos");
		}
	}
}

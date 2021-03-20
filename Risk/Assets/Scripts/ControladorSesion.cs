using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControladorSesion : MonoBehaviour
{
	private string usuario, clave, correo;
	private bool recibeCorreos;
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
		WWWForm form = new WWWForm();
		form.AddField("usuario", usuario);
		form.AddField("clave", clave);
		string result = await ControladorConexiones.instance.RequestHTTP("iniciarSesion",form);
		// TODO: Mover a pantalla principal si exito o a pantalla de error si ha dado error
		print("Recibido de RequestHTTP: " + result);
	}
	
	public async void Registrarse()
	{
		WWWForm form = new WWWForm();
		form.AddField("usuario", usuario);
		form.AddField("correo", correo);
		form.AddField("clave", clave);
		form.AddField("recibeCorreos", recibeCorreos.ToString());
		string result = await ControladorConexiones.instance.RequestHTTP("registrar", form);
		// TODO: Mover a pantalla principal si exito o a pantalla de error si ha dado error
		print("Recibido de RequestHTTP: " + result);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Script usado en la creación y modificación de una sala de espera
/// </summary>
public class CrearSala : MonoBehaviour {
	private string nombre;
	private int tiempo;
	
	/// <summary>
	/// Actualiza el nombre de la sala
	/// </summary>
	/// <param name="s">Nuevo nombre para la sala</param>
	public void ActualizarNombre(string s){
		nombre = s;
	}
	
	/// <summary>
	/// Opción de Partida, Nuevo tiempo de espera entre turnos
	/// </summary>
	/// <param name="s">Tiempo de espera entre turnos</param>
	public void ActualizarTiempo(string s) {
		tiempo = int.Parse(s);
	}
	
	/// <summary>
	/// Crea la Sala de Espera, realiza conexión y manda los datos de la sala al backend.
	/// </summary>
	public async void CreaSala(){
		ControladorPrincipal.instance.PantallaCarga(true);
		Usuario usuario = ControladorPrincipal.instance.usuarioRegistrado;
		ClasesJSON.CreacionSala datosSala = new ClasesJSON.CreacionSala(usuario.id, usuario.clave, tiempo, nombre);
		string datos = JsonConvert.SerializeObject(datosSala);
		if(!(await ConexionWS.instance.ConexionWebSocket("crearSala"))){
			ControladorPrincipal.instance.PantallaError("No se ha podido realizar la conexión con el servidor");
		} else {
			await ConexionWS.instance.EnviarWS(datos);
		}
		ControladorPrincipal.instance.PantallaCarga(false);
	}
}

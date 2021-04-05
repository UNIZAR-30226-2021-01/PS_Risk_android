﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class CrearSala : MonoBehaviour {
	private string nombre;
	private int tiempo;
	
	public void ActualizarNombre(string s){
		nombre = s;
	}
	
	public void ActualizarTiempo(string s) {
		tiempo = int.Parse(s);
	}
	
	public async void CreaSala(){
		Usuario usuario = ControladorUI.instance.usuarioRegistrado;
		ClasesJSON.CreacionSala datosSala = new ClasesJSON.CreacionSala(usuario.id, usuario.clave, tiempo, nombre);
		string datos = JsonConvert.SerializeObject(datosSala);
		await ControladorConexiones.instance.EnviarWS("crearSala", datos);
	}
}

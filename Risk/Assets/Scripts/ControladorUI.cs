﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControladorUI : MonoBehaviour
{
	public static ControladorUI instance; // Referencia estática a si mismo para usar como singleton
	public Dictionary<string, GameObject> pantallas;
	public Usuario usuarioRegistrado;
	public Sprite[] iconos, aspectos, colorAspectos; //Sprites para los iconos, aspectos y mascaras de color de los aspectos
	public string[] nombreIcono, nombreAspectos;
	public GameObject pantallaCarga, pantallaError;
	public TextMeshProUGUI textoError;

	//Cosmeticos desbloqueados
	public static ClasesJSON.ListaAspectosUsuario aspectosComprados; //Lista de aspectos que el usuario tiene comprados
	public static ClasesJSON.ListaIconosUsuario iconosComprados; //Lista de iconos que el usuario tiene comprados
	public static ClasesJSON.ListaAspectosTienda aspectosTienda; //Lista de aspectos en la tienda
	public static ClasesJSON.ListaIconosTienda iconosTienda; //Lista de iconos en la tienda
	
	private void Awake() {
		// Asignación de valor inicial de las variables de la clase
		instance = this;
		pantallas = new Dictionary<string, GameObject>();
		for(int i = 0; i < transform.childCount; i++) {
			GameObject hijo = transform.GetChild(i).gameObject;
			pantallas.Add(hijo.name, hijo);
		}
	}

	// Cambia la pantalla actual por la pantalla especificada, si existe
	public void AbrirPantalla(string pantalla) {
		GameObject objetoPantalla;
		if(pantallas.TryGetValue(pantalla, out objetoPantalla)) {
			DesactivarPantallas();
			objetoPantalla.SetActive(true);
		} else {
			Debug.LogError("No existe la pantalla especificada en el diccionario de pantallas");
		}
	}

	public void Salir(){
		#if UNITY_EDITOR
			print("Se ha salido de la aplicación");
		#endif
		Application.Quit();
	}

	public void PantallaError(string error) {
		pantallaError.SetActive(true);
		textoError.text = error;
	}
	
	public void PantallaCarga(bool activado) {
		pantallaCarga.SetActive(activado);
	}
	
	public void ActualizarUsuario(Usuario nuevo){
		usuarioRegistrado = nuevo;
	}

	private void DesactivarPantallas() {
		foreach (GameObject p in pantallas.Values) {
			// Desactivar todas las pantallas que no tengan el tag de mostrarSiempre
			if(!p.transform.CompareTag("mostrarSiempre")) p.SetActive(false);
		}
	}

}

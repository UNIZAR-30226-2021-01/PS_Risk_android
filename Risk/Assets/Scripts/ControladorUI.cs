using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorUI : MonoBehaviour
{
	public static ControladorUI instance; // Referencia estática a si mismo para usar como singleton
	public Dictionary<string, GameObject> pantallas;
	public Usuario usuarioRegistrado;
	public Sprite[] iconos, aspectos;
	
	private void Awake() {
		instance = this;
		pantallas = new Dictionary<string, GameObject>();
		// Empieza en 1 para ignorar el primer hijo
		for(int i = 1; i < transform.childCount; i++) {
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

	// Salir de la aplicación
	public void Salir(){
		#if UNITY_EDITOR
			print("Se ha salido de la aplicación");
		#endif
		Application.Quit();
	}

	public void PantallaError(string error){
		Debug.LogError("ERROR: " + error);
	}
	
	public void ActualizarUsuario(Usuario nuevo){
		usuarioRegistrado = nuevo;
	}

	// Desactiva todas las pantallas
	private void DesactivarPantallas() {
		foreach (GameObject p in pantallas.Values)
		{
			p.SetActive(false);
		}
	}

}

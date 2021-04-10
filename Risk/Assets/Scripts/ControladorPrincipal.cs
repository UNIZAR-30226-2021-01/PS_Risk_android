using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class ControladorPrincipal : MonoBehaviour
{
	public static ControladorPrincipal instance; // Referencia estática a si mismo para usar como singleton
	private Dictionary<string, GameObject> pantallas;
	public Usuario usuarioRegistrado;
	public Color[] coloresJugadores;
	public Sprite[] iconos, aspectos, colorAspectos; //Sprites para los iconos, aspectos y mascaras de color de los aspectos
	public string[] nombreIcono, nombreAspectos;
	[SerializeField]
	private GameObject pantallaCarga, pantallaError, pantallaInfo;
	public TextMeshProUGUI textoError, textoInfo;

	//Cosmeticos desbloqueados
	public ClasesJSON.ListaAspectosUsuario aspectosComprados; //Lista de aspectos que el usuario tiene comprados
	public ClasesJSON.ListaIconosUsuario iconosComprados; //Lista de iconos que el usuario tiene comprados
	public ClasesJSON.ListaAspectosTienda aspectosTienda; //Lista de aspectos en la tienda
	public ClasesJSON.ListaIconosTienda iconosTienda; //Lista de iconos en la tienda
	
	private void Awake() {
		// Asignación de valor inicial de las variables de la clase
		instance = this;
		pantallas = new Dictionary<string, GameObject>();
		for (int i = 0; i < transform.childCount; i++) {
			GameObject hijo = transform.GetChild(i).gameObject;
			pantallas.Add(hijo.name, hijo);
		}
	}

	// Cambia la pantalla actual por la pantalla especificada, si existe
	public void AbrirPantalla(string pantalla) {
		GameObject objetoPantalla;
		if (pantallas.TryGetValue(pantalla, out objetoPantalla)) {
			DesactivarPantallas();
			objetoPantalla.SetActive(true);
		} else {
			Debug.LogError("No existe la pantalla especificada en el diccionario de pantallas");
		}
	}

	public void Salir() {
		#if UNITY_EDITOR
			print("Se ha salido de la aplicación");
		#endif
		Application.Quit();
	}

	// Muestra la pantalla de error indicando el tipo de error
	// El string error contiene la estructura JSON de el error
	public void PantallaErrorWS(string riskErr) {
		ClasesJSON.RiskErrorWS error = JsonConvert.DeserializeObject<ClasesJSON.RiskErrorWS>(riskErr);
		if (error.code == 0) { 
			return;
		}
		pantallaError.SetActive(true);
		textoError.text = error.err;
	}

	// Muestra la pantalla de error indicando el tipo de error
	// El string error contiene el mensaje a ser mostrado
	public void PantallaError(string error) {
		pantallaError.SetActive(true);
		textoError.text = error;
	}

	// Muestra la pantalla de información
	public void PantallaInfo(string info) {
		pantallaInfo.SetActive(true);
		textoInfo.text = info;
	}
	
	public void PantallaCarga(bool activado) {
		pantallaCarga.SetActive(activado);
	}
	
	private void DesactivarPantallas() {
		foreach (GameObject p in pantallas.Values) {
			// Desactivar todas las pantallas que no tengan el tag de mostrarSiempre
			if(!p.transform.CompareTag("mostrarSiempre")) p.SetActive(false);
		}
	}

}

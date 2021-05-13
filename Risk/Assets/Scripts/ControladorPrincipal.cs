using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

/// <summary>
/// Script de control principal de la interfaz.
/// </summary>
public class ControladorPrincipal : MonoBehaviour
{
	/// <summary>
	/// Instancia del controlador en la escena del juego.
	/// </summary>
	public static ControladorPrincipal instance; // Referencia estática a si mismo para usar como singleton
	
	/// <summary>
	/// El usuario actualmente usando la aplicación.
	/// </summary>
	public Usuario usuarioRegistrado;
	
	/// <summary>
	/// Lista de los colores de los jugadores.
	/// </summary>
	public Color[] coloresJugadores;

	/// <summary>
	/// Lista de Sprites de los Iconos.
	/// </summary>
	public Sprite[] iconos;
	/// <summary>
	/// Lista de Sprites de los Aspectos.
	/// </summary>
	public Sprite[] aspectos;
	/// <summary>
	/// Lista de Sprites de los Overlays de color de los Aspectos.
	/// </summary>
	public Sprite[] colorAspectos; //Sprites para los iconos, aspectos y mascaras de color de los aspectos

	/// <summary>
	/// Lista de los nombres de los iconos.
	/// </summary>
	public string[] nombreIcono;
	/// <summary>
	/// Lista de los nombres de los aspectos.
	/// </summary>
	public string[] nombreAspectos;

	/// <summary>
	/// Componente de Texto de la pantalla de error.
	/// </summary>
	public TextMeshProUGUI textoError;
	/// <summary>
	/// Componente de Texto de la información de un error en la pantalla correspondiente.
	/// </summary>
	public TextMeshProUGUI textoInfo;

	//Cosmeticos desbloqueados
	/// <summary>Lista de aspectos que el usuario tiene comprados</summary>
	public ClasesJSON.ListaAspectosUsuario aspectosComprados; //Lista de aspectos que el usuario tiene comprados
	/// <summary>Lista de iconos que el usuario tiene comprados</summary>
	public ClasesJSON.ListaIconosUsuario iconosComprados; //Lista de iconos que el usuario tiene comprados
	/// <summary>Lista de aspectos en la tienda</summary>
	public ClasesJSON.ListaAspectosTienda aspectosTienda; //Lista de aspectos en la tienda
	/// <summary>Lista de iconos en la tienda</summary>
	public ClasesJSON.ListaIconosTienda iconosTienda; //Lista de iconos en la tienda


	[SerializeField]
	private GameObject pantallaCarga, pantallaError, pantallaInfo;
	private Dictionary<string, GameObject> pantallas;
	
	private Coroutine rutinaTimeout = null;
	
	private void Awake() {
		// Asignación de valor inicial de las variables de la clase
		instance = this;
		pantallas = new Dictionary<string, GameObject>();
		for (int i = 0; i < transform.childCount; i++) {
			GameObject hijo = transform.GetChild(i).gameObject;
			pantallas.Add(hijo.name, hijo);
		}
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}
	
	/// <summary>Cambia la pantalla actual por la pantalla especificada, si existe</summary>
	public void AbrirPantalla(string pantalla) {
		GameObject objetoPantalla;
		if (pantallas.TryGetValue(pantalla, out objetoPantalla)) {
			DesactivarPantallas();
			objetoPantalla.SetActive(true);
		} else {
			Debug.LogError("No existe la pantalla especificada en el diccionario de pantallas");
		}
	}

	/// <summary>Cerrar la aplicación</summary>
	public void Salir() {
		#if UNITY_EDITOR
			print("Se ha salido de la aplicación");
		#endif
		Application.Quit();
	}

	/// <summary>
	/// Muestra la pantalla de error con un mensaje de error personalizado.
	/// </summary>
	/// <param name="riskErr">Mensaje a ser mostrado</param>
	public void PantallaError(string error) {
		pantallaError.SetActive(true);
		textoError.text = error;
	}
	
	/// <summary>Muestra la pantalla de información</summary>
	/// <param name="info">Información a mostrar</param>
	public void PantallaInfo(string info) {
		pantallaInfo.SetActive(true);
		textoInfo.text = info;
	}
	
	/// <summary>Muestra/Esconde la pantalla de Carga</summary>
	/// <param name="activado">Si 'true', se activa la pantalla de carga, y viceversa</param>
	public void PantallaCarga(bool activado) {
		if (rutinaTimeout != null) {
			StopCoroutine(rutinaTimeout);
			rutinaTimeout = null;
		}
		if (activado) {
			rutinaTimeout = StartCoroutine("TimeoutError");
		}
		pantallaCarga.SetActive(activado);
	}
	
	// Espera 10 segundos a realizar el timeout
	IEnumerator TimeoutError() {
		yield return new WaitForSecondsRealtime(10f);
		AbrirPantalla("Inicio");
		PantallaError("Se ha sobrepasado el tiempo máximo de espera, volver a intentar más tarde");
		PantallaCarga(false);
	}

	private void DesactivarPantallas() {
		foreach (GameObject p in pantallas.Values) {
			// Desactivar todas las pantallas que no tengan el tag de mostrarSiempre
			if(!p.transform.CompareTag("mostrarSiempre")) p.SetActive(false);
		}
	}

}

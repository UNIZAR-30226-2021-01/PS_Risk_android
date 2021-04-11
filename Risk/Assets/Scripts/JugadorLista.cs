using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script usado en el prefab de un Jugador en la Lista de Jugadores.
/// </summary>
public class JugadorLista : MonoBehaviour {

	/// <summary>
	/// Imagen que muestra el icono del jugador.
	/// </summary>
	public Image icono;

	/// <summary>
	/// Imagen que muestra el aspecto de tropas del jugador.
	/// </summary>
	public Image tropa;

	/// <summary>
	/// Overlay del aspecto de tropas usado para darle color.
	/// </summary>
	public Image tropaOveraly;
	
	/// <summary>
	/// Campo de Texto que muestra el nombre del jugador
	/// </summary>
	public TextMeshProUGUI nombre;

	/// <summary>
	/// ID del jugador
	/// </summary>
	public int id;
	
}
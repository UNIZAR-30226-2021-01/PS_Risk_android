using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script usado en el prefab de lista de amigos a invitar, en salas de espera
/// </summary>
public class AmigoInvitar : MonoBehaviour {
	[SerializeField]
	private TextMeshProUGUI nombre;
	[SerializeField]
	private Image Icono;

	/// <summary>ID del amigo a invitar.</summary>
	public int id;

	/// <summary>Instancia del controlador de la sala de espera usada en la escena.</summary>
	public ControladorSalaEspera contSalaEspera;
}

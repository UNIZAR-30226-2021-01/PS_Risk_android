using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorInterfazPartida : MonoBehaviour {
	[SerializeField]
	private GameObject fondoMenu, partida;
	[SerializeField]
	private GameObject ventanaFin, ventanaRefuerzos, ventanaAtaque, ventanaMovimiento;

	private void OnEnable() {
		fondoMenu.SetActive(false); // Animacion de fundido en el futuro (?)
		partida.SetActive(true);
	}
	
	private void OnDisable() {
		fondoMenu.SetActive(true); // Animacion de fundido en el futuro (?)
		partida.SetActive(false);
	}
	
	/// <summary>
	/// Activa la ventana de refuerzos
	/// </summary>
	public void VentanaRefuerzos() {
		ventanaRefuerzos.SetActive(true);
	}
	
	/// <summary>
	/// Activa la ventana de ataque
	/// </summary>
	public void VentanaAtaque() {
		ventanaAtaque.SetActive(true);
	}
	
	/// <summary>
	/// Activa la ventana de movimiento
	/// </summary>
	public void VentanaMovimiento() {
		ventanaMovimiento.SetActive(true);
	}
	
	/// <summary>
	/// Activa la ventana de resultados de la partida
	/// </summary>
	public void VentanaFin() {
		ventanaFin.SetActive(true);
	}
	
}

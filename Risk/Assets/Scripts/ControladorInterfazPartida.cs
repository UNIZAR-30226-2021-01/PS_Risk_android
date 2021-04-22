using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorInterfazPartida : MonoBehaviour {
	[SerializeField]
	private GameObject fondoMenu, partida;
	[SerializeField]
	private GameObject ventanaFin, ventanaRefuerzos, ventanaAtaque, ventanaMovimiento;
	[SerializeField]
	private Image[] indicadorFase;

	private void OnEnable() {
		fondoMenu.SetActive(false); // Animacion de fundido en el futuro (?)
		partida.SetActive(true);
	}
	
	private void OnDisable() {
		try {
			fondoMenu.SetActive(true); // Animacion de fundido en el futuro (?)
			partida.SetActive(false);
		} catch {}
	}
	
	public void ActualizarInterfaz(){
		ActualizarFase(ControladorPartida.instance.datosPartida.fase-1); // Hacer más elegante
	}

	public void ActualizarFase(int fase){
		for (int i = 0; i < 3; i++) {
			indicadorFase[i].color = (i == fase ? Color.white : Color.black);
		}
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

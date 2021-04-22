using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ControladorInterfazPartida : MonoBehaviour {
	[SerializeField]
	private GameObject fondoMenu, partida;
	[SerializeField]
	private GameObject ventanaFin, ventanaRefuerzos, ventanaAtaque, ventanaMovimiento;
	[SerializeField]
	private Image[] indicadorFase;
	private int numeroTropas;
	[SerializeField]
	private TMP_InputField numeroRefuerzos;
	[SerializeField]
	private TMP_InputField numeroAtaque;
	[SerializeField]
	private TMP_InputField numeroMovimiento;

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
	
	/// <summary>
	/// Método invocado al pulsar la confirmación del refuerzo
	/// </summary>
	public void AceptarRefuerzo() {
		ControladorPartida.instance.Reforzar(numeroTropas);
	}
	
	/// <summary>
	/// Método invocado al pulsar la confirmación del ataque
	/// </summary>
	public void AceptarAtaque() {
		ControladorPartida.instance.Ataque(numeroTropas);
	}
	
	/// <summary>
	/// Método invocado al pulsar la confirmación del movimiento
	/// </summary>
	public void AceptarMovimiento() {
		ControladorPartida.instance.Movimiento(numeroTropas);
	}

	/// <summary>
	/// Actualiza todos los elementos de la interfaz
	/// </summary>
	public void ActualizarInterfaz(ClasesJSON.PartidaCompleta datosPartida) {
		ActualizarFase(datosPartida.fase-1); // Hacer más elegante
	}

	/// <summary>
	/// Actualiza el indicador de fase
	/// </summary>
	public void ActualizarFase(int fase){
		for (int i = 0; i < 3; i++) {
			indicadorFase[i].color = (i == fase ? Color.white : Color.black);
		}
	}
	
	/// <summary>
	/// Actualiza los textos de numeros de tropas en las ventanas de acciones
	/// </summary>
	public void ActualizarNumeroTropas() {
		numeroRefuerzos.text = numeroTropas.ToString();
		numeroAtaque.text = numeroTropas.ToString();
		numeroMovimiento.text = numeroTropas.ToString();
	}

	/// <summary>
	/// Aumenta en uno el numero de tropas
	/// </summary>
	public void AumentarTropas() {
		numeroTropas++;
		ActualizarNumeroTropas();
	}
	
	/// <summary>
	/// Disminuye en uno el numero de tropas
	/// </summary>
	public void DisminuirTropas() {
		if (numeroTropas > 0) {
			numeroTropas--;
			ActualizarNumeroTropas();
		}
	}
	
	/// <summary>
	/// Disminuye en uno el numero de tropas
	/// </summary>
	public void AsignarTropas(string s) {
		numeroTropas = int.Parse(s);
		ActualizarNumeroTropas();
	}
	
	/// <summary>
	/// Activa la ventana de refuerzos
	/// </summary>
	public void VentanaRefuerzos() {
		numeroTropas = 0;
		numeroRefuerzos.text = "0";
		ventanaRefuerzos.SetActive(true);
	}
	
	/// <summary>
	/// Activa la ventana de ataque
	/// </summary>
	public void VentanaAtaque() {
		numeroTropas = 0;
		numeroAtaque.text = "0";
		ventanaAtaque.SetActive(true);
	}
	
	/// <summary>
	/// Activa la ventana de movimiento
	/// </summary>
	public void VentanaMovimiento() {
		numeroTropas = 0;
		numeroMovimiento.text = "0";
		ventanaMovimiento.SetActive(true);
	}
	
	/// <summary>
	/// Activa la ventana de resultados de la partida
	/// </summary>
	public void VentanaFin() {
		ventanaFin.SetActive(true);
	}
	
}

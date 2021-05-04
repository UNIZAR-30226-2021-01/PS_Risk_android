using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class VentanaAccion : MonoBehaviour {
	[SerializeField]
	private TextMeshProUGUI textoTropasOrigen, textoTropasDestino, textoTropasSeleccionadas;
	[SerializeField]
	private Image tropaOrigen, tropaOrigenColor, tropaDestino, tropaDestinoColor;
	[SerializeField]
	private Slider sliderTropas;
	private int numeroTropasOrigen, numeroTropasDestino, tropasSeleccionadas, maxTropas;
	public bool ataque;
	
	public void ActualizarDatos(int idJugadorOrigen, int idJugadorDestino, int cantidadOrigen, int cantidadDestino, int limiteTropas) {
		tropasSeleccionadas = 1;
		numeroTropasOrigen = cantidadOrigen;
		numeroTropasDestino = cantidadDestino;
		if (tropaOrigen != null) {
			tropaOrigen.sprite = ControladorPrincipal.instance.aspectos[ControladorPartida.instance.datosPartida.jugadores.ToArray()[idJugadorOrigen].aspecto];
			tropaOrigenColor.sprite = ControladorPrincipal.instance.colorAspectos[ControladorPartida.instance.datosPartida.jugadores.ToArray()[idJugadorOrigen].aspecto];
			tropaOrigenColor.color = ControladorPrincipal.instance.coloresJugadores[idJugadorOrigen];
		}
		tropaDestino.sprite = ControladorPrincipal.instance.aspectos[ControladorPartida.instance.datosPartida.jugadores.ToArray()[idJugadorDestino].aspecto];
		tropaDestinoColor.sprite = ControladorPrincipal.instance.colorAspectos[ControladorPartida.instance.datosPartida.jugadores.ToArray()[idJugadorDestino].aspecto];
		tropaDestinoColor.color = ControladorPrincipal.instance.coloresJugadores[idJugadorDestino];
		sliderTropas.maxValue = limiteTropas;
		sliderTropas.minValue = (limiteTropas == 1 ? 0 : 1);
		ActualizarValores();
	}
	
	/// <summary>
	/// Aumenta en uno el numero de tropas
	/// </summary>
	public void IncrementarTropas() {
		if (tropasSeleccionadas < sliderTropas.maxValue) {
			tropasSeleccionadas++;
			ActualizarValores();
		}
	}
	
	/// <summary>
	/// Disminuye en uno el numero de tropas
	/// </summary>
	public void DisminuirTropas() {
		if (tropasSeleccionadas > 1) {
			tropasSeleccionadas--;
			ActualizarValores();
		}
	}
	
	/// <summary>
	/// Asigna en el numero de tropas
	/// </summary>
	public void AsignarTropas(float f) {
		int i = (int)f;
		tropasSeleccionadas = Mathf.Clamp(i, 1, (int)sliderTropas.maxValue);
		ActualizarValores();
	}

	private void ActualizarValores() {
		if (ataque) {
			textoTropasOrigen.text = tropasSeleccionadas.ToString();
			textoTropasDestino.text = numeroTropasDestino.ToString();
		} else {
			textoTropasDestino.text = (numeroTropasDestino + tropasSeleccionadas).ToString();
			textoTropasOrigen.text = (numeroTropasOrigen - tropasSeleccionadas).ToString();
		}
		textoTropasSeleccionadas.text = tropasSeleccionadas.ToString();
		sliderTropas.SetValueWithoutNotify(tropasSeleccionadas);
	}

	
	/// <summary>
	/// Método invocado al pulsar la confirmación del refuerzo
	/// </summary>
	public void AceptarRefuerzo() {
		ControladorPartida.instance.Reforzar(tropasSeleccionadas);
	}
	
	/// <summary>
	/// Método invocado al pulsar la confirmación del ataque
	/// </summary>
	public void AceptarAtaque() {
		ControladorPartida.instance.Ataque(tropasSeleccionadas);
	}
	
	/// <summary>
	/// Método invocado al pulsar la confirmación del movimiento
	/// </summary>
	public void AceptarMovimiento() {
		ControladorPartida.instance.Movimiento(tropasSeleccionadas);
	}
	
}

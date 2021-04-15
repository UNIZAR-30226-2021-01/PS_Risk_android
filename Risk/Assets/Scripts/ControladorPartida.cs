using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControladorPartida : MonoBehaviour {
	public static ControladorPartida instance;
	private ClasesJSON.PartidaCompleta datosPartida;

	private void Awake() {
		instance = this;
	}
	
	public void ActualizarDatosPartida(ClasesJSON.PartidaCompleta nuevaPartida) {
		datosPartida = nuevaPartida;
	}
}

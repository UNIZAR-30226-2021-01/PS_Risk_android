using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class ControladorPartida : MonoBehaviour {
	public static ControladorPartida instance;
	private ClasesJSON.PartidaCompleta datosPartida;
	private enum Fase {refuerzos, confirmacionRefuerzo, ataque, confirmacionAtaque, movimiento, confirmacionMovimiento, confirmacionFase};
	private Fase faseActual;
	private Mutex mtx;
	[SerializeField]
	private ControladorCamara cCamara;

	private void Awake() {
		instance = this;
	}
	
	public void ActualizarDatosPartida(ClasesJSON.PartidaCompleta nuevaPartida) {
		datosPartida = nuevaPartida;
	}
	
	public void GestionarMensajePartida(){
		mtx.WaitOne();
		switch (faseActual){
			case (Fase.refuerzos):
				break;
			case (Fase.ataque):
				break;
			case (Fase.movimiento):
				break;
		}
		mtx.ReleaseMutex();
	}
	
	/// <summary>Función llamada por cada territorio cuando este es seleccionado</summary>
	/// <param name="territorio">ID del territorio seleccionado</param>
	public void SeleccionTerritorio(int territorio) {
		// ¯\_(ツ)_/¯ TODO
	}
}

using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;

public class ControladorPartida : MonoBehaviour {
	private const string MSG_PASO_FASE = "{\"tipo\":\"Fase\"}";
	public static ControladorPartida instance;
	private ClasesJSON.PartidaCompleta datosPartida;
	private enum Fase {refuerzos, ataque, movimiento, confirmacionFase};
	private Fase faseActual;
	private bool esperandoConfirmacion, turnoJugador;
	private Mutex mtx;
	[SerializeField]
	private ControladorCamara cCamara;
	private GameObject ventanaFin;

	private void Awake() {
		instance = this;
	}
	
	public void ActualizarDatosPartida(ClasesJSON.PartidaCompleta nuevaPartida) {
		datosPartida = nuevaPartida;
	}
	
	public void PasarFase() {
		if (!esperandoConfirmacion) {
			ConexionWS.instance.EnviarWS(MSG_PASO_FASE);
		}
	}
	
	public void FinPartida(){
		ventanaFin.SetActive(true);
	}
	
	public void GestionarMensajePartida(string tipoMensaje, string mensaje) {
		mtx.WaitOne();
		if (tipoMensaje == "f") {
			// Avanzar fase en uno
			faseActual = (Fase)(((int)faseActual+1)%((int)Fase.confirmacionFase+1));
			esperandoConfirmacion = false;
			return;
		}
		switch (faseActual) {
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

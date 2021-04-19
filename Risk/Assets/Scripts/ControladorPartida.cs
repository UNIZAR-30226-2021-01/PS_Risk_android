// TODO: Recibir confirmaciones de refuerzo, ataques y movimiento (tambien desde ConexionWS.cs)
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;

public class ControladorPartida : MonoBehaviour {
	private const string MSG_PASO_FASE = "{\"tipo\":\"Fase\"}";
	private ClasesJSON.PartidaCompleta datosPartida;
	private enum Fase {refuerzos, ataque, movimiento};
	private Fase faseActual;
	private bool esperandoConfirmacion;
	private Mutex mtx;
	[SerializeField]
	private Mapa mapa;
	private GameObject ventanaFin, ventanaRefuerzos, ventanaAtaque, ventanaMovimiento;
	private int territorioOrigen = -1, territorioDestino = -1;
	public static ControladorPartida instance;
	public ClasesJSON.Jugador jugador = null;

	private void Awake() {
		instance = this;
	}
	
	/// <summary>
	/// Invocado cuando se recibe un mensaje de partida completa.
	/// Se actualiza el estado de la partida con el nuevo estado
	/// </summary>
	/// <param name="nuevaPartida"> Datos recibidos en el mensaje de partida completa </param>
	public void ActualizarDatosPartida(ClasesJSON.PartidaCompleta nuevaPartida) {
		foreach(ClasesJSON.Jugador j in nuevaPartida.jugadores){
			// TODO: Actualizar lista de jugadores	
			if(j.nombre == ControladorPrincipal.instance.usuarioRegistrado.nombre){
				jugador = j;
			}
		}
		if (jugador == null){
			ControladorPrincipal.instance.PantallaError("Jugador no encontrado en partida");
			ConexionWS.instance.CerrarConexionWebSocket();
			ControladorPrincipal.instance.AbrirPantalla("Principal");
		}
		datosPartida = nuevaPartida;
		mtx.WaitOne();
		faseActual = (Fase)(datosPartida.fase-1);
		mtx.ReleaseMutex();
		//mapa.ActualizarTerritorios(nuevaPartida.territorios);
		mapa.generarAleatorio();
	}

	/// <summary>Función llamada por cada territorio cuando este es seleccionado</summary>
	/// <param name="territorio">Territorio seleccionado</param>
	public void SeleccionTerritorio(Territorio territorio) {
		if(datosPartida.turnoActual != jugador.id || esperandoConfirmacion){
			// El jugador no debería poder interactuar
			return;
		}
		if(territorioOrigen == -1) {
			// Primera selección:
			// La primera seleccion debe ser siempre un territorio controlador por el usuario
			if(territorio.pertenenciaJugador != jugador.id) {
				Deseleccionar();
				return;
			}
			territorioOrigen = territorio.id;
			switch(faseActual){
				case (Fase.refuerzos):
					if(jugador.refuerzos <= 0){
						Deseleccionar();
						return;
					}
					ventanaRefuerzos.SetActive(true);
					break;
				case (Fase.ataque):
					mapa.MostrarAtaque(territorio.id);
					break;
				case (Fase.movimiento):
					mapa.MostrarMovimiento(territorio.id);
					break;
			}
		} else {
			// Segunda selección
			// La segunda seleccion debe ser siempre de un territorio no oculto
			if(territorio.Oculto || faseActual == Fase.refuerzos){
				Deseleccionar();
				return;
			}
			territorioDestino = territorio.id;
			switch(faseActual){
				case (Fase.ataque):
					ventanaAtaque.SetActive(true);
					break;
				case (Fase.movimiento):
					ventanaMovimiento.SetActive(true);
					break;
			}
		}
	}
	
	/// <summary>
	/// Invocado al pulsar el boton de cambio de fase.
	/// Envía al servidor un mensaje de cambio de fase.
	/// </summary>
	public async void PasarFase() {
		if (datosPartida.turnoActual == jugador.id) {
			await ConexionWS.instance.EnviarWS(MSG_PASO_FASE);
		}
	}
	
	/// <summary>
	/// Invocado cuando se recibe un mensaje de cambio de fase.
	/// Se aumenta la fase en uno.
	/// </summary>
	public void AvanzarFase(){
		mtx.WaitOne();
		faseActual = (Fase)(((int)faseActual+1)%4);
		mtx.ReleaseMutex();
		esperandoConfirmacion = false;
		Deseleccionar();
		// TODO: Actualizar la interfaz para mostrar el cambio de fase
	}
	
	/// <summary>Invocado cuando se termina la partida. Muestra los resultados de la partida</summary>
	public void FinPartida(){
		ventanaFin.SetActive(true);
	}
	
	/// <summary>Invocado cuando se pula confirmar desde la ventana de seleccion de tropas a reforzar.
	/// Notifica al seridor de una acción de refuerzo</summary>
	/// <param name ="tropas">Número de tropas a ser creadas en territorio origen seleccionado</param>
	public async void Reforzar(int tropas){
		if(faseActual != Fase.refuerzos || territorioOrigen == -1 || esperandoConfirmacion) {
			// Estados incorrectos, no deberían ocurrir pero so comprueban por si acaso
			return;
		}
		ClasesJSON.RefuerzoEnvio envio = new ClasesJSON.RefuerzoEnvio(territorioOrigen, tropas);
		string datos = JsonConvert.SerializeObject(envio);
		ControladorPrincipal.instance.PantallaCarga(true); // Sustituir por animacion personalizada (?)
		await ConexionWS.instance.EnviarWS(datos);
		esperandoConfirmacion = true;
		Deseleccionar();
	}
	
	/// <summary>Invocado cuando se pulsa confirmar desde la ventana de ataque.
	/// Notifica al seridor de una acción de ataque</summary>
	/// <param name ="tropas">Número de tropas a ser utilizadas en el ataque</param>
	public async void Ataque(int tropas){
		if(faseActual != Fase.ataque || territorioOrigen == -1 || territorioDestino == -1 || esperandoConfirmacion) {
			// Estados incorrectos, no deberían ocurrir pero so comprueban por si acaso
			return;
		}
		ClasesJSON.AtaqueEnvio envio = new ClasesJSON.AtaqueEnvio(territorioOrigen, territorioDestino, tropas);
		string datos = JsonConvert.SerializeObject(envio);
		ControladorPrincipal.instance.PantallaCarga(true); // Sustituir por animacion personalizada (?)
		await ConexionWS.instance.EnviarWS(datos);
		esperandoConfirmacion = true;
		Deseleccionar();
	}

	/// <summary>Invocado cuando se pulsa confirmar desde la ventana de movimiento.
	/// Notifica al seridor de una acción de ataque</summary>
	/// <param name ="tropas">Número de tropas a ser utilizadas en el ataque</param>
	public async void Movimiento(int tropas){
		if(faseActual != Fase.movimiento || territorioOrigen == -1 || territorioDestino == -1 || esperandoConfirmacion) {
			// Estados incorrectos, no deberían ocurrir pero so comprueban por si acaso
			return;
		}
		ClasesJSON.MovimientoEnvio envio = new ClasesJSON.MovimientoEnvio(territorioOrigen, territorioDestino, tropas);
		string datos = JsonConvert.SerializeObject(envio);
		ControladorPrincipal.instance.PantallaCarga(true); // Sustituir por animacion personalizada (?)
		await ConexionWS.instance.EnviarWS(datos);
		esperandoConfirmacion = true;
		Deseleccionar();
	}

	// Deselecciona todos los territorios seleccionados
	private void Deseleccionar(){
		territorioOrigen = -1;
		territorioDestino = -1;
		mapa.MostrarTodosTerritorios();
	}

}

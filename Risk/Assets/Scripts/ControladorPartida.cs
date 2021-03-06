// TODO: Recibir confirmaciones de refuerzo, ataques y movimiento (tambien desde ConexionWS.cs)
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;

public class ControladorPartida : MonoBehaviour {
	private const string MSG_PASO_FASE = "{\"tipo\":\"Fase\"}";
	private const int FASE_REFUERZOS = 0;
	private const int FASE_ATAQUE = 1;
	private const int FASE_MOVIMIENTO = 2;
	/// <summary> Contiene los datos mas recientes de la partida </summary>
	public ClasesJSON.PartidaCompleta datosPartida;
	private int faseActual;
	private Mutex mtx = new Mutex();
	// Propiedad de fase. Permite leer y escribir la fase asegurando exclusion mutua
	private int FaseActual {
		get {
			int toRet;
			mtx.WaitOne();
			toRet = faseActual;
			mtx.ReleaseMutex();
			return toRet;
		}
		set {
			mtx.WaitOne();
			faseActual = value;
			mtx.ReleaseMutex();
		}
	}
	private bool esperandoConfirmacion;
	[SerializeField]
	private Mapa mapa;
	[SerializeField]
	private ControladorInterfazPartida interfazPartida;
	private int territorioOrigen = -1, territorioDestino = -1;
	public static ControladorPartida instance;
	public int idJugador = -1; //ID del jugador del cliente
	private int idJugadorActual = -1; //ID del jugador que le toca jugar
	private int refuerzosRestantes; //Numero de refuerzos restantes por poner
	private bool haMovido = false;

	private void Awake() {
		instance = this;
		gameObject.SetActive(false);
	}

	private void Update() {
		if (Input.GetKeyDown(KeyCode.Space)) {
			print(FaseActual);
		}
	}
	
	/// <summary>
	/// Invocado cuando se entra a la partida.
	/// </summary>
	/// <param name="nuevaPartida"> Datos de partida completa </param>
	public void AsignarDatosPartida(ClasesJSON.PartidaCompleta nuevaPartida) {
		for(int i = 0; i < nuevaPartida.jugadores.Count; i++){
			ClasesJSON.Jugador j = nuevaPartida.jugadores[i];
			if (j.id == ControladorPrincipal.instance.usuarioRegistrado.id) {
				idJugador = i;
			}
		}
		if (idJugador == -1){
			ControladorPrincipal.instance.PantallaError("Jugador no encontrado en partida");
			ConexionWS.instance.CerrarConexionWebSocket();
			ControladorPrincipal.instance.AbrirPantalla("Principal");
		}
		ActualizarDatos(nuevaPartida);
		haMovido = false;
	}
	
	/// <summary>
	/// Invocado cuando se recibe un mensaje de partida completa.
	/// Se actualiza el estado de la partida con el nuevo estado
	/// </summary>
	/// <param name="nuevaPartida"> Datos recibidos en el mensaje de partida completa </param>
	public void ActualizarDatosPartida(ClasesJSON.PartidaCompleta nuevaPartida) {
		ControladorPrincipal.instance.PantallaCarga(false);
		ActualizarDatos(nuevaPartida);
	}
	
	private void ActualizarDatos(ClasesJSON.PartidaCompleta nuevaPartida) {
		datosPartida = nuevaPartida;
		FaseActual = datosPartida.fase-1;
		idJugadorActual = nuevaPartida.turnoJugador;
		refuerzosRestantes = nuevaPartida.jugadores[idJugador].refuerzos;
		interfazPartida.ActualizarInterfaz(nuevaPartida);
		interfazPartida.ActualizarRefuerzosRestantes(refuerzosRestantes); //Actualizar el indicador de refuerzos
		mapa.AsignarTerritorios(nuevaPartida.territorios);
		interfazPartida.ToggleRefuerzosRestantes(FaseActual == FASE_REFUERZOS && idJugador == idJugadorActual);
		interfazPartida.MostrarHistorialUltimaBatalla(false);
	}

	/// <summary>Funci??n llamada por cada territorio cuando este es seleccionado</summary>
	/// <param name="territorio">Territorio seleccionado</param>
	public void SeleccionTerritorio(Territorio territorio) {
		if(datosPartida.turnoJugador != idJugador || esperandoConfirmacion){
			// El jugador no deber??a poder interactuar
			return;
		}
		if(territorioOrigen == -1) {
			// Primera selecci??n:
			// La primera seleccion debe ser siempre un territorio controlador por el usuario
			if(territorio.pertenenciaJugador != idJugador) {
				Deseleccionar();
				return;
			}
			territorioOrigen = territorio.id;
			switch(FaseActual){
				case (FASE_REFUERZOS):
					if(refuerzosRestantes <= 0){
						Deseleccionar();
						ControladorPrincipal.instance.PantallaInfo("No quedan tropas que colocar");
						return;
					}
					interfazPartida.VentanaRefuerzos(refuerzosRestantes, territorio);
					break;
				case (FASE_ATAQUE):
					if (territorio.numeroTropas <= 1) {
						Deseleccionar();
						ControladorPrincipal.instance.PantallaInfo("No se puede atacar desde territorio con una sola tropa");
						return;
					}
					mapa.MostrarAtaque(territorio.id);
					break;
				case (FASE_MOVIMIENTO):
					if (haMovido) {
						Deseleccionar();
						ControladorPrincipal.instance.PantallaInfo("Solo se puede realizar un movimiento por turno");
						return;
					}
					if (territorio.numeroTropas <= 1) {
						Deseleccionar();
						ControladorPrincipal.instance.PantallaInfo("No se puede mover desde territorio con una sola tropa");
						return;
					}
					mapa.MostrarMovimiento(territorio.id);
					break;
			}
		} else {
			// Segunda selecci??n
			// La segunda seleccion debe ser siempre de un territorio no oculto
			if(territorio.Oculto || FaseActual == FASE_REFUERZOS){
				Deseleccionar();
				return;
			}
			territorioDestino = territorio.id;
			switch(FaseActual){
				case (FASE_ATAQUE):
					if(territorio.pertenenciaJugador != idJugador) {
						interfazPartida.VentanaAtaque(mapa.territorios[territorioOrigen], territorio);
					} else {
						territorioDestino = -1;
					}
					break;
				case (FASE_MOVIMIENTO):
					if(territorio.pertenenciaJugador == idJugador) {
						interfazPartida.VentanaMovimiento(mapa.territorios[territorioOrigen], territorio);
					} else {
						territorioDestino = -1;
					}
					break;
			}
		}
	}

	/// <summary>
	/// Invocado al pulsar el boton de cambio de fase.
	/// Env??a al servidor un mensaje de cambio de fase.
	/// </summary>
	public async void PasarFase() {
		if (datosPartida.turnoJugador == idJugador) {
			ControladorPrincipal.instance.PantallaCarga(true);
			await ConexionWS.instance.EnviarWS(MSG_PASO_FASE);
		}
	}
	
	/// <summary>Invocado cuando se pulsa confirmar desde la ventana de seleccion de tropas a reforzar.
	/// Notifica al seridor de una acci??n de refuerzo</summary>
	/// <param name ="tropas">N??mero de tropas a ser creadas en territorio origen seleccionado</param>
	public async void Reforzar(int tropas){
		if(FaseActual != FASE_REFUERZOS || territorioOrigen == -1 || esperandoConfirmacion) {
			// Estados incorrectos, no deber??an ocurrir pero so comprueban por si acaso
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
	/// Notifica al seridor de una acci??n de ataque</summary>
	/// <param name ="tropas">N??mero de tropas a ser utilizadas en el ataque</param>
	public async void Ataque(int tropas){
		if(FaseActual != FASE_ATAQUE || territorioOrigen == -1 || territorioDestino == -1 || esperandoConfirmacion) {
			// Estados incorrectos, no deber??an ocurrir pero so comprueban por si acaso
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
	/// Notifica al seridor de una acci??n de ataque</summary>
	/// <param name ="tropas">N??mero de tropas a ser utilizadas en el ataque</param>
	public async void Movimiento(int tropas){
		if(FaseActual != FASE_MOVIMIENTO || territorioOrigen == -1 || territorioDestino == -1 || esperandoConfirmacion) {
			// Estados incorrectos, no deber??an ocurrir pero so comprueban por si acaso
			return;
		}
		ClasesJSON.MovimientoEnvio envio = new ClasesJSON.MovimientoEnvio(territorioOrigen, territorioDestino, tropas);
		string datos = JsonConvert.SerializeObject(envio);
		ControladorPrincipal.instance.PantallaCarga(true); // Sustituir por animacion personalizada (?)
		await ConexionWS.instance.EnviarWS(datos);
		esperandoConfirmacion = true;
		Deseleccionar();
	}
	
	/// <summary>Invocado cuando se termina la partida. Muestra los resultados de la partida</summary>
	public void FinPartida(ClasesJSON.FinPartida datos){
		interfazPartida.VentanaFin(datos);
	}
	
	/// <summary>
	/// Invocado cuando se recibe un mensaje de cambio de fase.
	/// Se aumenta la fase en uno.
	/// </summary>
	public void ConfirmacionFase(){
		FaseActual = (faseActual+1)%3;
		esperandoConfirmacion = false;
		Deseleccionar();
		interfazPartida.ActualizarFase(FaseActual);
		interfazPartida.ToggleRefuerzosRestantes(FaseActual == FASE_REFUERZOS && idJugador == idJugadorActual);
		ControladorPrincipal.instance.PantallaCarga(false);
		interfazPartida.MostrarHistorialUltimaBatalla(false);
		haMovido = false;
	}
	
	/// <summary>
	/// Invocado cuando se recibe un mensaje de confirmaci??n de refuerzos
	/// Se actualiza el territorio modificado
	/// </summary>
	public void ConfirmacionRefuerzos(ClasesJSON.ConfirmacionRefuerzos refuerzos){
		refuerzosRestantes -= refuerzos.territorio.tropas-mapa.territorios[refuerzos.territorio.id].numeroTropas;
		interfazPartida.ActualizarRefuerzosRestantes(refuerzosRestantes);
		mapa.ActualizarTerritorio(refuerzos.territorio);
		esperandoConfirmacion = false;
		ControladorPrincipal.instance.PantallaCarga(false);
	}

	/// <summary>
	/// Invocado cuando se recibe un mensaje de confirmaci??n de ataque
	/// Se actualizan los territorios modificados y se muestran los dados resultantes
	/// </summary>
	public void ConfirmacionAtaque(ClasesJSON.ConfirmacionAtaque ataque){
		interfazPartida.ActualizarHistorialUltimaBatalla(ataque, mapa.territorios[ataque.territorioDestino.id]); //Mostrar Dados
		mapa.ActualizarTerritorio(ataque.territorioOrigen);
		mapa.ActualizarTerritorio(ataque.territorioDestino);
		esperandoConfirmacion = false;
		ControladorPrincipal.instance.PantallaCarga(false);
		interfazPartida.MostrarHistorialUltimaBatalla(true);
	}

	/// <summary>
	/// Invocado cuando se recibe un mensaje de confirmaci??n de movimiento
	/// Se actualizan los territorios modificados
	/// </summary>
	public void ConfirmacionMovimiento(ClasesJSON.ConfirmacionMovimiento movimiento){
		mapa.ActualizarTerritorio(movimiento.territorioOrigen);
		mapa.ActualizarTerritorio(movimiento.territorioDestino);
		esperandoConfirmacion = false;
		ControladorPrincipal.instance.PantallaCarga(false);
		haMovido = true;
	}

	/// <summary>
	/// Invocado cuando se selecciona en un lugar inv??lido al tener un territorio seleccionado
	/// Muestra todos los territorios y borra los seleccionados guardados
	/// </summary>
	public void Deseleccionar(){
		territorioOrigen = -1;
		territorioDestino = -1;
		mapa.MostrarTodosTerritorios();
	}
	
	/// <summary>
	/// Invocado cuando se recibe un error desde el servidor
	/// Si se estaba esperando una confirmaci??n significa que la operaci??n ha fallado
	/// </summary>
	public void Error(){
		esperandoConfirmacion = false;
	}

	/// <summary>
	/// Invocado cuando se recibe un error desde el servidor
	/// Si se estaba esperando una confirmaci??n significa que la operaci??n ha fallado
	/// </summary>
	public void SalirPartida(){
		ConexionWS.instance.CerrarConexionWebSocket();
	}

}

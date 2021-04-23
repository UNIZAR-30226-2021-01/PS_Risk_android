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
	private ClasesJSON.Jugador jugador = null;
	public int idJugador = -1;

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
	/// Invocado cuando se recibe un mensaje de partida completa.
	/// Se actualiza el estado de la partida con el nuevo estado
	/// </summary>
	/// <param name="nuevaPartida"> Datos recibidos en el mensaje de partida completa </param>
	public void ActualizarDatosPartida(ClasesJSON.PartidaCompleta nuevaPartida) {
		for(int i = 0; i < nuevaPartida.jugadores.Count; i++){
			// TODO: Actualizar lista de jugadores	
			ClasesJSON.Jugador j = nuevaPartida.jugadores[i];
			if (j.id == ControladorPrincipal.instance.usuarioRegistrado.id) {
				jugador = j;
				idJugador = i;
			}
		}
		if (jugador == null){
			ControladorPrincipal.instance.PantallaError("Jugador no encontrado en partida");
			ConexionWS.instance.CerrarConexionWebSocket();
			ControladorPrincipal.instance.AbrirPantalla("Principal");
		}
		datosPartida = nuevaPartida;
		FaseActual = datosPartida.fase-1;
		interfazPartida.ActualizarInterfaz(nuevaPartida);
		print("TurnoJugador: " + datosPartida.turnoJugador + " (" + datosPartida.jugadores.ToArray()[datosPartida.turnoJugador].nombre + "). Jugador.id: " + idJugador + "(" + datosPartida.jugadores.ToArray()[idJugador].nombre + "). Esperando confirmación: " + esperandoConfirmacion);
		mapa.ActualizarTerritorios(nuevaPartida.territorios);
	}

	/// <summary>Función llamada por cada territorio cuando este es seleccionado</summary>
	/// <param name="territorio">Territorio seleccionado</param>
	public void SeleccionTerritorio(Territorio territorio) {
		print("Seleccionado territorio: {turnoJugador: " + datosPartida.turnoJugador +
			", esperandoConfirmacion: " + esperandoConfirmacion +
			", territorioOrigen: " + territorioOrigen +
			", territorio.PetenenciaJugador" + territorio.pertenenciaJugador +
			", idJugador: " + idJugador +
			", FaseActual: " + FaseActual +
			", jugador.refuerzos: " + jugador.refuerzos + "}");
		if(datosPartida.turnoJugador != idJugador || esperandoConfirmacion){
			// El jugador no debería poder interactuar
			return;
		}
		if(territorioOrigen == -1) {
			// Primera selección:
			// La primera seleccion debe ser siempre un territorio controlador por el usuario
			if(territorio.pertenenciaJugador != idJugador) {
				Deseleccionar();
				return;
			}
			territorioOrigen = territorio.id;
			switch(FaseActual){
				case (FASE_REFUERZOS):
					if(jugador.refuerzos <= 0){
						Deseleccionar();
						return;
					}
					interfazPartida.VentanaRefuerzos();
					break;
				case (FASE_ATAQUE):
					mapa.MostrarAtaque(territorio.id);
					break;
				case (FASE_MOVIMIENTO):
					mapa.MostrarMovimiento(territorio.id);
					break;
			}
		} else {
			// Segunda selección
			// La segunda seleccion debe ser siempre de un territorio no oculto
			if(territorio.Oculto || FaseActual == FASE_REFUERZOS){
				Deseleccionar();
				return;
			}
			territorioDestino = territorio.id;
			switch(FaseActual){
				case (FASE_ATAQUE):
					interfazPartida.VentanaAtaque();
					break;
				case (FASE_MOVIMIENTO):
					interfazPartida.VentanaMovimiento();
					break;
			}
		}
	}

	/// <summary>
	/// Invocado al pulsar el boton de cambio de fase.
	/// Envía al servidor un mensaje de cambio de fase.
	/// </summary>
	public async void PasarFase() {
		if (datosPartida.turnoJugador == idJugador) {
			await ConexionWS.instance.EnviarWS(MSG_PASO_FASE);
		}
	}
	
	/// <summary>Invocado cuando se pulsa confirmar desde la ventana de seleccion de tropas a reforzar.
	/// Notifica al seridor de una acción de refuerzo</summary>
	/// <param name ="tropas">Número de tropas a ser creadas en territorio origen seleccionado</param>
	public async void Reforzar(int tropas){
		if(FaseActual != FASE_REFUERZOS || territorioOrigen == -1 || esperandoConfirmacion) {
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
		if(FaseActual != FASE_ATAQUE || territorioOrigen == -1 || territorioDestino == -1 || esperandoConfirmacion) {
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
		if(FaseActual != FASE_MOVIMIENTO || territorioOrigen == -1 || territorioDestino == -1 || esperandoConfirmacion) {
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
	
	/// <summary>Invocado cuando se termina la partida. Muestra los resultados de la partida</summary>
	public void FinPartida(){
		interfazPartida.VentanaFin();
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
	}
	
	/// <summary>
	/// Invocado cuando se recibe un mensaje de confirmación de refuerzos
	/// Se actualiza el territorio modificado
	/// </summary>
	public void ConfirmacionRefuerzos(ClasesJSON.ConfirmacionRefuerzos refuerzos){
		mapa.ActualizarTerritorio(refuerzos.territorio);
		esperandoConfirmacion = false;
		ControladorPrincipal.instance.PantallaCarga(false);
	}

	/// <summary>
	/// Invocado cuando se recibe un mensaje de confirmación de ataque
	/// Se actualizan los territorios modificados y se muestran los dados resultantes
	/// </summary>
	public void ConfirmacionAtaque(ClasesJSON.ConfirmacionAtaque ataque){
		mapa.ActualizarTerritorio(ataque.territorioOrigen);
		mapa.ActualizarTerritorio(ataque.territorioDestino);
		esperandoConfirmacion = false;
		ControladorPrincipal.instance.PantallaCarga(false);
	}

	/// <summary>
	/// Invocado cuando se recibe un mensaje de confirmación de movimiento
	/// Se actualizan los territorios modificados
	/// </summary>
	public void ConfirmacionMovimiento(ClasesJSON.ConfirmacionMovimiento movimiento){
		mapa.ActualizarTerritorio(movimiento.territorioOrigen);
		mapa.ActualizarTerritorio(movimiento.territorioDestino);
		esperandoConfirmacion = false;
		ControladorPrincipal.instance.PantallaCarga(false);
	}

	/// <summary>
	/// Invocado cuando se selecciona en un lugar inválido al tener un territorio seleccionado
	/// Muestra todos los territorios y borra los seleccionados guardados
	/// </summary>
	public void Deseleccionar(){
		territorioOrigen = -1;
		territorioDestino = -1;
		mapa.MostrarTodosTerritorios();
	}
	
	/// <summary>
	/// Invocado cuando se recibe un error desde el servidor
	/// Si se estaba esperando una confirmación significa que la operación ha fallado
	/// </summary>
	public void Error(){
		esperandoConfirmacion = false;
	}

	/// <summary>
	/// Invocado cuando se recibe un error desde el servidor
	/// Si se estaba esperando una confirmación significa que la operación ha fallado
	/// </summary>
	public void SalirPartida(){
		ConexionWS.instance.CerrarConexionWebSocket();
	}

}

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
	
	// GameObjects que muestran la lista de jugadores
	[SerializeField]
	private Image[] listaIconos;
	[SerializeField]
	private TextMeshProUGUI[] listaTextos;
	[SerializeField]
	private Image[] listaOverlaysColores;

	// GameObjects que muestran el jugador actual
	[SerializeField]
	private Image jugadorActualIcono, jugadorActualColor;
	[SerializeField]
	private TextMeshProUGUI jugadorActualTexto;

	private void OnEnable() {
		fondoMenu.SetActive(false); // Animacion de fundido en el futuro (?)
		partida.SetActive(true);
	}
	
	private void OnDisable() {
		try {
			fondoMenu.SetActive(true); // Animacion de fundido en el futuro (?)
			partida.SetActive(false);
			ControladorPartida.instance.SalirPartida();
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
		ActualizarLista(datosPartida);
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
	
	/// <summary>Actualiza la lista de jugadores a partir de un JSON de partida</summary>
	/// <param name="datosSala">Datos de la partida, los cuales incluye los jugadores</param>
	public void ActualizarLista(ClasesJSON.PartidaCompleta datosSala) {

		int nJugadores = datosSala.jugadores.Count;

		//Iterar por los IDs de partida que se usan
		for(int id = 0; id < nJugadores; id++) {
			ActualizarJugador(id,datosSala.jugadores[id]);
			if(datosSala.turnoJugador == id) //Actualizar el indicador del jugador actual
				ActualizarJugadorActual(id, datosSala.jugadores[id]);
		}

		//Desactivar el resto de gameobjects, los cuales no estan mostrando ningun jugador
		for(int i = nJugadores; i < 6; i++)
			listaIconos[i].gameObject.SetActive(false);
	}
	
	/// <summary>Actualiza los datos mostrados en la lista para un solo jugador</summary>
	/// <param name="id">ID de partida del jguador a actualizar</param>
	/// <param name="datosJugador">Nuevos datos</param>
	private void ActualizarJugador(int id, ClasesJSON.Jugador datosJugador) {
		listaOverlaysColores[id].color = ControladorPrincipal.instance.coloresJugadores[id]; //Colorear bandera
		listaTextos[id].text = datosJugador.nombre; //Mostrar nombre
		listaIconos[id].sprite = ControladorPrincipal.instance.iconos[datosJugador.icono]; //Mostrar icono
	}

	//Igual a ActualizarJugador(), pero para el jugador actual
	private void ActualizarJugadorActual(int id, ClasesJSON.Jugador datosJugador) {
		jugadorActualColor.color = ControladorPrincipal.instance.coloresJugadores[id]; //Colorear bandera
		jugadorActualTexto.text = datosJugador.nombre; //Mostrar nombre
		jugadorActualIcono.sprite = ControladorPrincipal.instance.iconos[datosJugador.icono]; //Mostrar icono
	}

	/// <summary>Muestra y esconde la lista de jugadores. Si se esconde, se muestra el jugador actual</summary>
	/// <param name="mostrar">Si 'true', se muestra la lista y se esconde el jugador actual. Viceversa para 'false'</param>
	public void MostrarListaJugadores(bool mostrar) {
		listaIconos[0].transform.parent.gameObject.SetActive(mostrar);
		jugadorActualIcono.gameObject.SetActive(!mostrar);
	}
}

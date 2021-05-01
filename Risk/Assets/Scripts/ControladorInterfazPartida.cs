using System;
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
	private Sprite[] spriteIndicadorFase;
	[SerializeField]
	private Image[] indicadorFase;
	private int numeroTropas;
	[SerializeField]
	private TMP_InputField numeroRefuerzos;
	[SerializeField]
	private TMP_InputField numeroAtaque;
	[SerializeField]
	private TMP_InputField numeroMovimiento;

	[SerializeField]
	private TextMeshProUGUI textoRefuerzosRestantes; //Campo de texto con el numero de refuerzos restantes por poner
	[SerializeField]
	private Animator animatorRefuerzosRestantes; //Animación para el indicador de refuerzos restantes
	[SerializeField]
	private JugadorPartida[] listaJugadores;
	[SerializeField]
	private Animator animatorJugadores; //Animator de lista de jugadores
	[SerializeField]
	private TextMeshProUGUI textoJugadorActual, tiempoActual;
	private DateTime inicioTurno;
	[SerializeField]
	private TextMeshProUGUI textoFin;

	//Historial, gameobjecst que muestran los resultados de la ultima batalla
	[SerializeField]
	private GameObject historialPanel; //Panel del historial de la ultima batalla
	[SerializeField]
	private Image[] historialDados; //Array de tamaño 5 con todos los dados que se pueden mostrar en el historial. Dos primeros son los de defensa
	[SerializeField]
	private GameObject[] historialDadosOutline; //Outline para cada dado en caso de que este resulte ganador, array de 4 ya que el último nunca puede iluminarse
	[SerializeField]
	private Image[] historialTropas; //Tropas en el historial, primero defensor
	[SerializeField]
	private Image[] historialTropasColor; //Overlay de color para las tropas en el historial, primero defensor
	[SerializeField]
	private TextMeshProUGUI[] historialTextos; //Textos del historial, 0 para cantidad defensor, 1 para cambio defensor, 2 para cantidad atacante, 3 para cambio atacante

	[SerializeField]
	private Sprite[] dadosRojos; //Array de sprites de los dados rojos (Ataque)
	[SerializeField]
	private Sprite[] dadosBlancos; //Array de sprites de los dados blancos (Defensa)
	[SerializeField]
	private Sprite dadosOutline; //Outline para indicar que dado ha permitido derrotar a otro dado

	[SerializeField]
	private Animator animatorUltimaBatalla; //Aminator del menu de la última batalla
	
	private bool listaJugadoresAbierto = false; //Indica si la lista de jugadores esta abierto o cerrado
	private int limiteTropas;

	private void ActualizarTiempo() {
		if (gameObject.activeInHierarchy) {
			tiempoActual.text = (DateTime.UtcNow - inicioTurno).ToString(@"hh\:mm\:ss");
		}
	}

	private void Start() {
		InvokeRepeating("ActualizarTiempo", 0, 1);
	}

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
		inicioTurno = DateTime.Parse(datosPartida.ultimoTurno, null, System.Globalization.DateTimeStyles.RoundtripKind).AddMinutes(datosPartida.tiempoTurno);
	}

	/// <summary>
	/// Actualiza el indicador de fase
	/// </summary>
	public void ActualizarFase(int fase){
		for (int i = 0; i < 3; i++) {
			indicadorFase[i].sprite = spriteIndicadorFase[(i == fase ? 0 : 1)];
			indicadorFase[i].color = (i == fase ? new Color(0.3f, 1, 0.3f, 1) : new Color(0.75f, 0.75f, 0.75f, 1));
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
		if (numeroTropas < limiteTropas) {
			numeroTropas++;
			ActualizarNumeroTropas();
		}
	}
	
	/// <summary>
	/// Disminuye en uno el numero de tropas
	/// </summary>
	public void DisminuirTropas() {
		if (numeroTropas > 1) {
			numeroTropas--;
			ActualizarNumeroTropas();
		}
	}
	
	/// <summary>
	/// Disminuye en uno el numero de tropas
	/// </summary>
	public void AsignarTropas(string s) {
		int prevNum = numeroTropas;
		int newNum = Mathf.Clamp(int.Parse(s), 1, limiteTropas);
		if (prevNum != newNum) {
			numeroTropas = newNum;
			ActualizarNumeroTropas();
		}
	}
	
	/// <summary>
	/// Activa la ventana de refuerzos
	/// </summary>
	public void VentanaRefuerzos(int refuerzos) {
		numeroTropas = 1;
		numeroRefuerzos.text = "1";
		limiteTropas = refuerzos;
		ventanaRefuerzos.SetActive(true);
	}
	
	/// <summary>
	/// Activa la ventana de ataque
	/// </summary>
	public void VentanaAtaque(int atacantes) {
		numeroTropas = 1;
		numeroAtaque.text = "1";
		limiteTropas = atacantes;
		ventanaAtaque.SetActive(true);
	}
	
	/// <summary>
	/// Activa la ventana de movimiento
	/// </summary>
	public void VentanaMovimiento(int tropas) {
		numeroTropas = 1;
		numeroMovimiento.text = "1";
		limiteTropas = tropas;
		ventanaMovimiento.SetActive(true);
	}
	
	/// <summary>
	/// Activa la ventana de resultados de la partida
	/// </summary>
	public void VentanaFin(ClasesJSON.FinPartida datos) {
		textoFin.text = "El ganador de la partida es <color=#" + ColorUtility.ToHtmlStringRGB(ControladorPrincipal.instance.coloresJugadores[datos.idGanador]) + 
				">" + ControladorPartida.instance.datosPartida.jugadores[datos.idGanador].nombre + "</color>";
		if (datos.idGanador == ControladorPartida.instance.idJugador) {
			textoFin.text += "\n Has ganado " + datos.riskos.ToString() + " riskos";
		}
		ventanaFin.SetActive(true);
	}
	
	/// <summary>Actualiza la lista de jugadores a partir de un JSON de partida</summary>
	/// <param name="datosSala">Datos de la partida, los cuales incluye los jugadores</param>
	public void ActualizarLista(ClasesJSON.PartidaCompleta datosSala) {

		int nJugadores = datosSala.jugadores.Count;

		//Iterar por los IDs de partida que se usan
		for(int id = 0; id < nJugadores; id++) {
			ActualizarJugador(id,datosSala.jugadores[id]);
			if(datosSala.turnoJugador == id) { //Actualizar el texto que indica el jugador actual
				textoJugadorActual.text = "Turno de: <color=#" + ColorUtility.ToHtmlStringRGB(ControladorPrincipal.instance.coloresJugadores[id]) + 
				">" + datosSala.jugadores[id].nombre + "</color>";
			}
		}

		//Desactivar el resto de gameobjects, los cuales no estan mostrando ningun jugador
		for(int i = nJugadores; i < 6; i++) {
			listaJugadores[i].gameObject.SetActive(false);
		}
	}
	
	/// <summary>Actualiza los datos mostrados en la lista para un solo jugador</summary>
	/// <param name="id">ID de partida del jugador a actualizar</param>
	/// <param name="datosJugador">Nuevos datos</param>
	private void ActualizarJugador(int id, ClasesJSON.Jugador datosJugador) {
		JugadorPartida jugador = listaJugadores[id];
		jugador.icono.sprite = ControladorPrincipal.instance.iconos[datosJugador.icono];
		jugador.banderaColor.color = ControladorPrincipal.instance.coloresJugadores[id];
		jugador.nombre.text = datosJugador.nombre;
	}

	/// <summary>Muestra y esconde la lista de jugadores</summary>
	public void ToggleListaJugadores() {
		listaJugadoresAbierto = !listaJugadoresAbierto;
		animatorJugadores.SetBool("Abierto", listaJugadoresAbierto);
	}

	/// <summary>Cambia el numero de refuerzos restantes a poner</summary>
	/// <param name="numero">Nueva cantidad a mostrar</param>
	public void ActualizarRefuerzosRestantes(int numero) {
		textoRefuerzosRestantes.text = numero.ToString();
	}

	/// <summary>Mostrar o esconder el panel con los refuerzos restantes</summary>
	/// <param name="mostrar">Si 'true', mostrar</param>
	public void ToggleRefuerzosRestantes(bool mostrar) {
		animatorRefuerzosRestantes.SetBool("Abierto", mostrar);
	}
	
	private IEnumerator esperarAnimadorRefuerzos(bool mostrar){
		yield return new WaitForEndOfFrame();
		animatorRefuerzosRestantes.SetBool("Abierto", mostrar);
	}

	/// <summary>Actualiza y permite ver la pantalla de la última batalla</summary>
	/// <param name="ataque">Información con el último ataque</param>
	public void ActualizarHistorialUltimaBatalla(ClasesJSON.ConfirmacionAtaque ataque) {
		ClasesJSON.Territorio origen = ataque.territorioOrigen;
		ClasesJSON.Territorio destino = ataque.territorioDestino;

		//Indicar cuantas tropas quedan
		historialTextos[0].text = destino.tropas.ToString();
		historialTextos[1].text = origen.tropas.ToString();

		//Desactivar todos los outlines de los dados
		foreach (var o in historialDadosOutline) {
			o.SetActive(false);
		}

		//Desactivar todos los dados
		foreach (var o in historialDados) {
			o.gameObject.SetActive(false);
		}

		//Obtener cuantos dados se comparan, puede ser 1 o 2
		int minDados = Mathf.Min(ataque.dadosOrigen.Length, ataque.dadosDestino.Length);

		//Activar los outlines y obtener perdidas de soldados
		for (int i = 0; i < minDados; i++) {
			bool destinoMayor = ataque.dadosDestino[i] >= ataque.dadosOrigen[i];
			historialDadosOutline[i].SetActive(destinoMayor); //Defensa
			historialDadosOutline[i + 2].SetActive(!destinoMayor); //Ataque
		}

		//Actualizar dados defensa
		for (int i = 0; i < ataque.dadosDestino.Length; i++) {
			historialDados[i].gameObject.SetActive(true);
			historialDados[i].overrideSprite = dadosBlancos[ataque.dadosDestino[i] - 1];
		}
		//Actualizar dados ataque
		for (int i = 0; i < ataque.dadosOrigen.Length; i++) {
			historialDados[i + 2].gameObject.SetActive(true);
			historialDados[i + 2].overrideSprite = dadosRojos[ataque.dadosOrigen[i] - 1];
		}

		//Tropas
		int jugadorAtacante = ataque.territorioOrigen.jugador;
		int jugadorDefensor = ataque.territorioDestino.jugador;

		int aspectoAtacante = ControladorPartida.instance.datosPartida.jugadores[jugadorAtacante].aspecto;
		int aspectoDefensor = ControladorPartida.instance.datosPartida.jugadores[jugadorDefensor].aspecto;

		historialTropas[1].overrideSprite = ControladorPrincipal.instance.aspectos[aspectoAtacante]; //Aspecto
		historialTropas[0].overrideSprite = ControladorPrincipal.instance.aspectos[aspectoDefensor];

		historialTropasColor[1].overrideSprite = ControladorPrincipal.instance.colorAspectos[aspectoAtacante]; //Aspecto
		historialTropasColor[0].overrideSprite = ControladorPrincipal.instance.colorAspectos[aspectoDefensor];
		historialTropasColor[1].color = ControladorPrincipal.instance.coloresJugadores[jugadorAtacante];
		historialTropasColor[0].color = ControladorPrincipal.instance.coloresJugadores[jugadorDefensor];

	}

	/// <summary>Abre y cierra la ventana con los datos de la ultima batalla</summary>
	public void ToggleHistorialUltimaBatalla() {
		animatorUltimaBatalla.SetBool("Abierto", !animatorUltimaBatalla.GetBool("Abierto"));
	}

	/// <summary>Muestra y esconde la ventana con la información de la última batalla</summary>
	/// <param name="mostrar">Si 'true', la ventana se mostrara, y viceversa</param>
	public void MostrarHistorialUltimaBatalla(bool mostrar) {
		historialPanel.SetActive(mostrar);
	}
}

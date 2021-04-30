using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Territorio : MonoBehaviour {

	private const float OPACIDAD_OVERLAY_TERRITORIO = 0.75f;
	private const int FASE_REFUERZOS = 0;
	private const int FASE_ATAQUE = 1;
	private const int FASE_MOVIMIENTO = 2;
	[SerializeField]
	private SpriteRenderer aspectoTropa, overlayTropa, overlayTerritorio;
	[SerializeField]
	private GameObject indicadorTu, indicadorSeleccionado;
	private ClasesJSON.Territorio datosAnteriores;
	[SerializeField]
	private TextMeshPro textoNumeroTropas;
	[SerializeField]
	private IndicadorCambioTropas textoCambio;
	[SerializeField]
	/// <summary>ID del territorio, iniciar en el editor</summary>
	public int id;
	/// <summary>Conexiones con otros territorios, inicializar en el editor</summary>
	public Territorio[] conexiones;
	/// <summary>ID del jugador (en cuanto a orden de la partida) del que pertence este territorio</summary>
	public int pertenenciaJugador, numeroTropas;
	private bool oculto = false, seleccionado = false;
	/// <summary>
	/// Property publico para asignar el estado de oculto al territorio.
	///	Además de actualizar el valor interno de oculto actualiza su representación cuando es asignado.
	///	</summary> 
	public bool Oculto {
		get { return oculto; }
		set {
			overlayTropa.gameObject.SetActive(!value);
			aspectoTropa.gameObject.SetActive(!value);
			overlayTerritorio.color = (value ? Color.black : ControladorPrincipal.instance.coloresJugadores[datosAnteriores.jugador])
				* new Color(1,1,1,OPACIDAD_OVERLAY_TERRITORIO);
			oculto = value;
		}
	}
	/// <summary>
	///	</summary> 
	public bool Seleccionado {
		get { return seleccionado; }
		set {
			indicadorSeleccionado.SetActive(value);
			seleccionado = value;
		}
	}
	
	/// <summary> Actualiza los datos del territorio con los datos proporciondos </summary>
	/// <param name="nuevosDatos"> Nuevos datos a ser almacenados en este territorio </param>
	public void AsignarTerritorio(ClasesJSON.Territorio nuevosDatos) {
		if (datosAnteriores == null) {
			id = nuevosDatos.id;
			datosAnteriores = nuevosDatos;
		}
		aspectoTropa.sprite = ControladorPrincipal.instance.aspectos[ControladorPartida.instance.datosPartida.jugadores[nuevosDatos.jugador].aspecto];
		overlayTropa.sprite = ControladorPrincipal.instance.colorAspectos[ControladorPartida.instance.datosPartida.jugadores[nuevosDatos.jugador].aspecto];
		overlayTropa.color = ControladorPrincipal.instance.coloresJugadores[nuevosDatos.jugador];
		overlayTerritorio.color = (oculto ? Color.black : ControladorPrincipal.instance.coloresJugadores[nuevosDatos.jugador])
			* new Color(1,1,1,OPACIDAD_OVERLAY_TERRITORIO);
		numeroTropas = nuevosDatos.tropas;
		textoNumeroTropas.text = numeroTropas.ToString();
		indicadorTu.SetActive(ControladorPartida.instance.idJugador == nuevosDatos.jugador);
		pertenenciaJugador = nuevosDatos.jugador;
		datosAnteriores = nuevosDatos;
	}
	
	/// <summary> Si los datos recibidos son diferentes a los actuales muestra los cambios mediante una animación </summary>
	/// <param name="nuevosDatos"> Nuevos datos a ser almacenados en este territorio </param>
	public void ActualizarTerritorio(ClasesJSON.Territorio nuevosDatos) {
		if (datosAnteriores == null) {
			id = nuevosDatos.id;
			datosAnteriores = nuevosDatos;
		} else if (datosAnteriores.Equals(nuevosDatos)) {
			// No hay nada que actualizar, mostrar los cambios de aspectos si han ocurrido
			aspectoTropa.sprite = ControladorPrincipal.instance.aspectos[ControladorPartida.instance.datosPartida.jugadores[nuevosDatos.jugador].aspecto];
			overlayTropa.sprite = ControladorPrincipal.instance.colorAspectos[ControladorPartida.instance.datosPartida.jugadores[nuevosDatos.jugador].aspecto];
			return;
		}
		// Mostrar animación de los cambios
		if(datosAnteriores.jugador != nuevosDatos.jugador) {
			textoCambio.Animar(nuevosDatos.tropas);
		} else {
			textoCambio.Animar(nuevosDatos.tropas-datosAnteriores.tropas);
		}
		AsignarTerritorio(nuevosDatos);
	}

	/// <summary> Método invocado cuando se selecciona el territorio </summary>
	public void Seleccionar() {
		ControladorPartida.instance.SeleccionTerritorio(this);
	}
	
	/// <summary> Muestra recursivamente todos los territorios del jugador conectados con este territorio </summary>
	public void MostrarContiguosUsuario() {
		Oculto = false;
		foreach(Territorio t in conexiones){
			if(pertenenciaJugador == t.pertenenciaJugador && t.Oculto){
				t.Oculto = false;
				t.MostrarContiguosUsuario();
			}
		}
	}

}

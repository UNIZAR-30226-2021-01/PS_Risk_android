using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;

public class ControladorSalaEspera : MonoBehaviour {
	private const string MENSAJE_COMIENZO = "{\"tipo\":\"Iniciar\"}";
	[SerializeField]
	private TextMeshProUGUI nombrePartida;
	[SerializeField]
	private ControladorAmigos listaAmigos;
	[SerializeField]
	private GameObject jugadorPrefab;
	[SerializeField]
	private Transform padreJugadores;
	List<ClasesJSON.Jugador> jugadores;
	private int host;
	
	public void ActualizarDatosSalaEspera(string datosSalaJSON){
		ClasesJSON.DatosSala datosSala = JsonConvert.DeserializeObject<ClasesJSON.DatosSala>(datosSalaJSON);
		nombrePartida.text = datosSala.nombrePartida;
		host = datosSala.jugadores.ToArray()[0].id;
		jugadores = datosSala.jugadores;
		for(int i = 0; i < padreJugadores.childCount; i++) {
			Destroy(padreJugadores.GetChild(i).gameObject);
		}
		foreach (var jugador in jugadores) {
			JugadorLista nuevoJugador = Instantiate(jugadorPrefab, padreJugadores).GetComponent<JugadorLista>();
			nuevoJugador.nombre.text = jugador.nombre;
			nuevoJugador.icono.sprite = ControladorPrincipal.instance.iconos[jugador.icono];
			nuevoJugador.tropa.sprite = ControladorPrincipal.instance.aspectos[jugador.aspecto];
			nuevoJugador.tropaOveraly.sprite = ControladorPrincipal.instance.colorAspectos[jugador.aspecto];
			nuevoJugador.tropaOveraly.color = ControladorPrincipal.instance.coloresJugadores[jugadores.IndexOf(jugador)];
		}
		listaAmigos.RecargarAmigos();
	}
	
	public async void EmpezarPartida(){
		if(jugadores.Count < 3){
			ControladorPrincipal.instance.PantallaError("Se necesitan como mínimo 3 jugadores para comenzar la partida");
			return;
		}
		if(ControladorPrincipal.instance.usuarioRegistrado.id == host){
			await ConexionWS.instance.EnviarWS(MENSAJE_COMIENZO);
		}
	}

}

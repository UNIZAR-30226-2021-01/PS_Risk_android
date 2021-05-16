using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapa : MonoBehaviour {
	/// <summary>Lista con los controladores de las tropas, asignados desde el editor</summary>
	public Territorio[] territorios;

	/// <summary> Sprites que indican si un continente esta controlado por completo por un jugador, asignadoes desde el editor</summary>
	public SpriteRenderer[] indicadoresContinentes;

	/// <summary>Color con el que brillan los continentes cuando estos son controlados por un único jugador</summary>
	public Color colorBrilloContinentes;

	//Terriorios pertenecientes a cada continente
	private List<int>[] territoriosContinentes = {
		new List<int>(new int[]{7,8,9,10,11,12}),
		new List<int>(new int[]{13,14,15,16,17,18,19,20,21,22,23,24}),
		new List<int>(new int[]{0,1,2,3,4,5,6}),
		new List<int>(new int[]{33,34,35,36,37,38,39,40,41}),
		new List<int>(new int[]{29,30,31,32}),
		new List<int>(new int[]{25,26,27,28})
	};

	///<summary>Actualiza los datos del territorio especificado</summary>
	///<param name="nuevoTerritorio">Territorio a actualizar</param>
	public void ActualizarTerritorio(ClasesJSON.Territorio nuevoTerritorio) {
		territorios[nuevoTerritorio.id].ActualizarTerritorio(nuevoTerritorio);
		ActualizarContinentes();
	}
	
	///<summary>Actualiza los datos de los territorios especificados</summary>
	///<param name="listaTerritorios">Lista de territorios a actualizar</param>
	public void ActualizarTerritorios(List<ClasesJSON.Territorio> listaTerritorios) {
		foreach (ClasesJSON.Territorio nt in listaTerritorios) {
			territorios[nt.id].ActualizarTerritorio(nt);
		}
		ActualizarContinentes();
	}
	
	///<summary>Asigna los datos de los territorios especificados</summary>
	///<param name="listaTerritorios">Lista de territorios a actualizar</param>
	public void AsignarTerritorios(List<ClasesJSON.Territorio> listaTerritorios) {
		foreach (ClasesJSON.Territorio nt in listaTerritorios) {
			territorios[nt.id].AsignarTerritorio(nt);
		}
		ActualizarContinentes();
	}
	
	///<summary>Muestra todos los territorios del mapa</summary>
	public void MostrarTodosTerritorios() {
		foreach (Territorio t in territorios) {
			t.Oculto = false;
			t.Seleccionado = false;
		}
	}

	/// <summary>
	/// Muestra los territorios a los que se puede atacar desde
	/// el territorio seleccionado
	/// </summary>
	/// <param name="territorio">ID del territorio seleccionado</param>
	public void MostrarAtaque(int territorio) {
		OcultarTerritorios();
		foreach (Territorio t in territorios[territorio].conexiones) {
			if(territorios[territorio].pertenenciaJugador != t.pertenenciaJugador){
				t.Oculto = false;
			}
		}
		territorios[territorio].Oculto = false;
		territorios[territorio].Seleccionado = true;
	}
	
	/// <summary>
	/// Muestra todos los territorios pertenecientes al usuario que se
	/// encuentran conectados al territorio especificado
	/// </summary>
	/// <param name="territorio">ID del territorio</param>
	public void MostrarMovimiento(int territorio) {
		OcultarTerritorios();
		territorios[territorio].MostrarContiguosUsuario();
		territorios[territorio].Seleccionado = true;
	}

	// Oculta todos los territorios
	private void OcultarTerritorios() {
		foreach (Territorio t in territorios) {
			t.Oculto = true;
			t.Seleccionado = false;
		}
	}

	///<summary>Actualiza los indicadores (sprites) de los continentes</summary>
	public void ActualizarContinentes() {
		//Para cada continente
		for(int c = 0; c < 6; c++) {
			bool completo = true;
			int d = territorios[territoriosContinentes[c][0]].pertenenciaJugador;

			//Comprobar si un jugador tiene el continente completo
			foreach(int t in territoriosContinentes[c]) {
				if(d != territorios[t].pertenenciaJugador) {
					completo = false;
					break;
				}
			}

			if(completo) { //Si lo tiene, mostrar borde y colorear
				indicadoresContinentes[c].gameObject.SetActive(true);
				//indicadoresContinentes[c].color = ControladorPrincipal.instance.coloresJugadores[d];
				indicadoresContinentes[c].color = colorBrilloContinentes;
			} else { //Si no lo tiene, desactivar borde
				indicadoresContinentes[c].gameObject.SetActive(false);
			}
		}
	}
}

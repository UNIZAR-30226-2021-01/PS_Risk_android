using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapa : MonoBehaviour {
	/// <summary>Lista con los controladores de las tropas, asignados desde el editor</summary>
	public Territorio[] territorios;

	// PRUEBAS SOLO: Asignar territorios de manera aleatoria
	public void generarAleatorio() {
		for(int i = 0; i < 42; i++) {
			int cid = Random.Range(0,5);
			ClasesJSON.Territorio t = new ClasesJSON.Territorio();
			t.id = i;
			t.jugador = cid;
			t.tropas = Random.Range(1,99);
			territorios[i].ActualizarTerritorio(t);
		}
	}

	///<summary>Actualiza los datos del territorio especificado</summary>
	///<param name="nuevoTerritorio">Territorio a actualizar</param>
	public void ActualizarTerritorio(ClasesJSON.Territorio nuevoTerritorio) {
		territorios[nuevoTerritorio.id].ActualizarTerritorio(nuevoTerritorio);
	}
	
	///<summary>Actualiza los datos de los territorios especificados</summary>
	///<param name="listaTerritorios">Lista de territorios a actualizar</param>
	public void ActualizarTerritorios(List<ClasesJSON.Territorio> listaTerritorios) {
		foreach (ClasesJSON.Territorio nt in listaTerritorios) {
			territorios[nt.id].ActualizarTerritorio(nt);
		}
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
}

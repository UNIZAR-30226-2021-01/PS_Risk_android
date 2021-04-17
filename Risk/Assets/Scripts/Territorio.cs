using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Territorio : MonoBehaviour {
	[SerializeField]
	private TextMeshPro numeroTropas;
	[SerializeField]
	private SpriteRenderer aspectoTropa, overlayTerritorio;
	private ClasesJSON.Territorio datosAnteriores;
	public Territorio[] conexiones;
	
	public void ActualizarTerritorio(ClasesJSON.Territorio nuevosDatos){
		if(datosAnteriores.Equals(nuevosDatos)){
			// No hay nada que actualizar
			return;
		}
		aspectoTropa.sprite = ControladorPrincipal.instance.aspectos[nuevosDatos.jugador];
		overlayTerritorio.color = ControladorPrincipal.instance.coloresJugadores[nuevosDatos.jugador];
		numeroTropas.text = nuevosDatos.tropas.ToString();
		datosAnteriores = nuevosDatos;
	}

	/// <summary> Método invocado cuando se selecciona el territorio </summary>
	public void Seleccionado(){
	}

}

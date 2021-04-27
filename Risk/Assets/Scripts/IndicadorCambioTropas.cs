using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class IndicadorCambioTropas : MonoBehaviour {
	public TextMeshPro indicador;
	public Animator animador;
	
	public void Animar(int numero){
		bool positivo = numero > 0;
		indicador.color = (positivo ? Color.green : Color.red);
		indicador.text = (positivo ? "+" : "") + numero.ToString();
		animador.SetTrigger("Reproducir");
	}
	
}

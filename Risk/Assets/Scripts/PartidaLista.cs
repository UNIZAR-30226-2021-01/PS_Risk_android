using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PartidaLista : MonoBehaviour {
	public int id;
	[SerializeField]
	private TextMeshProUGUI textoNotificacion;
	public void ActualizarTexto(string texto){
		textoNotificacion.text = texto;
	}

	public void Unirse(){
		// TODO
	}

}

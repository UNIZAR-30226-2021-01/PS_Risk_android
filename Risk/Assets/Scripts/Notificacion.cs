using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

/*
    Script para el prefab de notificacion
*/
public class Notificacion : MonoBehaviour {
	//public Image icono; //TODO: Icono identificativo del tipo de notificación
	public TextMeshProUGUI nombre; //Texto de información de la notificación

    private string emisor; //Emisor de
	private int id; //ID de la notificación
    private string tipo; //Tipo de notificación, usado para saber que hacer al aceptar la notificación

    private const string TIPO_AMISTAD = "amistad";
    private const string TIPO_TURNO = "turno";
    private const string TIPO_INVICATION = "invitacion";
	
	public void Rechazar() {
		//TODO
	}

    public void Aceptar() {
        switch(tipo) {
            case TIPO_AMISTAD:
                Debug.Log("[Notificacion] Aceptando petición de amistad");
                //TODO
                break;

            case TIPO_TURNO:
                //TODO
                break;

            case TIPO_INVICATION:
                //TODO
                break;

            default:
                Debug.LogError("[Notificacion] Tipo de notificacion '" + tipo + "' desconocido");
                break;
        }
    }

    //Actualiza los datos de la clase
    public void Actualizar(ClasesJSON.Notificacion n) {
        id = n.idNotificacion;
        tipo = n.tipo;

        //Actualizar texto
        nombre.text = n.infoExtra;
    }
}

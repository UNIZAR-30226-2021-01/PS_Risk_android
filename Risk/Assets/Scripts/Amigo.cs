using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

/// <summary>
/// Script usado para controlar el prefab de la lista de amigos
/// </summary>
public class Amigo : MonoBehaviour {

	/// <summary>Imagen con el icono del usuario</summary>
	public Image icono;

	/// <summary>Compontente de Texto que muestra el nombre del usuario</summary>
	public TextMeshProUGUI nombre;

	/// <summary>ID del usuario</summary>
	public int id;

	/// <summary>Instancia del Controlador de Amigos usado en la escena del juego</summary>
	public ControladorAmigos controladorAmigos;
	
	/// <summary>Envia petición al servidor con el id de este amigo para ser borrado de la lista de amigos del usuario</summary>
	public async void BorrarAmigo(){
		controladorAmigos.AsignarAmigoBorrar(this);
	}
	
	/// <summary>Invitar al amigo a una sala</summary>
	public async void InvitarAmigo(){
		ControladorPrincipal.instance.PantallaCarga(true);
		ClasesJSON.InvitacionSala invitacion = new ClasesJSON.InvitacionSala(id);
		string datos = JsonConvert.SerializeObject(invitacion);
		await ConexionWS.instance.EnviarWS(datos);
		try {
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(datos);
			if(error.code != 0) {
				// Error
				ControladorPrincipal.instance.PantallaError(error.err);
			} else {
				ControladorPrincipal.instance.PantallaInfo("Invitación enviada exitosamente");
			}
		} catch {
			ControladorPrincipal.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
		}
		ControladorPrincipal.instance.PantallaCarga(false);	
	}

}

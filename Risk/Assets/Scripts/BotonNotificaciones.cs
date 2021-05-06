using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>Script que controla el boton de notificaciones</summary>
public class BotonNotificaciones : MonoBehaviour
{
	[SerializeField]
	private Button botonNotificacion; //Boton de notificacion

	[SerializeField]
	private TextMeshProUGUI numeroNotificaciones; //Texto que indica el numero de notificaciones

	[SerializeField]
    private GameObject iconoCargando; // Gameobjects del icono de carga

    [SerializeField]
    private GameObject indicadorNotificaciones; //Imagen que indica si hay notificaciones y sirve de fondo para el numero de estas

    private const int tiempoRefresh = 5000; //Tiempo en milisegundos entre re-peticiones de obtencion de notificaciones

    private void OnEnable() {
		//Re-iniciar los boton
		botonNotificacion.interactable = false;
		numeroNotificaciones.text = "LD";
		indicadorNotificaciones.SetActive(false);
        iconoCargando.SetActive(true);

		ActualizarNotificaciones();
    }

	//Actualiza las notificaciones en el controlador 
	private async void ActualizarNotificaciones() {
        //Repetir obtencion de notificaciones mientras el boton de notificaciones este en pantalla
        //Repetir cada tiempoRefresh segundos
		while(this != null && this.gameObject.activeInHierarchy) {
            //Debug.Log("Obteniendo notificaciones...");

            //Obtener notificaciones
			await ControladorNotificaciones.ObtenerNotificaciones(false);

			//Obtener numero de notificaciones
			int n = 0;
			if(ControladorNotificaciones.notificaciones != null)
				n = ControladorNotificaciones.notificaciones.Count;

			//Actualizar los datos en pantalla, permitirle dar al boton
			indicadorNotificaciones.SetActive(true);
			botonNotificacion.interactable = true;
            iconoCargando.SetActive(false);

			if(n < 0)
				numeroNotificaciones.text = "ER";
			else if(n == 0)
				indicadorNotificaciones.SetActive(false);
			else if(n > 9)
				numeroNotificaciones.text = "9+";
			else
				numeroNotificaciones.text = n.ToString();

			//Scheudle esta funcion para dentro de 10 segundos
            await Task.Delay(tiempoRefresh);
		}
	}
}
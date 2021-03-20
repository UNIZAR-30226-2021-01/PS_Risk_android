using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorUI : MonoBehaviour
{
    private static int escaladoInterfaz = 1; //Indica el escalado de la interafz, numero natural, dependiente del tamaño de la pantalla

    // Start is called before the first frame update
    void Start()
    {
        //Actualizamos el escalado de la interfaz
        actualizarEscalado();

        Debug.Log("Escalado de Interfaz: " + escaladoInterfaz + " (Para resolución " + Screen.height + "px vertical)");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Verifica que el usuario haya sido verificado correctamente
    //y le redirecciona a la pantalla principal de la aplicación
    public void login() {
        //TODO: Verificación de datos

        abrirPantalla("Principal");
    }

    //Abre una pantalla con un nombre determinado
    public void abrirPantalla(string pantalla) {
        Debug.Log("Abriendo pantalla \"" + pantalla + "\"...");
        
        GameObject abrirEsteCanvas = gameObject.transform.Find(pantalla).gameObject; //Buscar pantalla/objeto a abrir

        if(abrirEsteCanvas == null) { //Si el objeto pedido no existe, mensaje de error
            Debug.LogError("No se ha podido abrir la pantalla de reglas\nNo se ha podido encontrar el child \"" + pantalla + "\" (Tipo Canvas)");
            return;
        }
        if ( !esCavnas(abrirEsteCanvas) ) { //Si el objeto a activar no es un canvas, mensaje de error
            Debug.LogError("Se ha seleccionado un objeto \"" + pantalla + "\" como pantalla que no es un canvas");
            return;
        }

        //No sabemos de donde se ha accedido a la nueva pantalla
        //(En caso de querer que se puedan acceder desde otro sitio)
        //Por lo que hay que desactivar todas las pantallas
        desactivar_pantalas();

        abrirEsteCanvas.SetActive(true);
    }

    //Desactiva todas las pantallas (canvases)
    private void desactivar_pantalas() {
        for(int i = 0; i < gameObject.transform.childCount; i++) {
            GameObject canvas = gameObject.transform.GetChild(i).gameObject; //Obtener hijo nº i

            if(canvas != null && esCavnas(canvas)) { //Desactivar aquellos gameobjects que sean canvases
                canvas.SetActive(false); //Desactivar
            }
        }
    }

    //Cambiar escalado de la interfaz
    //Usar en startup y si se desea cambiar la resolucion (raro en movil)
    public void actualizarEscalado() {
        //Inicializar el escalado de la interfaz segun la resolución vertical
        //Para dispositivos 1080p, este valor sera 3
        escaladoInterfaz = Screen.height / 360;
        if(escaladoInterfaz < 1)
            escaladoInterfaz = 1;

        //Escalar canvas
        CanvasScaler scaler = GetComponent<CanvasScaler>();
        scaler.scaleFactor = escaladoInterfaz;
    }

    //Determina si un GameObject es un canvas
    private bool esCavnas(GameObject obj) {
        return (obj.GetComponent<Canvas>() != null);
    }
}

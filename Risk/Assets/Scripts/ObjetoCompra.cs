using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/*
    Este MonoBehaivour se usa en la lista de aspectos y iconos a comprar
*/
public class ObjetoCompra : MonoBehaviour
{
    private bool esAspecto = false; //Si true, esta clase describe un aspecto, no un icono
    private int id = 0; //Indica la ID del aspecto u icono que esta clase describe
    private int coste = 0; //Coste del objeto que la clase describe

    [SerializeField]
    public Button boton_componente; //Componente botón del botón de compra
    [SerializeField]
    public GameObject indicadorVendido; //Señal que indica que el usuario ya tiene este objeto
    [SerializeField]
    public Image spriteIconoAspecto; //Panel que muestra el icono de perfil o aspecto
    [SerializeField]
    private TextMeshProUGUI texto_nombre; //Texto que indica el nombre del objeto
    [SerializeField]
    private TextMeshProUGUI texto_precio; //Texto que indica el precio
    [SerializeField]
    public Image spriteTropasColor; //Sprite-Mascara de la colorización de las tropas

    //Inicializar clase basandose en un icono
    public void Actualizar(ClasesJSON.Icono icono) {
        esAspecto = false;
        id = icono.id;
        coste = icono.precio;
        texto_nombre.text = "ICONO";
        texto_precio.text = "<i>" + coste + "</i>";

        //Comprobar si este objeto ya esta comprado
        //¿Eficiencia?
        bool sePuedeComprar = true;
        foreach(var o in ControladorSesion.iconos_comprados.iconos)
            if(o.id == id) {
                sePuedeComprar = false;
                break;
            }
        SetComprar(sePuedeComprar);
        ActualizarImagen();
    }

    //Inicializar clase basandose en un aspecto
    public void Actualizar(ClasesJSON.Aspecto aspecto) {
        esAspecto = true;
        id = aspecto.id;
        coste = aspecto.precio;
        texto_nombre.text = "ASPECTO";
        texto_precio.text = "<i>" + coste + "</i>";

        //Si no se tiene dinero suficiente, dehabilitar boton
        if(coste > ControladorUI.instance.usuarioRegistrado.riskos)
            boton_componente.enabled = false;

        //Comprobar si este objeto ya esta comprado
        //¿Eficiencia?
        bool sePuedeComprar = true;
        foreach(var o in ControladorSesion.aspectos_comprados.aspectos)
            if(o.id == id) {
                sePuedeComprar = false;
                break;
            }
        SetComprar(sePuedeComprar);
        ActualizarImagen();
    }

    //Actualiza la imagen mostrada
    private void ActualizarImagen() {
        if(spriteIconoAspecto != null || spriteTropasColor != null) {
            try {
                if(!esAspecto) { //Iconos
                    Sprite s = ControladorUI.instance.iconos[id];
                    spriteIconoAspecto.overrideSprite = s;
                } else { //Aspectos
                    spriteIconoAspecto.overrideSprite = ControladorUI.instance.aspectos[id];
                    spriteTropasColor.overrideSprite = ControladorUI.instance.aspectos_color[id];

                    //Mostrar colores para los aspectos (tropas)
                    spriteTropasColor.gameObject.SetActive(true);
                }
            } catch {}
        }
    }


    //Boton de comprar
    public void BotonComprar() {
        ControladorPerfil.instance.AbrirConfirmacionCompra(this);
    }

    //Comunicarse con Backend para comprar el objecto
    //Indicar al juego que este objeto se ha comprado
    public void Comprar() {
        //Marcar como comprado
        SetComprar(false);

        if(!esAspecto) {
            Debug.Log("Comprando Icono " + id + "...");
            //Hablar con la API

            //Actualizar riskos

            //Añadir a la lista de comprados
            ClasesJSON.Icono cjson = null;
            foreach(var o in ControladorSesion.iconos_tienda.tiendaIconos) {
                if(o.id == id) {
                    cjson = o;
                    break;
                }
            }
            ControladorSesion.iconos_comprados.iconos.Add(cjson);
        }
        else {
            Debug.Log("Comprando Aspecto " + id + "...");
            //Hablar con la API

            //Actualizar riskos

            //Añadir a la lista de comprados
            ClasesJSON.Aspecto cjson = null;
            foreach(var o in ControladorSesion.aspectos_tienda.tiendaAspectos) {
                if(o.id == id) {
                    cjson = o;
                    break;
                }
            }
            ControladorSesion.aspectos_comprados.aspectos.Add(cjson);
        }

        //Actualizar tienda
        ControladorPerfil.instance.ActualizarTienda();
    }

    //Permitir / Bloquear la posibilidad de darle al botón de comprar
    //Si la entrada es 'true', se permite que se pueda comprar y viceversa
    public void SetComprar(bool sePuedeComprar) {
        if(boton_componente != null)
            boton_componente.interactable = sePuedeComprar;
        if(indicadorVendido != null)
            indicadorVendido.SetActive(!sePuedeComprar);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using TMPro;

/*
	Este MonoBehaivour se usa en la lista de aspectos y iconos a comprar
*/
public class ObjetoCompra : MonoBehaviour {
	private bool esAspecto = false; //Si true, esta clase describe un aspecto, no un icono
	private int id = 0; //Indica la ID del aspecto u icono que esta clase describe
	private int coste = 0; //Coste del objeto que la clase describe

	[SerializeField]
	private Button botonCompra; //Componente botón del botón de compra
	[SerializeField]
	private GameObject indicadorVendido; //Señal que indica que el usuario ya tiene este objeto
	[SerializeField]
	private Image spriteIconoAspecto; //Panel que muestra el icono de perfil o aspecto
	[SerializeField]
	private TextMeshProUGUI nombre; //Texto que indica el nombre del objeto
	[SerializeField]
	private TextMeshProUGUI precio; //Texto que indica el precio
	[SerializeField]
	private Image spriteTropasColor; //Sprite-Mascara de la colorización de las tropas
	
	public ControladorPerfil controladorPerfil;

	//Inicializar clase basandose en un icono
	public void Actualizar(ClasesJSON.Icono icono) {
		esAspecto = false;
		id = icono.id;
		coste = icono.precio;
		nombre.text = "ICONO";
		precio.text = "<i>" + coste + "</i>";

		//Comprobar si este objeto ya esta comprado
		//¿Eficiencia?
		bool sePuedeComprar = true;
		foreach(var o in ControladorPrincipal.instance.iconosComprados.iconos)
			if(o.id == id) {
				sePuedeComprar = false;
				break;
			}
		SetComprar(sePuedeComprar);
		ActualizarDatos();

		//Si no se tiene dinero suficiente, dehabilitar boton, hacer texto de precio rojo
		if(coste > ControladorPrincipal.instance.usuarioRegistrado.riskos) {
			botonCompra.interactable = false;
			precio.color = new Color(1,0.5f,0.5f,1f);
		}
	}

	//Inicializar clase basandose en un aspecto
	public void Actualizar(ClasesJSON.Aspecto aspecto) {
		esAspecto = true;
		id = aspecto.id;
		coste = aspecto.precio;
		nombre.text = "ASPECTO";
		precio.text = "<i>" + coste + "</i>";

		//Comprobar si este objeto ya esta comprado
		//¿Eficiencia?
		bool sePuedeComprar = true;
		foreach(var o in ControladorPrincipal.instance.aspectosComprados.aspectos)
			if(o.id == id) {
				sePuedeComprar = false;
				break;
			}
		SetComprar(sePuedeComprar);
		ActualizarDatos();

		//Si no se tiene dinero suficiente, dehabilitar boton, hacer texto de precio rojo
		if(coste > ControladorPrincipal.instance.usuarioRegistrado.riskos) {
			botonCompra.interactable = false;
			precio.color = new Color(1,0.25f,0.25f,1f);
		}
		else if(coste <= 0) { //Si el coste es 0 o menor, decir que es gratis 
			precio.text = "<i>Gratis</i>";
		}
	}

	//Actualiza la imagen mostrada
	private void ActualizarDatos() {
		if(spriteIconoAspecto != null || spriteTropasColor != null) {
			try {
				if(!esAspecto) { //Iconos
					Sprite s = ControladorPrincipal.instance.iconos[id];
					spriteIconoAspecto.overrideSprite = s;
					nombre.text = ControladorPrincipal.instance.nombreIcono[id];
				} else { //Aspectos
					spriteIconoAspecto.overrideSprite = ControladorPrincipal.instance.aspectos[id];
					spriteTropasColor.overrideSprite = ControladorPrincipal.instance.colorAspectos[id];
					nombre.text = ControladorPrincipal.instance.nombreAspectos[id];

					//Mostrar colores para los aspectos (tropas)
					spriteTropasColor.gameObject.SetActive(true);
				}
			} catch {}
		}
	}


	//Boton de comprar
	public void BotonComprar() {
		controladorPerfil.AbrirConfirmacionCompra(this);
	}

	//Comunicarse con Backend para comprar el objecto
	//Indicar al juego que este objeto se ha comprado
	public void Comprar() {
		//Marcar como comprado, haya error o no
		Comprar_API();
	}

	//Manda mensaje a backend de compra
	private async void Comprar_API() {
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorPrincipal.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorPrincipal.instance.usuarioRegistrado.clave);
		form.AddField("cosmetico", id);
		if(esAspecto)
			form.AddField("tipo", "Aspecto");
		else
			form.AddField("tipo","Icono");

		//Obtener respuesta del servidor
		string respuesta = await ConexionHTTP.instance.RequestHTTP("comprar", form);

		//Procesar respuesta
		try {
			ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(respuesta);

			if(error.code != 0) //Mostrar pantalla de error solo si la respuesta no es error 0
				ControladorPrincipal.instance.PantallaError(error.err);
			else { //Si no, restar riskos, añadir cosmetico
				ControladorPrincipal.instance.usuarioRegistrado.riskos -= coste;
			
				if(!esAspecto) {
					ClasesJSON.Icono cjson = null;
					foreach(var o in ControladorPrincipal.instance.iconosTienda.tiendaIconos) {
						if(o.id == id) {
							cjson = o;
							break;
						}
					}
					ControladorPrincipal.instance.iconosComprados.iconos.Add(cjson);
				}
				else {
					ClasesJSON.Aspecto cjson = null;
					foreach(var o in ControladorPrincipal.instance.aspectosTienda.tiendaAspectos) {
						if(o.id == id) {
							cjson = o;
							break;
						}
					}
					ControladorPrincipal.instance.aspectosComprados.aspectos.Add(cjson);
				}

				//Actualizar tienda
				SetComprar(false);
				controladorPerfil.ActualizarTienda();
			}
		} catch {
			//Respuesta desconocida, ¿El servidor esta mandando una respuesta?
		}
	}

	//Permitir / Bloquear la posibilidad de darle al botón de comprar
	//Si la entrada es 'true', se permite que se pueda comprar y viceversa
	public void SetComprar(bool sePuedeComprar) {
		if(botonCompra != null)
			botonCompra.interactable = sePuedeComprar;
		if(indicadorVendido != null)
			indicadorVendido.SetActive(!sePuedeComprar);
	}
}

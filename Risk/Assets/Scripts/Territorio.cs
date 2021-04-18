using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Territorio : MonoBehaviour {

	/// <summary>ID del territorio, iniciar en el editor</summary>
	public int id;

	[SerializeField]
	private TextMeshPro numeroTropas;

	[SerializeField]
	private SpriteRenderer aspectoTropa, overlayTropa, overlayTerritorio, iconoTu;

	private ClasesJSON.Territorio datosAnteriores;

	/// <summary>Conexiones con otros territorios, inicializar en el editor</summary>
	public Territorio[] conexiones;
	
	public void ActualizarTerritorio(ClasesJSON.Territorio nuevosDatos){
		if(datosAnteriores.Equals(nuevosDatos)){
			// No hay nada que actualizar
			return;
		}
		aspectoTropa.sprite = ControladorPrincipal.instance.aspectos[nuevosDatos.jugador];
		overlayTropa.color = ControladorPrincipal.instance.coloresJugadores[nuevosDatos.jugador];
		overlayTerritorio.color = ControladorPrincipal.instance.coloresJugadores[nuevosDatos.jugador];
		numeroTropas.text = nuevosDatos.tropas.ToString();
		datosAnteriores = nuevosDatos;
	}

	/// <summary>Indica si el territorio debe de estar oscurecido (Usado al seleccionar un movimiento)</summary>
	/// <param name="estaOscurecido">Si 'true', el territorio se oscureze, y viceversa</param>
	public void Oscurecer(bool estaOscurecido)
	{
		overlayTropa.gameObject.SetActive(!estaOscurecido);
		aspectoTropa.gameObject.SetActive(!estaOscurecido);

		if(estaOscurecido)
			overlayTerritorio.color = Mapa.colorOscuro;
	}

	/// <summary>Colorea el territorio y la tropa segun un color</summary>
	/// <param name="color">Color a usar en el territorio y tropa</param>
	public void Colorear(Color color)
	{
		overlayTropa.color = color;
		overlayTerritorio.color = color;

		overlayTropa.gameObject.SetActive(true);
		aspectoTropa.gameObject.SetActive(true);
	}

	/// <summary>Actualiza el numero indicado en la tropa</summary>
	/// <param name="numero">Nuevo numero a indicar al usuario</param>
	public void NumeroTropa(int numero)
	{
		numeroTropas.text = numero.ToString();
	}

	/// <summary>Indica si la tropa deberia mostrar el mensaje '¡Tu!', para que el usuario sepa que esta tropa es suya</summary>
	/// <param name="esOwner">Si 'true', el mensaje sera mostrado, y viceversa</param>
	public void IndicarOwnerTropa(bool esOwner)
	{
		iconoTu.gameObject.SetActive(esOwner);
	}

	/// <summary> Método invocado cuando se selecciona el territorio </summary>
	public void Seleccionado(){
		ControladorPartida.instance.SeleccionTerritorio(id);
	}

}

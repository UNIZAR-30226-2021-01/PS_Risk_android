using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estructura que guarda los datos de un usuario
/// </summary>
[System.Serializable]
public class Usuario{
	/// <summary>
	/// ID del usuario
	/// </summary>
	public int id;

	/// <summary>
	/// ID del icono que el usuario esta usado
	/// </summary>
	public int icono;
	/// <summary>
	/// ID del aspecto que el usuario esta usando
	/// </summary>
	public int aspecto;
	/// <summary>
	/// Cantidad de Riskos (Moneda del juego) tiene el usuario
	/// </summary>
	public int riskos;

	/// <summary>
	/// Nombre del usuario
	/// </summary>
	public string nombre;
	/// <summary>
	/// Correo del usuario
	/// </summary>
	public string correo;
	/// <summary>
	/// Clave del usuario
	/// </summary>
	public string clave;

	/// <summary>
	/// Indica si el usuario recive correos
	/// </summary>
	public bool recibeCorreos;
}

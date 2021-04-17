using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

/// <summary>
/// Colección de clases utilizadas para almacenar el contenido recibido del servidor en 
/// formato JSON. Las clases y atributos corresponden con aquellos de la API del servidor.
/// </summary>
public static class ClasesJSON {

	/// <summary>
	/// Opciones de deserialización de los JSONs, lanzara excepciones 
	/// si la deserializacion no funciona correctamente.
	/// </summary>
	public static JsonSerializerSettings settings = new JsonSerializerSettings {
		/*Error = (SpriteRenderer, args) =>
		{
			Debug.LogError("Error en Deserializacion: " + args.ErrorContext.Error.Message);
			args.ErrorContext.Handled = true;
		},*/
		MissingMemberHandling = MissingMemberHandling.Error
	};

	/// <summary>Clase serializable para el JSON 'RiskError'.</summary>
	[System.Serializable]
	public class RiskError {
		public int code;
		public string err;
	}
	
	/// <summary>Clase serializable para el JSON 'Cosmetico'.</summary>
	[System.Serializable]
	public class Cosmetico {
		public int id;
		public int precio;
	}
	
	/// <summary>Clase serializable para el JSON 'Cosmetico'.</summary>
	[System.Serializable]
	public class UsuarioCompleto {
		public Usuario usuario;
		public List<Cosmetico> aspectos, iconos, tiendaAspectos, tiendaIconos;
	}
	
	/// <summary>Clase serializable para el JSON 'Cosmetico'.</summary>
	[System.Serializable]
	public class Amigo {
		public int id, icono, aspecto;
		public string nombre;
	}

	/// <summary>Clase serializable para el JSON 'Cosmetico'.</summary>
	[System.Serializable]
	public class ListaAmigos {
		public List<Amigo> amigos;
	}

	/// <summary>Clase serializable para el JSON 'Cosmetico'.</summary>
	[System.Serializable]
	public class Notificacion {
		public string infoExtra;

		public string tipo;

		public int idEnvio;
	}
	
	/// <summary>Clase serializable para el JSON 'Cosmetico'.</summary>
	[System.Serializable]
	public class ListaNotificaciones {
		public List<Notificacion> notificaciones;
	}
	
	/// <summary>
	/// Clase serializable para el JSON 'Aspecto'.
	/// Aspecto, usado para deserializar datos de usuario y tienda
	/// </summary>
	[System.Serializable]
	public class Aspecto {
		public int id;		//ID del Aspecto
		public int precio;	//Precio del Aspecto en la tienda
	}
	
	/// <summary>
	/// Clase serializable para el JSON 'ListaAspectosUsuario'.
	/// Lista de los aspectos que tiene el usuario
	/// </summary>
	[System.Serializable]
	public class ListaAspectosUsuario {
		public List<Aspecto> aspectos;
	}
	
	/// <summary>
	/// Clase serializable para el JSON 'ListaAspectosTienda'.
	/// Lista de los aspectos que estan disponibles en la tienda
	/// </summary>
	[System.Serializable]
	public class ListaAspectosTienda {
		public List<Aspecto> tiendaAspectos;
	}
	
	/// <summary>
	/// Clase serializable para el JSON 'Icono'.
	/// Icono, usado para deserializar datos de usuario y tienda
	/// </summary>
	[System.Serializable]
	public class Icono {
		public int id;		//ID del Icono
		public int precio;	//Precio del Icono en la tienda
	}
	
	/// <summary>
	/// Clase serializable para el JSON 'ListaIconosUsuario'.
	/// Lista de los iconos de perfil que tiene el usuario
	/// </summary>
	[System.Serializable]
	public class ListaIconosUsuario {
		public List<Icono> iconos;
	}
	
	/// <summary>
	/// Clase serializable para el JSON 'ListaIconosTienda'.
	/// Lista de los iconos de perfil que estan disponibles en la tienda
	/// </summary>
	[System.Serializable]
	public class ListaIconosTienda {
		public List<Icono> tiendaIconos;
	}
	
	/// <summary>
	/// Clase serializable para el JSON 'CreacionSala'.
	/// Estructura a enviar para la creación de la sala
	/// </summary>
	[System.Serializable]
	public class CreacionSala {
		public int idUsuario;
		public string clave;
		public int tiempoTurno;
		public string nombreSala;
		
		public CreacionSala(int id, string cl, int ti, string nm){
			idUsuario = id;
			clave = cl;
			tiempoTurno = ti;
			nombreSala = nm;
		}
	}
	
	/// <summary>
	/// Clase serializable para el JSON 'AceptarSala'.
	/// Estructura a enviar para aceptar una sala
	/// </summary>
	[System.Serializable]
	public class AceptarSala {
		public int idUsuario, idSala;
		public string clave;
		
		public AceptarSala(int us, string cl, int sl) {
			idUsuario = us;
			clave = cl;
			idSala = sl;
		}
	}
	
	/// <summary>
	/// Clase serializable para el JSON 'InvitacionSala'.
	/// Invitacion de amigo a sala
	/// </summary>
	[System.Serializable]
	public class InvitacionSala {
		public int idInvitado;
		public string tipo = "Invitar";
		
		public InvitacionSala(int id){
			idInvitado = id;
		}
	}
	
	/// <summary>
	/// Clase serializable para la solicitud de la lista de partidas
	/// </summary>
	public class ListaPartidas {
		public List<Partida> partidas;
	}


	/// <summary>
	/// Clase serializable que representa una partida en la lista de partidas
	/// </summary>
	public class Partida {
		public int id, turnoActual, tiempoTurno;
		public string nombre, nombreTurno, ultimoTurno;
	}

	/// <summary>
	/// Clase serializable para el mensaje 'MensajeWebsocket'.
	/// Mensaje enviado por websocket. Contiene un string con un caracter que indica el tipo de mensaje
	/// </summary>
	public class MensajeWebsocket {
		public string _tipoMensaje;
		
	}
	
	/// <summary>
	/// Clase serializable para el mensaje Websocket 'RiskErrorWS'.
	/// Error producido por websocket
	/// </summary>
	[System.Serializable]
	public class RiskErrorWS : MensajeWebsocket{
		public int code;
		public string err;	
	}
	
	/// <summary>
	/// Clase serializable para el mensaje Websocket 'DatosSala'.
	/// Datos de Sala de espera
	/// </summary>
	[System.Serializable]
	public class DatosSala : MensajeWebsocket{
		public int tiempoTurno, idSala;
		public string nombrePartida;
		public List<Jugador> jugadores;
	}
	
	/// <sumary>
	/// Clase serializable para el mensaje Websocket 'Partida Completa'.
	/// </sumary>
	[System.Serializable]
	public class PartidaCompleta : MensajeWebsocket {
		public int tiempoTurno, turnoActual, fase;
		public string nombreSala, ultimoTurno;
		public List<Territorio> territorios;
		public List<Jugador> jugadores;
	}
	
	/// <summary>
	/// Clase serializable para el mensaje Websocket 'Jugador'.
	/// Jugador de una partida
	/// </summary>
	[System.Serializable]
	public class Jugador {
		public int id, icono, aspecto, refuerzos;
		public string nombre;
		public bool sigueVivo;
	}
	
	/// <sumary>
	/// Clase serializable para la representacion de territorios
	/// </sumary>
	public class Territorio {
		public int id, jugador, tropas;

		public override bool Equals(object obj) {
			if (obj == null || (obj.GetType() != GetType())){
				return false;
			} else {
				Territorio otro = (Territorio) obj;
				return ((this.id == otro.id) && (this.jugador == otro.jugador) && (this.tropas == otro.tropas));
			}
		}
	}
}

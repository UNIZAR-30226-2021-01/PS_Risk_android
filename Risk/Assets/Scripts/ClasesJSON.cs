using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

// Colección de clases utilizadas para almacenar el contenido recibido del servidor en
// formato JSON. Las clases y atributos corresponden con aquellos de la API del servidor.
public static class ClasesJSON {

	//Opciones de deserialización de los JSONs, lanzara excepciones si la deserializacion no funciona correctamente
	public static JsonSerializerSettings settings = new JsonSerializerSettings {
		/*Error = (SpriteRenderer, args) =>
		{
            Debug.LogError("Error en Deserializacion: " + args.ErrorContext.Error.Message);
			args.ErrorContext.Handled = true;
		},*/
		MissingMemberHandling = MissingMemberHandling.Error
	};

	[System.Serializable]
	public class RiskError {
		public int code;
		public string err;
	}
	
	[System.Serializable]
	public class Cosmetico {
		public int id;
		public int precio;
	}
	
	[System.Serializable]
	public class UsuarioCompleto {
		public Usuario usuario;
		public List<Cosmetico> aspectos, iconos, tiendaAspectos, tiendaIconos;
	}
	
	[System.Serializable]
	public class Amigo {
		public int id, icono, aspecto;
		public string nombre;
	}

	[System.Serializable]
	public class ListaAmigos {
		public List<Amigo> amigos;
	}

	[System.Serializable]
	public class Notificacion {
		public string infoExtra;

		public string tipo;

		public int idEnvio;
	}
	
	[System.Serializable]
    public class ListaNotificaciones {
        public List<Notificacion> notificaciones;
    }
	
}

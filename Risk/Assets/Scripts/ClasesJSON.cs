using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ClasesJSON {
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
	public class Notificacion {
		//Información extra sobre la notificación
		public string infoExtra;

		//Tipo de notificación
		public string tipo;

		//Emisor de la notificación
		public int idEnvio;
	}
	
}

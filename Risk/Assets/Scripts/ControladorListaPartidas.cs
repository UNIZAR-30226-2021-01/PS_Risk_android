using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class ControladorListaPartidas : MonoBehaviour {
	private List<ClasesJSON.Partida> partidas;
	[SerializeField]
	private GameObject partidaPrefab, noPartidaPrefab;
	[SerializeField]
	private Transform padrePartidas;
	
	/// <summary>Realiza una petición al servidor para actualizar la lista de partidas</summary>
	public async void RecargarPartidas() {
		// Borrar todas las partidas de la lista
		for(int i = 0; i < padrePartidas.childCount; i++) {
			Destroy(padrePartidas.GetChild(i).gameObject);
		}
		// Crear formulario a enviar
		WWWForm form = new WWWForm();
		form.AddField("idUsuario", ControladorPrincipal.instance.usuarioRegistrado.id);
		form.AddField("clave", ControladorPrincipal.instance.usuarioRegistrado.clave);
		// Enviar petición al servidor
		string recibido = await ConexionHTTP.instance.RequestHTTP("partidas", form);
		// En algunas circunstancias el servidor no envía un array, en ese caso no hay amigos
		if (!recibido.Contains("[")) {
			Instantiate(noPartidaPrefab, padrePartidas);
			return;
		}
		try {
			List<ClasesJSON.Partida> listaPartidas = JsonConvert.DeserializeObject<ClasesJSON.ListaPartidas>(recibido, ClasesJSON.settings).partidas;
			if (listaPartidas.Count == 0) {
				Instantiate(noPartidaPrefab, padrePartidas);
				return;
			}
			foreach (var partida in listaPartidas) {
				PartidaLista nuevaPartida = Instantiate(partidaPrefab, padrePartidas).GetComponent<PartidaLista>();
				nuevaPartida.id = partida.id;
				if(partida.nombreTurno == ControladorPrincipal.instance.usuarioRegistrado.nombre){
					nuevaPartida.ActualizarTexto("<b><color=#FF0000>" + partida.nombre + "</color></b>: Es tu turno");
				} else {
					nuevaPartida.ActualizarTexto("<b><color=#FF0000>" + partida.nombre + "</color></b>: Turno de <b><color=#FF0000>" + partida.nombreTurno + "</color></b>");
				}
			}
		} catch {
			try {
				// Error, mostrar mensaje de error
				ClasesJSON.RiskError error = JsonConvert.DeserializeObject<ClasesJSON.RiskError>(recibido, ClasesJSON.settings);
				ControladorPrincipal.instance.PantallaError(error.err);
			} catch {
				// No hay error
				print(recibido);
				ControladorPrincipal.instance.PantallaError("Respuesta desconocida recibida desde el servidor");
			}
		}

	}
}

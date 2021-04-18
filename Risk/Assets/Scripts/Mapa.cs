using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mapa : MonoBehaviour {
    /// <summary>Lista con los controladores de las tropas, inicializar en el editor</summary>
    public Territorio[] territorios;

    //Valor alfa para los territorios
    private const float ALFA_TERRITORIOS = 0.75f;

    //Los colores de cada territorio
    private Color[] coloresTerritorios;

    //Colores usado para territorios oscurecidos
    public static Color colorOscuro;

    // Start is called before the first frame update
    void Start()
    {
        coloresTerritorios = new Color[42];

        colorOscuro = Color.black;
		colorOscuro.a = ALFA_TERRITORIOS;

		for(int i = 0; i < 42; i++)
			territorios[i].id = i;

        //Pruebas
        mostrarOff();
        for(int i = 0; i < 42; i++) {
            int c_id = Random.Range(0,5);
            Color c = ControladorPrincipal.instance.coloresJugadores[c_id];
            actualizarColor(i,c);
            mostrarColor(i);
            actualizarNumeroTropa(i,Random.Range(1,99));
            actualizarOwnerTropa(i,(c_id == 0));
        }
		mostrarColor();
        //mostrarOscuro(1); //Hacer que el territorio 1 este oscurezido.
        //mostrarOff(2); //Deshabilita el mostrado de color en el territorio 2
    }

    ///<summary>Cambia el numero indicado en una tropa</summary>
    ///<param name="territorio">ID del territorio con la tropa a modificar</param>
    ///<param name="numero">Nuevo numero de tropas a indicar en el mapa</param>
    public void actualizarNumeroTropa(int territorio, int numero) {
        territorios[territorio].NumeroTropa(numero);
    }

    ///<summary>Cambia si una tropa especificada debe indicar que es del jugador</summary>
    ///<param name="territorio">ID del territorio con la tropa a modificar</param>
    ///<param name="esDelJugador">Si 'true', la tropa indicara que es del jugador, y viceversa</param>
    public void actualizarOwnerTropa(int territorio, bool esDelJugador) {
        territorios[territorio].IndicarOwnerTropa(esDelJugador);
    }

    ///<summary>Cambia el color de un territorio y su tropa</summary>
    ///<param name="territorio">ID del territorio</param>
    ///<param name="colorTerritorio">Nuevo color para el territorio y para las tropas</param>
    public void actualizarColor(int territorio, Color colorTerritorio) 
    {
        try {
            coloresTerritorios[territorio] = colorTerritorio;
			coloresTerritorios[territorio].a = ALFA_TERRITORIOS;
        } catch {
            Debug.LogError("[Controlador Mapa] No se ha podido cambiar el color del territorio " + territorio);
        }
    }

    ///<summary>Mostrar el color del jugador de un territorio y su tropa</summary>
    ///<param name="territorio">ID del Territorio</param>
    public void mostrarColor(int territorio) {
        Territorio t = territorios[territorio];

		t.gameObject.SetActive(true);
		t.Colorear(coloresTerritorios[territorio]);
    }

    ///<summary>Oscurecer el color de un territorio y su tropa</summary>
    ///<param name="territorio">ID del Territorio</param>
    public void mostrarOscuro(int territorio) {
        Territorio t = territorios[territorio];

		t.gameObject.SetActive(true);
		t.Oscurecer(true);
    }

    ///<summary>Quitar el color de un territorio y vuelve su tropa invisible/no interaccionable</summary>
    ///<param name="territorio">ID del Territorio</param>
    public void mostrarOff(int territorio) {
    	territorios[territorio].gameObject.SetActive(false);
    }

    ///<summary>Realiza los efectos de mostrarColor(int territorio) para todos los territorios</summary>
    public void mostrarColor() {
        for(int i = 0; i < 42; i++)
            mostrarColor(i);
    }

    ///<summary>Realiza los efectos de mostrarOscuro(int territorio) para todos los territorios</summary>
    public void mostrarOscuro() {
        for(int i = 0; i < 42; i++)
            mostrarOscuro(i);
    }

	///<summary>Muestra solo aquellos territorios adyecientes a un territorio específico</summary>
	///<param name="territorio">ID del territorio</param>
	public void mostrarSoloColindantes(int territorio)
	{
		//Oscurecer todos los territorios
		mostrarOscuro();

		//Colorear territorios colindantes
		foreach(Territorio t in territorios[territorio].conexiones)
			mostrarColor(t.id);

		//Colorear territorio central
		mostrarColor(territorio);
	}


    ///<summary>Realiza los efectos de mostrarOff(int territorio) para todos los territorios</summary>
    public void mostrarOff() {
        for(int i = 0; i < 42; i++)
            mostrarOff(i);
    }
}

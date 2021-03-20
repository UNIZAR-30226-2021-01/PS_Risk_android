using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Clase adjuntable a un elemento imagen el cual lo anima
    Produce un movimiento de derecha a izquierda lento .
*/
public class FondoAnimado : MonoBehaviour
{
    //Distancia maxima a recorrer
    private static float DISTANCIA_MAXIMA = 400;

    //Guarda la posición original del fondo
    private float posicionOriginal = 0;

    //Distancia que se ha recorrido
    private float distanciaRecorrida = 0;

    //Distancia que se mueve el fondo por segundo
    private float velocidad = 2f;

    // Start is called before the first frame update
    void Start()
    {
        posicionOriginal = gameObject.transform.localPosition.x;
    }

    // Update is called once per frame
    void Update()
    {
        //Moverse en la direccion opuesta, se ha llegado al limite
        if(distanciaRecorrida > DISTANCIA_MAXIMA) {
            velocidad = -velocidad;
            distanciaRecorrida -= DISTANCIA_MAXIMA;
        }

        float deltaDistancia = Time.deltaTime * velocidad;
        distanciaRecorrida += Mathf.Abs(deltaDistancia);

        Vector3 pos = new Vector3(gameObject.transform.localPosition.x + deltaDistancia, gameObject.transform.localPosition.y, gameObject.transform.localPosition.z);

        gameObject.transform.localPosition = pos;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    Esta clase hara que el objeto al que este adjunto
    se desvanezca a partir de un tiempo determinado y
    durante un tiempo determinado

    Después de esto, el objeto se vuelve inactivo

    El objeto requiere de un componente "CanvasGroup"
*/
public class AnimDesaparecer : MonoBehaviour
{
    //Tiempo de espera desde awake hasta empezar a desaparecer (Segundos)
    [SerializeField]
    public float tiempoEspera = 3;

    //Tiempo que dura la desaparición (Segundos)
    [SerializeField]
    public float tiempoDifuminacion = 1;

    //Timer de la animación
    private float animTimer = 0;

    //Canvas Group, controla el alpha
    private CanvasGroup cg;

    // Llamado cuando el objeto deja de estar inactivo
    void Awake()
    {
        animTimer = 0;

        cg = gameObject.GetComponent<CanvasGroup>();

        //El objeto no tiene el componente requirido
        if(cg == null) {
            gameObject.SetActive(false);
            Debug.LogWarning("Se ha añadido el script \"AnimDesaparecer.cs\" a " + gameObject.name + ", pero no tiene el componente necesario \"CanvasGroup\"");
        }
    }

    // Update is called once per frame
    void Update()
    {
        animTimer += Time.deltaTime;

        //Animacion terminada, desactivar objeto
        if(animTimer > (tiempoEspera + tiempoDifuminacion)) {
            gameObject.SetActive(false);
        }
        else if(animTimer > tiempoEspera) { //Difuminar
            float alpha = 1f - (animTimer - tiempoEspera) / tiempoDifuminacion; //Alpha en este frame

            cg.alpha = alpha; //Poner alpha al objeto
        }
    }
}

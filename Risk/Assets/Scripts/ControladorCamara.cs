using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControladorCamara : MonoBehaviour {
	private Camera mainCam;
	[SerializeField]
	private float velocidadZoom, maxTiempoToque, maxDistanciaToque;
	public bool permitirJugar = true;
	private bool permitirMovimiento;
	/// <summary> Posición en worldspace de la esquina inferior izquierda de la cámara </summary>
	private Vector2 esquinaII;
	/// <summary> Posición en worldspace de la esquina superior derecha de la cámara </summary>
	private Vector2 esquinaSD;
	public EventSystem evSys;
	private Vector2 posicionComienzoToque;
	private float tiempoToque;
	

	private void OnEnable() {
		mainCam.orthographicSize = 5;
		permitirMovimiento = true;
	}

	private void OnDisable() {
		mainCam.orthographicSize = 5;
		permitirMovimiento = false;
	}

	private void Awake() {
		mainCam = Camera.main;
		esquinaII = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0));
		esquinaSD = mainCam.ScreenToWorldPoint(new Vector3(Screen.currentResolution.width, Screen.currentResolution.height, 0));
	}

	private void Update() {
		if(permitirMovimiento && !EventSystem.current.IsPointerOverGameObject(0)){
			if(Input.GetMouseButtonDown(0)){
				Vector2 point = mainCam.ScreenToWorldPoint(Input.mousePosition);
				RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
				if(hit.collider != null){
					hit.collider.GetComponent<Territorio>().Seleccionado();
				}
			}
			// Un dedo, el usuario esta tocando la pantalla: Interactuar con territorios
			if(Input.touchCount == 1){
				Touch t = Input.GetTouch(0);
				switch(t.phase){
					case TouchPhase.Began:
						tiempoToque = Time.realtimeSinceStartup;
						posicionComienzoToque = t.position;
						break;
					case TouchPhase.Moved:
						mainCam.transform.position -= mainCam.ScreenToWorldPoint(t.deltaPosition) - mainCam.ScreenToWorldPoint(Vector2.zero);
						ReajustarPantalla();
						break;
					case TouchPhase.Ended:
						if(Time.realtimeSinceStartup-tiempoToque <= maxTiempoToque && Vector2.Distance(t.position, posicionComienzoToque) <= maxDistanciaToque){
							Vector2 punto = mainCam.ScreenToWorldPoint(Input.GetTouch(0).position);
							RaycastHit2D hit = Physics2D.Raycast(punto, Vector2.zero);
							if(hit.collider != null){
								hit.collider.GetComponent<Territorio>().Seleccionado();
							}
						}
						break;
					default: break;
				}
			}
			// Dos dedos, el usuario esta "pellizcando" la pantalla
			if(Input.touchCount >= 2){
				Touch t0 = Input.GetTouch(0);
				Touch t1 = Input.GetTouch(1);
				float dist = Vector2.Distance(t0.position, t1.position)-Vector2.Distance(t0.position+t0.deltaPosition, t1.position+t1.deltaPosition);
				mainCam.orthographicSize = Mathf.Lerp(1,5,Mathf.InverseLerp(1,5,mainCam.orthographicSize+dist*velocidadZoom));
				ReajustarPantalla();
			}
		}
	}

	/// <summary> Reajusta la pantalla para estar dentro de los bordes configurados al inicio </summary>
	private void ReajustarPantalla(){
		Vector2 nuevaEsquinaII = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0));
		Vector2 nuevaEsquinaSD = mainCam.ScreenToWorldPoint(new Vector3(Screen.currentResolution.width, Screen.currentResolution.height, 0));
		mainCam.transform.position += new Vector3(Mathf.Max(0, esquinaII.x-nuevaEsquinaII.x), Mathf.Max(0, esquinaII.y-nuevaEsquinaII.y), 0);
		mainCam.transform.position += new Vector3(Mathf.Min(0, esquinaSD.x-nuevaEsquinaSD.x), Mathf.Min(0, esquinaSD.y-nuevaEsquinaSD.y), 0);
	}

}

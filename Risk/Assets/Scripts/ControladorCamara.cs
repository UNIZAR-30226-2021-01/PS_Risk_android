using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControladorCamara : MonoBehaviour {
	private const float MIN_ZOOM = 1f, MAX_ZOOM = 5f;
	private Camera mainCam;
	[SerializeField]
	private float velocidadZoom, maxTiempoToque, maxDistanciaToque;
	private bool permitirMovimiento;
	// Posición en worldspace de la esquina inferior izquierda de la cámara
	private Vector2 esquinaII;
	// Posición en worldspace de la esquina superior derecha de la cámara
	private Vector2 esquinaSD;
	private Vector2 posicionComienzoToque;
	private float tiempoToque;
	private Vector3 ultimaPosicionRaton;
	public EventSystem evSys;
	

	private void OnEnable() {
		try {
			mainCam.orthographicSize = 5;
			permitirMovimiento = true;
		} catch {}
	}

	private void OnDisable() {
		try {
			mainCam.orthographicSize = 5;
			permitirMovimiento = false;
			ReajustarPantalla();
		} catch {}
	}

	private void Awake() {
		mainCam = Camera.main;
		esquinaII = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0));
		esquinaSD = mainCam.ScreenToWorldPoint(new Vector3(Screen.currentResolution.width, Screen.currentResolution.height, 0));
	}

	private void Update() {
		#if !UNITY_EDITOR
		if(permitirMovimiento && !EventSystem.current.IsPointerOverGameObject(0)){
		#else
		if(permitirMovimiento && !EventSystem.current.IsPointerOverGameObject(-1)){
				if(Input.GetMouseButtonDown(0)){
					Vector2 point = mainCam.ScreenToWorldPoint(Input.mousePosition);
					RaycastHit2D hit = Physics2D.Raycast(point, Vector2.zero);
					if(hit.collider != null){
						hit.collider.GetComponent<Territorio>().Seleccionar();
					} else {
						ControladorPartida.instance.Deseleccionar();
					}
				}
				if(Input.GetMouseButton(1) || Input.GetMouseButton(2)){
					mainCam.transform.position -= mainCam.ScreenToWorldPoint(Input.mousePosition-ultimaPosicionRaton) - mainCam.ScreenToWorldPoint(Vector2.zero);
				}
				mainCam.orthographicSize = Mathf.Lerp(MIN_ZOOM, MAX_ZOOM, Mathf.InverseLerp(MIN_ZOOM, MAX_ZOOM, mainCam.orthographicSize-Input.GetAxis("Mouse ScrollWheel")));
				ReajustarPantalla();
				ultimaPosicionRaton = Input.mousePosition;
			#endif
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
						// Ocurre in toque
						if(Time.realtimeSinceStartup-tiempoToque <= maxTiempoToque && Vector2.Distance(t.position, posicionComienzoToque) <= maxDistanciaToque){
							Vector2 punto = mainCam.ScreenToWorldPoint(Input.GetTouch(0).position);
							RaycastHit2D hit = Physics2D.Raycast(punto, Vector2.zero);
							if(hit.collider != null){
								hit.collider.GetComponent<Territorio>().Seleccionar();
							} else {
								ControladorPartida.instance.Deseleccionar();
							}
						}
						break;
				}
			}
			// Dos dedos, el usuario esta "pellizcando" la pantalla
			if(Input.touchCount >= 2){
				Touch t0 = Input.GetTouch(0);
				Touch t1 = Input.GetTouch(1);
				float dist = Vector2.Distance(t0.position, t1.position)-Vector2.Distance(t0.position+t0.deltaPosition, t1.position+t1.deltaPosition);
				mainCam.orthographicSize = Mathf.Lerp(MIN_ZOOM, MAX_ZOOM, Mathf.InverseLerp(MIN_ZOOM, MAX_ZOOM, mainCam.orthographicSize+dist*velocidadZoom));
				ReajustarPantalla();
			}
		}
	}

	// Reajusta la pantalla para estar dentro de los bordes configurados al inicio
	private void ReajustarPantalla(){
		Vector2 nuevaEsquinaII = mainCam.ScreenToWorldPoint(new Vector3(0, 0, 0));
		Vector2 nuevaEsquinaSD = mainCam.ScreenToWorldPoint(new Vector3(Screen.currentResolution.width, Screen.currentResolution.height, 0));
		mainCam.transform.position += new Vector3(Mathf.Max(0, esquinaII.x-nuevaEsquinaII.x), Mathf.Max(0, esquinaII.y-nuevaEsquinaII.y), 0);
		mainCam.transform.position += new Vector3(Mathf.Min(0, esquinaSD.x-nuevaEsquinaSD.x), Mathf.Min(0, esquinaSD.y-nuevaEsquinaSD.y), 0);
	}

}

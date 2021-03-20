using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class FormPost : MonoBehaviour {
	public TextMeshProUGUI uiText;
	private string textToSend;
	
	[System.Serializable]
	private class datosRecibidos{
		public int ID;
		public string Cadena;
		
		datosRecibidos(int n, string s){
			ID = n;
			Cadena = s;
		}
		
	}
	
	public void updateSendText(string newValue) {
		textToSend = newValue;
	}

	public void sendRequest() {
		StartCoroutine("SendRoutine");
	}
	
	IEnumerator SendRoutine() {
		WWWForm form = new WWWForm();
		form.AddField("dato", textToSend);
		UnityWebRequest www = UnityWebRequest.Post("https://fathomless-ridge-74437.herokuapp.com/pruebaPost", form);
		yield return www.SendWebRequest();
		if(www.isNetworkError || www.isHttpError) {
			uiText.text = "Error";
		} else {
			datosRecibidos recibido = JsonUtility.FromJson<datosRecibidos>(www.downloadHandler.text);
			uiText.text = "ID: " + recibido.ID + "\nCadena: " + recibido.Cadena;
		}
	}

}

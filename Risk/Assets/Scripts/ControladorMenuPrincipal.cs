﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControladorMenuPrincipal : MonoBehaviour
{
	[SerializeField]
	private Image fotoPerfil; //Icono de perfil

	// Start is called before the first frame update
	void OnEnable()
	{
		try {
			fotoPerfil.overrideSprite = ControladorPrincipal.instance.iconos[ControladorPrincipal.instance.usuarioRegistrado.icono];
		} catch {}
	}
}

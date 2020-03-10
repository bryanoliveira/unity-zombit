using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drink : MonoBehaviour {

	private bool dentro = false;

	private void Update() {
		if (dentro && Game_Controles.acao && Player_Score.Comprar(1000)) {
			// compra drink
		}
	}

	private void OnTriggerEnter(Collider obj) {
		if (obj.tag == "Player") {
			dentro = false;
			Player_Canvas.MostraEstatico ("Pressione A para comprar um drink (-1000)");
		}
	}

	private void OnTriggerExit(Collider obj) {
		if (obj.tag == "Player") {
			dentro = false;
			Player_Canvas.EscondeEstatico ();
		}
	}
}

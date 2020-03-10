using System.Collections;
using UnityEngine;

public class Telefone : MonoBehaviour {

	private bool disponivel = false;

	[SerializeField]
	private Transform dropzone;

	private void Update() {
		if (disponivel && Game_Controles.acao && Player_Score.Comprar(500)) {
			Helicoptero.eu.gameObject.SetActive(true);
			Helicoptero.eu.alvoAtual = dropzone;
			Player_Objetivo.eu.CompletaDica (1);
			Player_Canvas.MostraEstatico("Suporte a caminho");
            disponivel = false;
		}
	}

	private void OnTriggerEnter(Collider obj) {
		if (obj.tag == "Player") {
            if (Helicoptero.eu.gameObject.activeSelf) {
				disponivel = false;

				if(Helicoptero.disponivel)
                	Player_Canvas.MostraEstatico("Suporte a caminho");
				else
					Player_Canvas.MostraEstatico("Esfriando...");
            }
            else {
				Player_Canvas.MostraEstatico("Pedir suporte (-500)", new Sprite());
                disponivel = true;
            }
		}
	}
	private void OnTriggerExit(Collider obj) {
		if (obj.tag == "Player") {
			Player_Canvas.EscondeEstatico ();
			disponivel = false;
		}
	}
}

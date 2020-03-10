using UnityEngine;
using System.Collections;

public class Arma_Pegar : MonoBehaviour {

	[SerializeField]
	private int id = 0;
	public int balasDeixadas = 10;

	private bool perto = false;

	private Player_Armas armas;

	private void Update() {
		if (perto && Game_Controles.acao) {
			if (Player_Armas.emUso [0].id == id) {
				Player_Armas.emUso [0].AddBalas (balasDeixadas);
				Destroy (gameObject);
			} else if (Player_Armas.emUso [1].id == id) {
				Player_Armas.emUso [1].AddBalas (balasDeixadas);
				Destroy (gameObject);
			} else {
				Player_Armas.eu.Trocar (id);
				Destroy (gameObject);
			}
			Player_Canvas.EscondeEstatico ();
		}
	}

	private void OnTriggerEnter (Collider obj){
	   	if(obj.gameObject.tag == "Player") {
			if (Player_Armas.emUso [0].id == id || Player_Armas.emUso [1].id == id) {
				Player_Canvas.MostraEstatico ("Recarregar", new Sprite());
			} else {
				Player_Canvas.MostraEstatico ("Pegar", new Sprite());
			}
			perto = true;
	   	}
	}
	private void OnTriggerExit(Collider obj) {
		if (obj.gameObject.tag == "Player") {
			Player_Canvas.EscondeEstatico ();
			perto = false;
		}
	}
}
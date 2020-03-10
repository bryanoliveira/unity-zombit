using UnityEngine;
using System.Collections;

public class Mapa_AbreCaminho : MonoBehaviour {
	[SerializeField]
	private GameObject[] jogadosLonge;
	[SerializeField]
	private GameObject[] destroi;
	[SerializeField]
	private GameObject vestigio;

	void OnTriggerEnter (Collider col){
		if(col.tag == "Nuke" && Spawner.podeAbrirMapa) {
			Instantiate(vestigio, new Vector3(col.transform.position.x, -3f, col.transform.position.z), Quaternion.identity);
			foreach (GameObject porta in destroi) {
				Destroy (porta);
			}
			foreach (GameObject jogado in jogadosLonge) {
				jogado.transform.position = new Vector3(transform.position.x + Random.Range(0, 5), transform.position.y, transform.position.z + Random.Range(0, 5));
			}
			Player_Objetivo.eu.CompletaObjetivo (Player_Objetivo.objetivoAtual - 1); // só vai poder abrir mapa quando esse for o objetivo 
			Destroy(gameObject);
		}
	}
}
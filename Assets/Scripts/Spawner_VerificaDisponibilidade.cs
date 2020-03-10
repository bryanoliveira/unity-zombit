using UnityEngine;
using System.Collections;

public class Spawner_VerificaDisponibilidade : MonoBehaviour {
	public bool  disponivel = true;

	void OnTriggerEnter ( Collider col  ){
		if(col.tag == "Player") {
			disponivel = false;
		}
	}

	void OnTriggerExit ( Collider col  ){
		if(col.tag == "Player") {
			disponivel = true;
		}
	}
}
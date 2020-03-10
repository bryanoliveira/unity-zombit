using UnityEngine;
using System.Collections;

public class Mapa_CorAleatoria : MonoBehaviour {

	[SerializeField] private float maxVermelho = 1;
	private float maxVerde = 0.5f;
	private float maxAzul = 0;

	void Start (){
		SpriteRenderer sprite= GetComponent<SpriteRenderer>();
		sprite.color = new Color(Random.Range(0.5f, maxVermelho), Random.Range(0.5f, maxVerde), Random.Range(0.5f, maxAzul), 1.0f);
		Destroy(this);
	}
}
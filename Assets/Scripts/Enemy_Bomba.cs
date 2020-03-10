using UnityEngine;
using System.Collections;

public class Enemy_Bomba : MonoBehaviour {

	[SerializeField] private float timer = 10;
	public bool boa = true;

	[SerializeField] private GameObject explosao;
	[SerializeField] private SpriteRenderer sprite;
	[SerializeField] private Light brilho;
	[SerializeField] private AudioClip somCarga;
	[SerializeField] private AudioSource sound;

	void Start (){
		sound.PlayOneShot(somCarga);
		
		if (!boa || Random.Range(0, 10) > 3 || !Spawner.podeAbrirMapa) {
			boa = false;
			brilho.color = new Color (1, 1, 0, 1);
		} else {
			Player_Objetivo.eu.MostraDica (0);
		}
	}

	void FixedUpdate (){
		if(timer > 0)
			timer -= Player.time;
		else
			explode();
		brilho.range = 2 + timer * 2;
	}

	void OnTriggerEnter (Collider col){
		if(boa && col.tag == "Player") {
			Player_Score.eu.PegaBomba();
			Destroy(gameObject);
		}
	}

	void explode (){
		Instantiate(explosao, new Vector3(transform.position.x, transform.position.y + 5, transform.position.z), transform.rotation);
		Destroy(gameObject);
	}
}
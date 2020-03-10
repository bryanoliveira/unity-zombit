using UnityEngine;
using System.Collections;

public class Destroi : MonoBehaviour {

	public float tempo = 1;
	private float timer;
	[SerializeField] private bool bala = false;
	[SerializeField] private bool pu = false;
	[SerializeField] private bool arma = false;
	[SerializeField] private bool shotgun = false;
	[SerializeField] private bool explosao = false;
	[SerializeField] private bool nuke = false;
	private GameObject player;
	[SerializeField] private AudioSource sound;
	[SerializeField] private GameObject soundObj;
	[SerializeField] private AudioClip[] explosoes;

	void Start (){
		if(bala || pu || nuke) {
			player = GameObject.Find("Player");
		}
		if(pu) {
			timer = tempo;
		}
		StartCoroutine(Destrua(tempo));
		if(explosao) {
			if(nuke) {
				soundObj.transform.parent = null;
				Camera_Flasher.eu.Flash("branco", 1f, 1f);
				Player.timeScale = 0.3f;
			}
			sound.clip = explosoes[Random.Range(0, explosoes.Length)];
			sound.Play();

			StartCoroutine (DestroiCollider());
		}
	}
	void Update (){
		if(nuke) {
			if(Player.timeScale < 1) {
				Player.timeScale += Player.time;
			}
		}
		if(pu && timer > 0) {
			// Diminui o tamanho com o tempo
			transform.localScale = new Vector3(1,1,1) * timer/5;
			timer -= Player.time;
			
			// Move-se em direçao ao player
			Vector3 dir= player.transform.position - transform.position; 
			transform.Translate(dir * (timer/10) * Player.time, Space.World); 
		}
		
		if(arma && timer > 0) {
			// Diminui o tamanho com o tempo
			transform.localScale = new Vector3(1,1,1) * timer/15;
			timer -= Player.time;
		}
		
		if(shotgun && timer > 0) {
			// Aumenta o tamanho com o tempo
			transform.localScale = new Vector3(1,1,1) * Player.time;
			timer -= Player.time;
		}
	}

	IEnumerator Destrua (float time){
		yield return new WaitForSeconds(time);
		/*
		if(bala && GetComponent<Bala_Dano>().jaAtravessou == 0) {
			player.GetComponent<Player_Objetivo>().errou++;
		}
		*/
		Destroy(gameObject);
	}

	IEnumerator DestroiCollider() {
		yield return new WaitForSeconds(0.5f);
		Destroy(GetComponent<BoxCollider>());
	}
}
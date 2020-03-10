using UnityEngine;
using System.Collections;

public class Enemy_Saude : MonoBehaviour {

	// EnemyTypes: AI.cs
	private enemyTypes tipo;
	private int saude = 100;
	// AUDIOS
	[SerializeField] private AudioClip hitSound;

	// REFERÊNCIAS
	[SerializeField] private GameObject[] powerUp;
	[SerializeField] private GameObject[] pedaco;
	//Particulas 
	[SerializeField] private GameObject blood;
	[SerializeField] private GameObject bloodSprite;
	//
	[SerializeField] private SpriteRenderer meuSprite; // pra saber a cor dos pedaços
	[SerializeField] private GameObject bomba; // pro zumbi bomba spawnar

	void Awake (){
		tipo = GetComponent<Enemy_AI>().tipo;
	}
	void Start (){
		if (tipo == enemyTypes.Death) {
			saude = 2500;
			StartCoroutine (FicaSoltandoBomba ());
		}
		else if(tipo == enemyTypes.Gordo) 
			saude = Spawner.round * 50;
		else 
			saude = Spawner.round * 3 + 70;
	}

	void OnTriggerEnter (Collider obj) {
		if(obj.tag == "Explosao") {
			if (tipo != enemyTypes.Death) {
				aplicarDano (200);
			}
		}
		if (obj.tag == "Nuke") {
			aplicarDano (10000);
		}
	}

	public void aplicarDano (int dano){
		Player.eu.sound.PlayOneShot (hitSound);
		saude -= dano;
		// Sangue
		Instantiate(bloodSprite, new Vector3(transform.position.x, transform.position.y - 2, transform.position.z), transform.rotation);
		// Pedaço
		GameObject oPedaco = Instantiate(pedaco[Random.Range(0, pedaco.Length)], new Vector3(transform.position.x, transform.position.y - 1.8f, transform.position.z), transform.rotation) as GameObject;
		oPedaco.GetComponent<Destroi_Fade>().meuSprite.color = meuSprite.color;
		// Splash
		Instantiate(blood, transform.position, transform.rotation);

		if(tipo == enemyTypes.Death)
			Player_Canvas.AtualizaVidaBoss(saude);

		if(saude <= 0) {
			Morre();
		}
	}
	void Morre (){
		int pu = Random.Range(0, 50 + Spawner.round);
        // PowerUps
        if (pu < 1) {
            Instantiate(powerUp[0], this.transform.position, this.transform.rotation); // 0 - slow
        } else if (pu < 4) {
            Instantiate(powerUp[1], this.transform.position, this.transform.rotation); // 1 - saude
        } else if (pu < 6) {
            Instantiate(powerUp[2], this.transform.position, this.transform.rotation); // 2 - invencibilidade 
        } else if (pu < 9) {
            Instantiate(powerUp[3], this.transform.position, this.transform.rotation); // 3 - dobro de pontos
        } else if (pu < 12) {
            Instantiate(powerUp[4], this.transform.position, this.transform.rotation); // 4 - speed
		} else if (pu < 17) {
            Instantiate(powerUp[5], this.transform.position, this.transform.rotation); // 5 - municao
        } else if (pu < 21) {
            Instantiate(powerUp[6], this.transform.position, this.transform.rotation); // 6 - municao infinita
        }
	   
		if(tipo == enemyTypes.Bomba && Random.Range(0, 50 - Spawner.round * 2) < 10) {
			Instantiate(bomba, new Vector3(transform.position.x, transform.position.y - 0.45f, transform.position.z), transform.rotation);
		}
			
		Player_Armas.ZumbiMorreu (GetComponent<Collider>());

		Camera_Flasher.eu.Flash("azul", 0.1f, 0.3f);
		Camera_Flasher.eu.Zoom(0.1f, 0.3f);

		Spawner.zumbis.Remove (gameObject);

		if (tipo == enemyTypes.Death) {
			Player_Canvas.AvisaNoMeio ("BOSS DERROTADO", 0.5f);
			Player_Objetivo.eu.CompletaObjetivo (0);
			Spawner.podeAbrirMapa = true;
			Player_Canvas.MostraVidaBoss (false, "");
		}
		Destroy(gameObject);
	}

	private IEnumerator FicaSoltandoBomba() {
		for (;;) {
			yield return new WaitForSeconds (5);
			Instantiate (bomba, new Vector3 (transform.position.x, transform.position.y - 0.45f, transform.position.z), transform.rotation);
		}
	}
}
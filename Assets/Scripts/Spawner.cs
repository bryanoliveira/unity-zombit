using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	// Inteiros
	public static int round;
	private static int enemiesToSpawn;
    // Booleans
	public static bool podeSpawnar;
	public static bool podeAbrirMapa = false;

	public static List<GameObject> zumbis;

	// Eventos
	public delegate void SpawnerEvent();
	public static event SpawnerEvent EventRoundUp;

	// Referencias
	[SerializeField] private Transform[] spawnPoint; // não pode ser estático pq tem diferentes valores
	[SerializeField] private GameObject[] prefabZumbis; // prefab do zumbi a ser spawnado

	private static Spawner eu; // um generico pra segurar uma posição pra spawnar a altura do boss

	// cuidado ao spawnar instancias daqui pois reseta a classe toda
	private void Awake() {
		eu = this; // qualquer um que chamar serve
		round = 1;
		podeSpawnar = false;
		enemiesToSpawn = 15;
		podeAbrirMapa = false;
		EventRoundUp = null;
		Bala_Dano.vida = 1000;
		Bala_Dano.jaSpawnouCorvo = false;
	}

	private void Start() {
		zumbis = new List<GameObject> ();
	}

	private void OnTriggerStay (Collider col){
		if (col.tag == "Player") {
			// Se ainda ha inimigos pra spawnar
			if (podeSpawnar && zumbis.Count < 12) {
				if (enemiesToSpawn > 0) {
					Spawn ();
				} else if(zumbis.Count == 0) {
					StartCoroutine(RoundUp ());
				}
			}
		}
	}

	private IEnumerator RoundUp (){
		round++;
		Player_Canvas.AtualizaRound (round);
		if(EventRoundUp != null) {
			EventRoundUp ();
		}
		enemiesToSpawn = 14 + 2 * round;
		podeSpawnar = false;
		yield return new WaitForSeconds (3);
		podeSpawnar = true;
	}

	void Spawn (){
		GameObject obj, instancia;
		Transform pos;

		do {
			pos = spawnPoint[Random.Range(0, spawnPoint.Length)];
		} while(!pos.GetComponent<Spawner_VerificaDisponibilidade>().disponivel);

		if(round > 1 && round % 5 == 0)
			obj	= prefabZumbis[Random.Range(0, 3)];
		else if(round > 2)
			obj = prefabZumbis[Random.Range(0, 2)];
		else
			obj = prefabZumbis[0];

		instancia = Instantiate(obj, pos.position, pos.rotation) as GameObject;
		instancia.GetComponent<Enemy_AI>().minhaArvore = pos;
		zumbis.Add (instancia);
		enemiesToSpawn --;
	}

	public static void SpawnaCorvo(Transform arvore) {
		GameObject instancia = Instantiate(eu.prefabZumbis[3], new Vector3(arvore.position.x, eu.spawnPoint[0].position.y, arvore.position.z), eu.spawnPoint[0].rotation) as GameObject;
		instancia.GetComponent<Enemy_AI> ().minhaArvore = arvore;
		Camera_Follow.Mostra (instancia.transform, 2, 0.5f);
		Player_Canvas.AvisaNoMeio ("Corvo!", 0.5f);
		Player_Canvas.MostraVidaBoss (true, "Corvo");
	}

	// faz os zumbis irem ou não atras do player
	public static void AtualizaAlvos (){
		GameObject[] inimigas = GameObject.FindGameObjectsWithTag("Enemy");
		foreach(var i in inimigas) {
			i.GetComponent<Enemy_AI>().AtualizaTarget();
		}
		GameObject boss = GameObject.FindWithTag("Death");
		if(boss) {
			boss.GetComponent<Enemy_AI>().AtualizaTarget();
		}
	}
}
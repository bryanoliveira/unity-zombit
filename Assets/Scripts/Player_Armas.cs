using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_STANDALONE
using XInputDotNetPure;
#endif

public class Player_Armas : MonoBehaviour {
	// Booleans
	private static short slotSelecionado;
	public static bool municaoInfinita;

	// Armas
	[SerializeField]
	private GameObject monstrao;
	[SerializeField]
	private GameObject[] gunsPrefabs;

	public static Arma[] emUso; // Slots
	[SerializeField]
	private Arma[] guns;

	[SerializeField]
	private Player_Movimento mov;

	// Efeitos sonoros
	[SerializeField] private AudioClip audioTroca;
	// Referencias
	public AudioSource sound;

	private List<Collider> zumbis = new List<Collider>(); // pro joystick mirar sozinho

	public static Player_Armas eu;

	private void Awake() {
		slotSelecionado = 0;
		municaoInfinita = true;
		eu = this;
	}

	private void Start() {
		Spawner.EventRoundUp += BalasParaTodos;

		// como é estático, temos que colocar a faca e a pistola em uso
		emUso = new Arma[2];
		emUso [0] = guns [1];
		emUso [1] = guns [0];
		emUso [0].gameObject.SetActive (true);
		Player_Canvas.AtualizaArma(emUso[0].GetIcone(), emUso[0].GetNome()); 
	}

	void Update (){		
		if(Game_Controles.armaSwitch)
			Trocar(-1);
	}

	public void Trocar (int para){
		if (Player_Saude.monstro)
            return;
		if (para > -1) {
			if (para != emUso [slotSelecionado].id) {
				sound.PlayOneShot (audioTroca);

				emUso [slotSelecionado].DesligaFlares ();
				emUso [slotSelecionado].gameObject.SetActive (false);
				// descarta arma atual (instancia ela no chao)
				GameObject armaJogada = GameObject.Instantiate(gunsPrefabs[emUso[slotSelecionado].id], new Vector3(transform.position.x + 3, -1, transform.position.z), transform.rotation);
				armaJogada.GetComponent<Arma_Pegar> ().balasDeixadas = emUso [slotSelecionado].balas;
				// troca
				emUso [slotSelecionado] = guns [para];
				emUso [slotSelecionado].gameObject.SetActive (true);
					
				Player_Canvas.AtualizaArma(emUso[slotSelecionado].GetIcone(), emUso[slotSelecionado].GetNome()); // troca o icone
			}
		} else if (para == -1) { // só alterna entre os slots
			sound.PlayOneShot (audioTroca);

			emUso [slotSelecionado].DesligaFlares ();
			emUso [slotSelecionado].gameObject.SetActive (false);
			slotSelecionado = (short)(1 - slotSelecionado); // se o slot 1 estiver selecionado, o 0 desliga e vice-versa
			emUso [slotSelecionado].gameObject.SetActive (true);

			Player_Canvas.AtualizaArma(emUso[slotSelecionado].GetIcone(), emUso[slotSelecionado].GetNome()); // troca o icone
			Player_Objetivo.eu.CompletaDica(0);
		} else if (para == -2) { // troca se a outra arma tiver balas
			if (emUso [1 - slotSelecionado].balas > 0 || emUso[1 - slotSelecionado].tipo == TipoArma.branca)
				Trocar (-1);
		}
	}

	public void ViraMonstro (bool agora){
		emUso [slotSelecionado].DesligaFlares(); // desabilita o flare
		emUso [0].gameObject.SetActive (false);
		emUso [1].gameObject.SetActive(false);
		if(agora) {
			Player_Canvas.AtualizaArma(monstrao.GetComponent<SpriteRenderer>().sprite, "Adrenaline"); // troca o icone
			Player_Canvas.AtualizaBalas(-1);
			monstrao.SetActive(true);
			#if UNITY_STANDALONE
			GamePad.SetVibration(0, 0, 1);
			#endif
		}
		else {
			monstrao.SetActive (false);
			emUso [slotSelecionado].gameObject.SetActive (true);
			Player_Canvas.AtualizaArma(emUso[slotSelecionado].GetIcone(), emUso[slotSelecionado].GetNome()); // troca o icone
			#if UNITY_STANDALONE
			GamePad.SetVibration(0, 0, 0);
			#endif
		}
	}

	public void MaisBalas (int balas){
		Camera_Flasher.eu.Flash("amarelo", 0.1f, 0.6f);
		if (emUso [slotSelecionado].tipo != TipoArma.branca)
			emUso [slotSelecionado].AddBalas (balas);
		else if (emUso [1 - slotSelecionado].tipo != TipoArma.branca) {
			emUso [1 - slotSelecionado].AddBalas (balas);
		}
	}

	public void MunicaoInfinita(int tempo) {
		StopCoroutine ("MunicaoInfinitaRoutine");
		StartCoroutine(MunicaoInfinitaRoutine(tempo));
	}
	private IEnumerator MunicaoInfinitaRoutine (int tempo){
		municaoInfinita = true;
		Player_Canvas.MostraPowerup((int)TipoDoPowerup.municaoInfinita, tempo);
		Camera_Flasher.eu.Flash("amarelo", 0.4f, 0.8f);
		Camera_Flasher.eu.Overlay("amarelo", 5, 1);
		Player_Canvas.AtualizaBalas ("∞");
		yield return new WaitForSeconds (tempo);
		municaoInfinita = false;
	}

	public void BalasParaTodos (){
		for(int i = 1; i < guns.Length; i++) { // começa em 1 pra pular o id 0 que é a faca
			guns[i].AddBalas(1000);
		}
	}

	#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1
	public void canhoto (bool sim){
		Arma.canhoto = sim;
	}
	#else
	IEnumerator Vibra ( float esquerda ,   float direita ,   float duracao  ){
		GamePad.SetVibration(0, esquerda, direita);
		yield return new WaitForSeconds(duracao);
		GamePad.SetVibration(0, 0, 0);
	}
	#endif

	void OnTriggerEnter (Collider enemy){
		if(enemy.tag == "Enemy" && Input.GetJoystickNames().Length > 0 && Input.GetJoystickNames()[0] != "") {
			if(mov.alvo == null)
				mov.alvo = enemy.transform;
			
			if(!zumbis.Contains(enemy)) {
				zumbis.Add(enemy);
			}
		}
	}

	void OnTriggerExit (Collider enemy){
		if(zumbis.Contains(enemy)) {
			zumbis.Remove(enemy);
		}
		if(zumbis.ToArray().Length == 0) {
			mov.alvo = null;
		}
	}


	public static void ZumbiMorreu(Collider ultimoZumbiCol) {
		// Verifica as armas e completa missões
		if(eu.zumbis.Contains(ultimoZumbiCol))
			eu.zumbis.Remove(ultimoZumbiCol);

		Player_Score.AddScore(5);
	}
}
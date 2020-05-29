using UnityEngine;
using System.Collections;
//using Steamworks;

public class Player_Saude : MonoBehaviour {

	// Ints
	public static int vida; // public para ser mostrado na gui
	public static int vidaMax; // public para ser alterado pelo PowerUp

	// Floats
	//private float tempoDeVida = 0; // steam

	// Booleans
	/*private bool desabl30 = true; // steam
	private bool desabl10 = true;
	private bool desabl60 = true; */
	public static bool morto;
	private static bool podeReviver; // verifica se pode reviver mais uma vez
	public static bool monstro;
	public static bool invencivel;
	public static bool slowMotion;
	public static bool salvar;
	public static bool vibracao;
	public static bool quaseMorto; // publico pra todo mundo ver
	public static bool revivendo; // public para o pause mostrar o progresso

	// Referencias
	[SerializeField] private GameObject arma;
	[SerializeField] private GameObject pernas;
	[SerializeField] private GameObject touch;
	[SerializeField] private Camera cam;
	[SerializeField] private SpriteRenderer sprite;
	// efeitos
	// [SerializeField] private Vignetting vig;
	[SerializeField] private UnityStandardAssets.ImageEffects.BlurOptimized blur;
	private Sprite dead;
	// Efeitos sonoros
	[SerializeField] private AudioSource sound;
	private AudioClip ai;
	[SerializeField] private AudioClip deadSound;
	[SerializeField] private AudioClip reviviSound;
	[SerializeField] private AudioClip batimento;
	[SerializeField] private AudioClip[] oh;
	[SerializeField] private AudioClip[] painSounds;
	[SerializeField] private AudioClip[] masc;

	public static Player_Saude eu;

	void Awake (){
		eu = this;

		/////////////////// RESETA OS STATIC
		vida = 100;
		vidaMax = 100;

		// Booleans
		morto = monstro = invencivel = slowMotion = quaseMorto = revivendo = false;
		salvar = vibracao = podeReviver = true;
		//////////////////////// RESETOU OS STATIC

		// Recupera valores salvos
		PlayerPrefs.SetInt("jogatinas", PlayerPrefs.GetInt("jogatinas") + 1);
		if(PlayerPrefs.GetInt("salvar") != 0)
			salvar = false;
		
		//SteamUserStats.RequestCurrentStats ();
	}
		
	private void Start () {		
		// faz as referencias
		dead = GameObject.Find("Personagem").GetComponent<Player_Personagem>().personagens[7*PlayerPrefs.GetInt("personagem") + 5];

		// Recupera valores salvos
		if (Player.personagem == 0)
			vidaMax = 250;
		else if (Player.personagem == 3) {
			vida = 30;
			StartCoroutine (ViveWalter ());
		}
		
		if(PlayerPrefs.GetInt("vibracao") != 0)
			vibracao = false;

		Player_Canvas.AtualizaVida (vida, false);

		Spawner.EventRoundUp += RoundUp;
	} // Start

	private void Update (){
		if (morto) {
			
		} else {
			/*
			tempoDeVida += Time.deltaTime;
			if (tempoDeVida > 3600 && desabl60) {
				desabl60 = false;
				SteamUserStats.SetAchievement ("SURVIVOR_EASY");
				pause.sMissao (SteamUserStats.GetAchievementDisplayAttribute("SURVIVOR_EASY", "name"), SteamUserStats.GetAchievementDisplayAttribute("SURVIVOR_EASY", "desc"), true, 0);
			} else if (tempoDeVida > 1800 && desabl30) {
				desabl30 = false;
				SteamUserStats.SetAchievement ("SURVIVOR_MEDIUM");
				pause.sMissao (SteamUserStats.GetAchievementDisplayAttribute("SURVIVOR_MEDIUM", "name"), SteamUserStats.GetAchievementDisplayAttribute("SURVIVOR_MEDIUM", "desc"), true, 0);
			} else if (tempoDeVida > 600 && desabl10) {
				desabl10 = false;
				SteamUserStats.SetAchievement ("SURVIVOR_HARD");
				pause.sMissao (SteamUserStats.GetAchievementDisplayAttribute("SURVIVOR_HARD", "name"), SteamUserStats.GetAchievementDisplayAttribute("SURVIVOR_HARD", "desc"), true, 0);
			}*/
			
			if(monstro && transform.localScale.x < 8.5f) {
				transform.localScale += new Vector3(Player.time * 7, 0, Player.time * 7);
			}
			else if(transform.localScale.x > 4.986393f) {
				transform.localScale -= new Vector3(Player.time * 7, 0, Player.time * 7);
			}
		} // Se !morto
	} // Update

	private IEnumerator ViveWalter() {
		for (;;) {
			if (!invencivel && !Player.pausado && !quaseMorto && !morto && Spawner.podeSpawnar) {
				vida --;
				Player_Canvas.AtualizaVida (vida, true);
			}
			yield return new WaitForSeconds (1);
		}
	}

	public void aplicarDano (int dano){
		if(!morto && !quaseMorto && !invencivel && !revivendo) {
			// Gemido
			if(Player.personagem > 1) 		
				ai = masc[Random.Range(0, masc.Length)];
			else if(PlayerPrefs.GetInt("oh") != 0)
				ai = oh[Random.Range(0, oh.Length)];
			else 
				ai = painSounds[Random.Range(0, painSounds.Length)];
			sound.PlayOneShot(ai);
			// Aplica dano
			if(vibracao)
				Player_Vibration.Vibra(100);
			Camera_Flasher.eu.Flash("vermelho", 0.5f, 1);
			Camera_Flasher.eu.Zoom (-1, 1);
			
			if (vida - dano > 0) {
				vida -= dano;
				Player_Canvas.AtualizaVida (vida, true);
			} else {
				/*if(podeReviver) {
					if (Player_Score.score >= 300)
						StartCoroutine (Sangra ());
					else
						Morre ();
				}
				else {*/
					Morre();
				//}
			}
		}
	}

	private IEnumerator Sangra (){
		quaseMorto = true;
		Player_Canvas.QuaseFim ();
		sound.PlayOneShot(batimento, 1);
		blur.enabled = true;
#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1
	    touch.SetActive(false);
#else
        if (Game_Controles.indiceControle > 0)
            Camera_Cursor.pode = false;
#endif
		Player.pausado = true;
		Player_Movimento.eu.enabled = false;
		Player.timeScale = 0.3f;
		Camera_Flasher.eu.Flash("branco", 0.8f, 0.8f);
		Soundtrack.eu.SetPitch(0.7f);
		Soundtrack.eu.SetVolume(0.05f);
		yield return new WaitForSeconds(5);
		if(quaseMorto)
			Morre();
	}

	public IEnumerator Reviva (){
		quaseMorto = false;
		podeReviver = false;
		revivendo = true;
		Player.pausado = false;
		Spawner.AtualizaAlvos();
		Player.timeScale = Time.timeScale = 1;

		yield return new WaitForSeconds(3); // espera os zumbis sairem de perto

		Player_Canvas.Reviveu ();
		revivendo = false;		
		sound.PlayOneShot(reviviSound);
		blur.enabled = false;
		#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1
			touch.SetActive(true);
		#else
			if(Game_Controles.indiceControle > 0)
				Camera_Cursor.pode = false;
		#endif
		Camera_Flasher.eu.Zoom(0.5f, 3);
		Camera_Flasher.eu.Flash("branco", 0.9f, 0.5f);
		Player_Movimento.eu.enabled = true;
		vida = 20;
		Invencibilidade (5);
		Player_Score.score -= 300;
		yield return new WaitForSeconds(0.8f); // espera tocar o som que reviveu
		Soundtrack.eu.SetPitch(1);
		Soundtrack.eu.SetVolume(0.5f);
		Spawner.AtualizaAlvos();
	}

	private void Morre (){
		Player_Canvas.eu.FimDeJogo ();
		PlayerPrefs.SetInt("SalvoJogo", 0);
		morto = true;
		quaseMorto = false;
		revivendo = false;
		cam.orthographicSize = 6;
		Camera_Flasher.eu.Zoom (1, 0.1f);
		blur.enabled = true;
		Player.timeScale = Time.timeScale = 0.2f;
		#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_WEBPLAYER
        if (Game_Controles.indiceControle > 0)
            Camera_Cursor.pode = false;
		#else
		touch.SetActive(false);
		#endif
        arma.SetActive(false);
		sprite.sprite = dead;
		sprite.transform.position = new Vector3(sprite.transform.position.x, 0.15f, sprite.transform.position.z);
		pernas.SetActive(false);
		sound.PlayOneShot(deadSound);
		Player_Movimento.eu.enabled = false;
		Soundtrack.eu.SetPitch(0.7f);
		Soundtrack.eu.SetVolume(0.2f);
	}

	public static void AddVida (int tanto){
		Camera_Flasher.eu.Flash("branco", 0.1f, 0.6f);
		
		if(vida + tanto > vidaMax)
			vida += vidaMax - vida;
		else
			vida += tanto;

		if(!invencivel)
		    Player_Canvas.AtualizaVida (vida, false);
	}

	public static void Invencibilidade(int tempo) {
		eu.StopCoroutine ("InvencibilidadeMethod");
		eu.StartCoroutine (eu.InvencibilidadeMethod (tempo));
	}
	private IEnumerator InvencibilidadeMethod (int tempo){
		invencivel = true;
		//Camera.main.GetComponent<Vignetting>().blur = 1.5f;
		Player_Canvas.MostraPowerup((int)TipoDoPowerup.invenc, tempo);
		Player_Canvas.AtualizaVida (-1, false); // mostra o infinito na vida
		Camera_Flasher.eu.Flash("branco", 0.3f, 0.4f);
		Camera_Flasher.eu.Overlay("branco", tempo, 1);
		Camera_Flasher.eu.Zoom(0.4f, 2f);

		yield return new WaitForSeconds (tempo);
		invencivel = false;
		// Camera.main.GetComponent<Vignetting>().blur = 0;
		Player_Canvas.AtualizaVida (vida, false);
	}

	public static void SlowMotion(int tempo) {
		eu.StopCoroutine ("SlowMotionMethod");
		eu.StartCoroutine (eu.SlowMotionMethod (tempo));
	}
	private IEnumerator SlowMotionMethod (int tempo){
		slowMotion = true;
		// aplica o powerup
		Player.timeScale = 0.5f;
		Soundtrack.eu.SetPitch(0.7f);
		Player_Movimento.SuperVelocidade(5);
		// indica o powerup
		Player_Canvas.MostraPowerup((int)TipoDoPowerup.slowMotion, tempo);
		Camera_Flasher.eu.Flash("azul", 0.3f, 0.8f);
		Camera_Flasher.eu.Zoom(0.4f, 2f);
		Camera_Flasher.eu.Overlay("azul", tempo, 1);
		// Camera.main.GetComponent<Vignetting>().blur = 1.5f;
		// Camera.main.GetComponent<Vignetting>().blurSpread = 3;

		yield return new WaitForSeconds (tempo);

		slowMotion = false;
		Soundtrack.eu.SetPitch(1);
		Player.timeScale = 1;
		//Camera.main.GetComponent<Vignetting>().blur = 0;
		//Camera.main.GetComponent<Vignetting>().blurSpread = 1.5f;
	}


	public static void ViraMonstro() {
		eu.StopCoroutine ("ViraMonstroMethod");
		eu.StartCoroutine(eu.ViraMonstroMethod ());
	}

	private IEnumerator ViraMonstroMethod (){
		if (monstro)
			yield break;

		monstro = true;
		Player_Armas.eu.ViraMonstro(true);
		Invencibilidade(5);
		Player_Movimento.SuperVelocidade(5);
		Camera_Flasher.eu.Overlay("roxo", 5f, 1);
		Soundtrack.eu.SetPitch(1.2f);
		Camera_Flasher.eu.Flash("roxo", 0.3f, 0.8f);

		yield return new WaitForSeconds(5);

		monstro = false;
		Soundtrack.eu.SetPitch (1);
		Player_Armas.eu.ViraMonstro(false);
	}

	private void OnCollisionEnter (Collision col){
		if(monstro && col.gameObject.tag == "Enemy") {
			col.gameObject.GetComponent<Enemy_Saude>().aplicarDano(10000);
			if(vibracao)
				Player_Vibration.Vibra(10);
		}
	}

	private void OnTriggerEnter(Collider obj) {
		if (obj.tag == "Explosao" && !invencivel) {
			aplicarDano (20);
		}
	}
		
	public void RoundUp() {
		AddVida (20);
	}
}
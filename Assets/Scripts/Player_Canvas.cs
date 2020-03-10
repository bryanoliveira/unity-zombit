using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player_Canvas : MonoBehaviour {

	public static Player_Canvas eu;

	[SerializeField]
	private Text txtMensagemEstatica;
	[SerializeField]
	private Text txtVida;
	[SerializeField]
	private Text txtBalas;
	[SerializeField]
	private Text txtNomeArma;
	[SerializeField]
	private Text txtRound;
	[SerializeField]
	private Text txtAvisoMeio;
	[SerializeField]
	private Text txtPontos;
	[SerializeField]
	private Text txtObjetivo;
	[SerializeField]
	private Text txtDica;
	[SerializeField]
	private Text txtPontosPausa;
	[SerializeField]
	private Text txtPontosFim;
	[SerializeField]
	private Text txtPontosParaRecorde;
	[SerializeField]
	private Text txtTextoRecorde;
	[SerializeField]
	private Text txtNomePausa;
	[SerializeField]
	private Text txtNomeFim;
	[SerializeField]
	private Text txtNomeBoss;

	public InputField iptServidor;

	[SerializeField]
	private RectTransform fillVida;
	[SerializeField]
	private RectTransform fillVidaBoss;

	[SerializeField]
	private Image imgArmaAtual;
	[SerializeField]
	private Image imgMensagemEstatica;
	[SerializeField]
	private Image imgControleDica;
	[SerializeField]
	private Image[] imgPowerups;

	private Coroutine[] rotinasPowerups;

	[SerializeField]
	private Animator animAvisoMeio;
	[SerializeField]
	private Animator animFade;
    [SerializeField]
    private Animator animVidaFill;
    [SerializeField]
    private Animator animVidaBG;
    [SerializeField]
    private Animator animPontos;
    [SerializeField]
    private Animator animBalas;
    [SerializeField]
    private Animator animRound;
	[SerializeField]
	private Animator animObjetivos;

	[SerializeField]
	private GameObject viewEquipamentos;
	[SerializeField]
	private GameObject viewDicas;
	[SerializeField]
	private GameObject viewVidaBoss;
	[SerializeField]
	private GameObject telaPausa;
	[SerializeField]
	private GameObject telaJogo;
	[SerializeField]
	private GameObject telaFimDeJogo;
	[SerializeField]
	private GameObject telaQuaseFim;
	[SerializeField]
	private GameObject telaRevivendo;
	[SerializeField]
	private GameObject telaTrocaNome;
	[SerializeField]
	private GameObject prefabItemMissao;
	[SerializeField]
	private GameObject[] viewTutorial;
	[SerializeField]
	private GameObject[] prefabEquipamento;
	private GameObject[] instanciaEquipamento;

	[SerializeField]
	private UnityStandardAssets.ImageEffects.BlurOptimized blur;

    [SerializeField]
    private Font fontePadrao;
    [SerializeField]
    private Font fonteInfinito;
		
	private Vector3 posicaoInicial;

	public static bool jaMirou;

	private void Awake (){
		eu = this;
		jaMirou = false;
		instanciaEquipamento = new GameObject[prefabEquipamento.Length];
		rotinasPowerups = new Coroutine[imgPowerups.Length];
		Game_Controles.ControleMudou += ControleMudou;
	}
	private void Start() {
		StartCoroutine (EsperaEMostraTutorial ());
		posicaoInicial = transform.position;
	}
	private void OnApplicationFocus (bool focus){
		if (!focus) {
			if (!Player_Saude.morto) {
				Player.SalvaJogo ();
				if (!Player.pausado && !Player_Saude.quaseMorto)
					Pausa (true);
			}
		}
	}

	private void Update() {
		if (!Player_Saude.morto && !Player_Saude.quaseMorto && !Player_Saude.revivendo) {
			if (Game_Controles.start) {
				if (Player.pausado && !telaTrocaNome.activeSelf) {
					Pausa (false);
				} else {
					Pausa (true);
				}
			}
			if (!Player.pausado) {
				if (Game_Controles.nuke)
					Player_Score.SoltaNuke ();
				if (Game_Controles.dinamite)
					Player_Score.SoltaBomba ();
				
				if (Game_Controles.back) {
					animObjetivos.SetTrigger ("Mostra");
					Player_Objetivo.eu.CompletaDica (3);
				}
				if (Player.personagem == 4) {
					if (Mathf.Abs (Game_Controles.movimento_x) > 0.5f || Mathf.Abs (Game_Controles.movimento_y) > 0.5f) {
						Player.timeScale = 1;
						Soundtrack.eu.AddPitch (Player.time, 1);
					} else {
						Player.timeScale = 0.2f;
						Soundtrack.eu.AddPitch (-Player.time, 0.6f);
					}
				}
			}
			#if UNITY_ANDROID
			if(Input.touchCount > 3)
			pause();
			else if(Input.touchCount > 0) {
			foreach(Touch t in Input.touches) {
			if(t.phase == TouchPhase.Began) {
			if(t.position.y > 4 * Screen.height / 5) {
			int posicaao= Screen.width / 6;
			if(t.position.x < posicaao) {
			tab = !tab;
			}
			else if(t.position.x > 5 * posicaao) {
			tiro.trocar(-1);
			}
			else if(t.position.x > 2.5f * posicaao && t.position.x < 3.5f * posicaao) {
			pause();
			}
			}
			}
			}
			//Efeito de camera lenta ao tirar o dedo
			if(efeitoLento && !paused) {
			Time.timeScale = 1;
			if(musicas.pitch < 1) {
			musicas.pitch += Time.deltaTime;
			}
			}
			}
			else if(efeitoLento && !paused) {
			Time.timeScale = 0.2f;
			if(musicas.pitch > 0.6f)
			musicas.pitch -= Time.deltaTime;
			}
			#endif
		} else if (Player_Saude.morto) {
			if (Game_Controles.start) {
				Recomecar ();
			}
		} else if (Player_Saude.quaseMorto) {
			if (Game_Controles.acao) {
				Continua ();
			}
		}
	}

	// ------- TEXTO ESTÁTICO

	public static void MostraEstatico(string texto, Sprite imagem) {
		MostraEstatico (texto);
		eu.imgMensagemEstatica.gameObject.SetActive (true);
		//eu.imgMensagemEstatica.sprite = imagem;
	}
	public static void MostraEstatico(string texto) {
		eu.imgMensagemEstatica.gameObject.SetActive (false);
		eu.txtMensagemEstatica.text = texto;
		eu.txtMensagemEstatica.gameObject.SetActive (true);
	}
	public static void EscondeEstatico() {
		eu.txtMensagemEstatica.gameObject.SetActive (false);
		eu.imgMensagemEstatica.gameObject.SetActive (false);
	}

	// ------- BALAS

	public static void AtualizaBalas(int balas) {
        if (Player_Armas.municaoInfinita)
            return;

        eu.txtBalas.fontSize = 140;
        eu.txtBalas.font = eu.fontePadrao;
        if (balas >= 0)
			eu.txtBalas.text = balas.ToString ();
		else
			eu.txtBalas.text = "";

        eu.animBalas.SetTrigger("Destaca");
	}
	public static void AtualizaBalas(string balas) {
        eu.txtBalas.fontSize = 90;
        eu.txtBalas.font = eu.fonteInfinito;
        eu.txtBalas.text = balas;
        eu.animBalas.SetTrigger("Destaca");
    }

	// ------- ARMAS

	public static void AtualizaArma(Sprite img, string nome) {
		eu.imgArmaAtual.sprite = img;
		eu.txtNomeArma.text = nome;

		eu.viewEquipamentos.SetActive (false);
		eu.txtNomeArma.gameObject.SetActive (true);

		eu.StartCoroutine(eu.EscondeNomeVoltaEquipamentosRoutine());
	}
	private IEnumerator EscondeNomeVoltaEquipamentosRoutine() {
		yield return new WaitForSeconds (3);
		eu.viewEquipamentos.SetActive (true);
		eu.txtNomeArma.gameObject.SetActive (false);
	}

	// ------- VIDA

	public static void AtualizaVida(int vida, bool dano) {
		if (vida >= 0) {
            eu.txtVida.text = vida.ToString ();
			eu.fillVida.sizeDelta = new Vector2 (200 * ((float) vida / Player_Saude.vidaMax), 40);
            if (dano)
                eu.animVidaBG.SetTrigger("Destaca");
            else
                eu.animVidaFill.SetTrigger("Destaca");
		} else {
            eu.animVidaFill.SetTrigger("Destaca");
            eu.txtVida.text = "";
			eu.fillVida.sizeDelta = new Vector2 (200, 40);
		}
	}

	// ------- EQUIPAMENTOS

	public static void AtualizaEquipamentos(int qual, int quantidade) {
		if (quantidade > 0 && eu.instanciaEquipamento [qual] == null) {
			eu.instanciaEquipamento [qual] = GameObject.Instantiate (eu.prefabEquipamento [qual], eu.viewEquipamentos.transform);
			RectTransform tmpRect = eu.instanciaEquipamento [qual].GetComponent<RectTransform> ();
			tmpRect.sizeDelta = new Vector2 (50, 55);
			tmpRect.localScale = new Vector3(1, 1, 1);
		} else {
			if(eu.instanciaEquipamento[qual] != null)
				Destroy(eu.instanciaEquipamento [qual]);
			return;
		}
	}

	// ------- POWERUPS

	public static void MostraPowerup(int qual, int tempo) {
		eu.MostraPowerupMethod (qual, tempo);
	}
	private void MostraPowerupMethod (int qual, int tempo) {
		if (rotinasPowerups [qual] != null) {
			StopCoroutine (rotinasPowerups [qual]);
		}
		imgPowerups [qual].gameObject.SetActive (true);
		rotinasPowerups [qual] = StartCoroutine (EscondePowerupRoutine (qual, tempo));
	}
	private IEnumerator EscondePowerupRoutine(int qual, int tempo) {
		yield return new WaitForSeconds (tempo - 2);
        imgPowerups[qual].GetComponent<Animator>().enabled = false;
		Color tmpColor = imgPowerups [qual].color; // vai alterar só o alpha, pode deixar o resto de fora
		while (imgPowerups [qual].color.a > 0) {
			tmpColor.a -= 0.02f;
			imgPowerups [qual].color = tmpColor;
			yield return new WaitForSeconds (0.02f);
		}
		tmpColor.a = 1;
		imgPowerups [qual].color = tmpColor;
		imgPowerups [qual].gameObject.SetActive (false);
		rotinasPowerups [qual] = null;
	}

	// ------- ROUNDS

	public static void AtualizaRound(int round) {
		eu.txtRound.text = round.ToString ();
		AvisaNoMeio ("ROUND " + round);
        eu.animRound.SetTrigger("Destaca");
		Camera_Flasher.eu.Flash ("branco", 0.8f, 0.5f);
	}

	// ------- AVISOS

	public static void AvisaNoMeio(string texto) {
		eu.txtAvisoMeio.text = texto;
		eu.animAvisoMeio.SetTrigger ("avisa");
	}

	public static void AvisaNoMeio(string texto, float timeScale) {
		eu.StartCoroutine (eu.VoltaScale());
		Time.timeScale = timeScale;
		eu.txtAvisoMeio.text = texto;
		eu.animAvisoMeio.SetTrigger ("avisa");
	}
	private IEnumerator VoltaScale() {
		yield return new WaitForSeconds (1.5f);
		if (!Player.pausado) {
			Time.timeScale = 1;
		}
	}

	// ------- PONTOS

	public static void AtualizaPontos(int pontos) {
		eu.txtPontos.text = pontos.ToString ();
        eu.animPontos.SetTrigger("Destaca");
	}

	// ------- MENU DE PAUSA
	public static void Pausa(bool sim) {
		if (sim) {
			eu.telaJogo.SetActive (false);
			eu.telaPausa.SetActive (true);
			Player.pausado = true;

			Player.timeScale = 0;
			eu.blur.enabled = true;
			Soundtrack.eu.SetPitch (0.9f);
			eu.txtPontosPausa.text = Player_Score.score.ToString();
			if (AudioListener.volume > 0)
				AudioListener.volume = 0.4f;
			#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1
		joystick.SetActive(false);
			#else
			Camera_Cursor.pode = true;
			#endif
			Player_Objetivo.eu.CompletaDica (4);
			Player.SalvaJogo ();
		} else {
			eu.telaJogo.SetActive (true);
			eu.telaPausa.SetActive (false);
			Player.pausado = false;

			eu.blur.enabled = false;
			#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1
			if(mov.joystick != 4)
			joystick.SetActive(true);
			#else
			if (Game_Controles.indiceControle > 0)
				Camera_Cursor.pode = false;
			#endif

			if (!Player_Saude.slowMotion) {
				Player.timeScale = 1;
				Soundtrack.eu.SetPitch(1);
			} else {
				Player.timeScale = 0.5f;
				Soundtrack.eu.SetPitch(0.7f);
			}
			//soundtrack.cVolume(0.5f);
			if (AudioListener.volume > 0)
				AudioListener.volume = 1;
		}
	}
	public void SwitchPausa() {
		if (Player.pausado)
			Pausa (false);
		else
			Pausa (true);
	}

	public void Liga(GameObject oque) {
		oque.SetActive(true);
	}
	public void Desliga(GameObject oque) {
		oque.SetActive (false);
	}

	public void Sair() {
		Player.SalvaJogo ();
		animFade.SetTrigger ("FadeOut");
		StartCoroutine (LoadRoutine ("Menu"));
	}
	private IEnumerator LoadRoutine(string cena) {
		yield return new WaitForSeconds (2);
		SceneManager.LoadScene (cena);
	}

	// ------ FIM DE JOGO

	public static void QuaseFim() {
		Pausa (false);
		eu.blur.enabled = true;
		if (Player.meuRecordeAnterior - Player_Score.score > 0) {
			eu.txtTextoRecorde.text = "recorde será batido em";
		} else {
			eu.txtTextoRecorde.text = "recorde batido em";
		}
		eu.txtPontosParaRecorde.text = Mathf.Abs(Player.meuRecordeAnterior - Player_Score.score).ToString();
		eu.telaQuaseFim.SetActive (true);
		eu.telaJogo.SetActive (false);
	}
	public void Continua() {
		Player_Saude.eu.Reviva ();
		telaQuaseFim.SetActive (false);
		telaRevivendo.SetActive (true);
	}
	public static void Reviveu() {
        eu.blur.enabled = false;
        eu.telaRevivendo.SetActive (false);
		eu.telaJogo.SetActive (true);
	}
		
	public void FimDeJogo() {
		Pausa (false);
		AvisaNoMeio ("Game Over");
		Player.eu.MostraMeuRecorde (1);
		Camera_Flasher.eu.Flash ("branco", 0.8f, 10f);
		txtPontosFim.text = Player_Score.score.ToString ();
		telaJogo.SetActive (false);
		telaQuaseFim.SetActive (false);
        telaRevivendo.SetActive(false);
		blur.enabled = true;
		StartCoroutine (FimDeJogoRoutine ()); 
	}
	private IEnumerator FimDeJogoRoutine() {
		yield return new WaitForSeconds (1.5f);
		telaFimDeJogo.SetActive (true);
		Player.timeScale = 0.004f;
		Time.timeScale = 1;
	}

	public void Recomecar() {
		Player.nome = txtNomeFim.text.ToLower();
		PlayerPrefs.SetString ("nome", Player.nome);
		Player.eu.SalvaRecordeLocal ();
		Soundtrack.eu.SetPitch (1);
		Game_Controles.ResetaControleMudou ();

		if (Player.personagem < 3) {
			PlayerPrefs.SetInt ("personagem", Player.personagem + 1);
		} else {
			PlayerPrefs.SetInt ("personagem", 0);
		}

		animFade.SetTrigger ("FadeOut");
		StartCoroutine (LoadRoutine ("Game"));
	}

	public void ControleMudou() {
		
	}

	// ------- OBJETIVOS
	public static void AtualizaObjetivo(string objetivo) {
		eu.txtObjetivo.text = objetivo;
		eu.animObjetivos.SetTrigger("Mostra");
	}
	public static void AtualizaDica(string dica) {
		eu.imgControleDica.gameObject.SetActive (false);
		if (dica == "") {
			eu.viewDicas.SetActive (false);
		} else {
			eu.viewDicas.SetActive (true);
			eu.txtDica.text = dica;
		}
	}
	public static void AtualizaDica(string dica, Sprite controle) {
		AtualizaDica (dica);
		eu.imgControleDica.gameObject.SetActive (true);
		eu.imgControleDica.sprite = controle;
	}

	// ------- TUTORIAL
	private IEnumerator EsperaEMostraTutorial() {
		yield return new WaitForSeconds (1.5f);
		for(int i = 0; i < viewTutorial.Length; i++) {
			yield return new WaitForSeconds (1);
			viewTutorial[i].SetActive (true);
			float tempoObjetivo = 0;
			while (tempoObjetivo < 0.3f) {
				if (i == 0) { // andar
					if (transform.position != posicaoInicial) {
						tempoObjetivo = 1;
						continue;
					}
					if (Game_Controles.movimento_x != 0 || Game_Controles.movimento_y != 0) {
						tempoObjetivo += Time.deltaTime;
					} else {
						tempoObjetivo = 0;
					}
				} else if (i == 1) { // mirar
					if (jaMirou) {
						tempoObjetivo = 1;
						continue;
					}
					if (Game_Controles.indiceControle == 0) {
						tempoObjetivo = 1;
					}
					else if (Game_Controles.rotacao_x != 0 || Game_Controles.rotacao_y != 0) {
						tempoObjetivo += Time.deltaTime;
					} else {
						tempoObjetivo = 0;
					}
				} else if (i == 2) { // atirar
					if (Game_Controles.fire) {
						break;
					}
				}
				yield return new WaitForSeconds (0.02f);
			}
			viewTutorial[i].SetActive (false);
		}
		Spawner.podeSpawnar = true;
		Player_Armas.municaoInfinita = false;
	}

	// -------- NOME
	public static void AtualizaNome(string nome) {
		eu.txtNomePausa.text = nome;
		eu.txtNomeFim.text = nome;
	}

	// ------- BOSS
	public static void MostraVidaBoss(bool sim, string nome) {
		if (sim) {
			eu.viewVidaBoss.SetActive (true);
			eu.txtNomeBoss.text = nome;
		} else {
			eu.viewVidaBoss.SetActive (false);
		}
	}
	public static void AtualizaVidaBoss(int vida) {																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																																								
		eu.fillVidaBoss.sizeDelta = new Vector2 ((vida / 2500f) * 523, 17.7f);
	}

	// ------- RECORDE
	public void AtualizaServidor() {
		Player.servidor = iptServidor.text;
		PlayerPrefs.SetString ("servidor", Player.servidor);
	}
}
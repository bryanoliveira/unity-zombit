using UnityEngine;
using System.Collections;

public class Camera_Flasher : MonoBehaviour {

	public static bool pode;
	// Flash
	private float opacidade = 0.0f;
	private float timer = 0.0f;
	private float velocidade = 1.0f;
	private float rC = 1.0f;
	private float gC = 1.0f;
	private float bC = 1.0f;
	// Overlay
	private float oopacidade = 0.0f;
	private float otimer = 0.0f;
	private float ovelocidade = 1.0f;
	private float orC = 1.0f;
	private float ogC = 1.0f;
	private float obC = 1.0f;
	// Zoom
	private float campoVisao = 13.39f;
	private float zVel = 1;
	// Outros
	[SerializeField] private Texture2D branco;
	[SerializeField] private Texture2D overlayTex;
	[SerializeField] private Camera cam;
	// Musica
	[SerializeField] private Vignetting vig; // só pro chromatic
	private AudioSource music;
	//
	private float rmsValue;   // nivel dos sons
	private float[] samples; // samples

	public static Camera_Flasher eu;

	private void Awake() {
		eu = this;
		pode = true;
	}

    private void Start() {
        useGUILayout = false;
        music = GameObject.FindWithTag("SoundTrack").GetComponent<AudioSource>();
		samples = new float[1024];

        if (PlayerPrefs.GetInt("flash") == 1){
            pode = false;
        }
		
		if(PlayerPrefs.GetInt("personagem") == 2) {
			campoVisao = 17f;
		}
	}

	private void Update (){
		if(pode) {
			// flash
			if(timer > 0)
				timer -= Time.deltaTime * velocidade;
			opacidade = timer;
			// overlay
			if(otimer > 0)
				otimer -= Time.deltaTime * ovelocidade;
			oopacidade = otimer;
			// glitch
			if(!Player_Saude.morto && music.volume > 0) {
				GetVolume();
			}

			// zoom
			if (!Player_Saude.morto && !Player.pausado && !Player_Saude.quaseMorto) {
				if (cam.orthographicSize < campoVisao) {
					cam.orthographicSize += Time.deltaTime * zVel;
				} else if (cam.orthographicSize > campoVisao) {
					cam.orthographicSize -= Time.deltaTime * zVel;
				}
				if (Game_Controles.zoom && cam.orthographicSize < campoVisao + 2) {
					Zoom (-1.5f, 3);
				}
			}
		}
	}
	private void OnGUI (){
		if(!Player.pausado && pode) {		
			GUI.color = new Color(rC,gC,bC,opacidade);
			GUI.DrawTexture( new Rect(0,0,Screen.width,Screen.height), branco);
			
			if(!Player_Saude.morto && !Player_Saude.quaseMorto) {
				GUI.color = new Color(orC,ogC,obC,oopacidade);
				GUI.DrawTexture( new Rect(0,0,Screen.width,Screen.height), overlayTex);

				GUI.color = new Color(1, 1, 1, rmsValue - 0.45f);
				GUI.DrawTexture( new Rect(0,0,Screen.width,Screen.height), branco);
			}
		}
	}

	public void Flash(string cor, float op, float vel){
		if (QualitySettings.GetQualityLevel() > 1) {
			timer = op;
			velocidade = vel;
			if(cor == "branco") {
				rC = 1.0f;
				gC = 1.0f;
				bC = 1.0f;
			} else if(cor == "vermelho") {
				rC = 1.0f;
				gC = 0;
				bC = 0;
			} else if(cor == "azul") {
				rC = 0;
				gC = 0.61f;
				bC = 1;
			} else if(cor == "amarelo") {
				rC = 1;
				gC = 1;
				bC = 0;
			} else if(cor == "roxo") {
				rC = 1.0f;
				gC = 0;
				bC = 1.0f;
			} else if(cor == "preto") {
				rC = 0;
				gC = 0;
				bC = 0;
			}
		}
	}

	public void Overlay (string ocor, float oop, float ovel){
		if (QualitySettings.GetQualityLevel() > 1) {
			otimer = oop;
			ovelocidade = ovel;
			if(ocor == "branco") {
				orC = 1.0f;
				ogC = 1.0f;
				obC = 1.0f;
			} else if(ocor == "verde") {
				orC = 0;
				ogC = 1;
				obC = 0;
			} else if(ocor == "azul") {
				orC = 0;
				ogC = 0.61f;
				obC = 1;
			} else if(ocor == "amarelo") {
				orC = 1;
				ogC = 1;
				obC = 0;
			} else if(ocor == "roxo") {
				orC = 1.0f;
				ogC = 0;
				obC = 1.0f;
			} else if(ocor == "vermelho") {
				orC = 1.0f;
				ogC = 0;
				obC = 0;
			} else if(ocor == "preto") {
				orC = 0;
				ogC = 0;
				obC = 0;
			}
		}
	}

	public void Zoom (float quanto, float velocidade){
		if (QualitySettings.GetQualityLevel() > 1) {
			if(cam.orthographicSize > 7) {
				Player_Objetivo.eu.CompletaDica (2);
				cam.orthographicSize -= quanto;
				zVel = velocidade;
			}
		}
	}

	private void GetVolume (){
		music.GetOutputData(samples, 0); // preenche o array
		float fator;

		if(Player_Score.chain < 4)
			fator = Player_Score.chain * 0.01f;
		else
			fator = 3 * 0.1f;
		
		float soma = 0;
		for(int i = 0; i < 1024; i++){
			soma += samples[i]*samples[i]; // eleva ao quadrado
		}
		rmsValue = Mathf.Sqrt(soma/1024) + fator; // faz a media

		vig.chromaticAberration = rmsValue*50;
	}

	public void SetVisao(float quanto) {
		campoVisao = quanto;
	}
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {
	public AudioSource sound;

	public static Player eu;

	public static int personagem;
	public static int meuRecordeAnterior;

	public static float time;
	public static float timeScale;

	public static bool pausado;

	[SerializeField]
	private AudioClip somRoundUp;
	[SerializeField] 
	private AudioClip[] fxS;

	public static string nome;
	public static string servidor;

	[SerializeField]
	private Recorde_Referencias[] meuRecorde;

	[SerializeField]
	private Transform viewRecordesPausa;

	[SerializeField]
	private GameObject prefabItemRecorde;

	private void Awake() {
		personagem = PlayerPrefs.GetInt("personagem");

		// reseta os static
		timeScale = Time.timeScale = 1;
		pausado = false;
		/*
		if(personagem < pause.persIcones.Length - 1) {
			PlayerPrefs.SetInt("personagem", personagem + 1);
			if(personagem + 1 < 2)
				PlayerPrefs.SetInt("psexo", 0);
			else
				PlayerPrefs.SetInt("psexo", 1);
		}
		else {
			PlayerPrefs.SetInt("personagem", 0);
			PlayerPrefs.SetInt("psexo", 0);
		}*/

		eu = this;

		Spawner.EventRoundUp += RoundUp;
	}

	private void Start() {
		if (PlayerPrefs.GetInt ("jogoSalvo") == 1) {
			CarregaJogo ();
		}
		StartCoroutine (PlayFX ());

		// Gerencia o nome
		if(PlayerPrefs.HasKey("nome"))
			nome = PlayerPrefs.GetString("nome");
		else {
			if(SystemInfo.deviceName.Contains("-PC")) {
				string[] meuNome;
				meuNome = SystemInfo.deviceName.Split("-PC"[0]);
				nome = meuNome[0].ToLower();
			}
			else {
				nome = SystemInfo.deviceName.ToLower();
			}
			PlayerPrefs.SetString ("nome", nome);
		}
		Player_Canvas.AtualizaNome (nome);

		if (PlayerPrefs.HasKey ("servidor")) {
			Player.servidor = PlayerPrefs.GetString ("servidor");
		} else {
			Player.servidor = "localhost";
			PlayerPrefs.SetString ("servidor", Player.servidor);
		}
		Player_Canvas.eu.iptServidor.text = Player.servidor;

		MostraRecordesLocais (viewRecordesPausa);
	}

	private void Update() {
		time = Time.deltaTime * timeScale;
	}

	public static void SalvaJogo (){	
		
	}
	public static void CarregaJogo (){
		
	}

	public void RoundUp() {
		sound.PlayOneShot (somRoundUp, 1);
	}

	private IEnumerator PlayFX (){
		sound.volume = 0.8f;
		yield return new WaitForSeconds(0.3f);
		sound.PlayOneShot(fxS[Random.Range(0, fxS.Length)]);
		yield return new WaitForSeconds(1);
		sound.volume = 1;
	}

	public void SalvaRecordeLocal() {
		if (PlayerPrefs.GetInt ("recorde") < Player_Score.score) {
			PlayerPrefs.SetInt ("recorde", Player_Score.score);
			StartCoroutine (SalvaOnline (Player_Score.score));
		}
	}
	private IEnumerator SalvaOnline(int recorde) {
		WWW salvando = new WWW (servidor + "/zombit/?nome=" + nome + "&id=" + SystemInfo.deviceUniqueIdentifier + "&recorde=" + recorde);
		yield return salvando;

		if(salvando.error != null || salvando.text.Contains ("erro")) {
			Debug.Log ("Ocorreu um erro ao salvar recorde");
		}
		else {
			Debug.Log ("Recorde salvo com sucesso");
		}
	}

	private void MostraRecordesLocais(Transform pai) {
		// reseta a view
		for (int i = 0; i < pai.childCount; i++) {
			Destroy (pai.GetChild (i).gameObject);
		}

		StartCoroutine (PegaRecordesOnline (pai));
	}
	private IEnumerator PegaRecordesOnline(Transform pai) {
		Debug.Log (servidor);
		WWW busca = new WWW (servidor + "/zombit/");
		yield return busca;

		string[] recordes = null;

		if (!string.IsNullOrEmpty(busca.error)) {
			Debug.Log ("Ocorreu um erro ao buscar recordes: " + busca.error);
			yield break;
		} else {
			string temp = busca.text;
			recordes = temp.Split ("/"[0]);
		}
		for (int i = 0; i < recordes.Length; i++) {
			string[] umRecorde = recordes [i].Split ("#"[0]);

			if (umRecorde[0] == nome.ToLower()) {
				if (pai.name == "Pausa") {
					meuRecorde [0].posicao.text = (i + 1).ToString ();
					meuRecorde [0].pontos.text = umRecorde[1];
				} else {
					meuRecorde [1].posicao.text = (i + 1).ToString ();
					meuRecorde [1].pontos.text = umRecorde[1];
				}
			} else {
				AdicionaRecordeView (pai, umRecorde[0], umRecorde[1], (i + 1));
			}
		}
	}

	private void AdicionaRecordeView(Transform view, string nome, string recorde, int posicao) {
		// cria o objeto da tabela
		GameObject item = Instantiate(prefabItemRecorde);
		// coloca na lista pra ser exibido
		item.transform.SetParent(view);
		// conserta a escala
		item.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
		// será utilizado 3 vezes, melhor criar a referencia
		Recorde_Referencias refItem = item.GetComponent<Recorde_Referencias>();
		refItem.nome.text = nome;
		refItem.pontos.text = recorde;
		refItem.posicao.text = posicao.ToString();
	}

	public void MostraMeuRecorde(int onde) {
		meuRecordeAnterior = PlayerPrefs.GetInt ("recorde");
		meuRecorde [onde].pontos.text = meuRecordeAnterior.ToString ();
	}

	public void Toca(AudioClip som) {
		sound.PlayOneShot (som, 1);
	}

	public void ApagaSaves() {
		PlayerPrefs.DeleteAll();
	}
}
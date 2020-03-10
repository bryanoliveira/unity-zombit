using UnityEngine;
using System.Collections;

public class Player_Score : MonoBehaviour {

	// Booleans
	public static bool habilScore;
	public static bool dobro;
	// Floats e ints
	public static int score;
	public static int cash;
	public static int chain;
	public static int bombas;
	public static int nukes;
	private static float tempo;

	// Efeitos sonoros
	[SerializeField] private AudioClip semCash;
	[SerializeField] private AudioClip comprou;
	[SerializeField] private AudioClip pegou;
	public static Player_Score eu;

	[SerializeField] private GameObject nukeObj;
	[SerializeField] private GameObject bombaObj;

	private void Awake() {
		eu = this;
		habilScore = false;
		dobro = false;
		score = 0;
		cash = 0;
		chain = 0;
		bombas = 0;
		nukes = 0;
		tempo = 0;
	}

	private void Update (){
		if (tempo > 0) {
			tempo -= Player.time;
			if (tempo < 0.1f) {
				chain = 0;
				tempo = 0;
			}
		}
	}

	public static void AddScore(int tanto) {
		tempo = 1;
		chain++;
		int bonus = 1;

		if (Player.personagem == 3 || habilScore) {
			bonus = Random.Range (0, 6);
			Player_Saude.AddVida (3);
		}
		if(dobro) 
			tanto *= 2;

		if(chain > 2) {
			score += tanto * chain * bonus;
			cash += tanto * chain * bonus;
			//pause.indica("Serial x" + chain, 2, 20);
			if(chain == 10)
				Player_Saude.ViraMonstro();
		} 
		else {
			score += tanto * bonus;
			cash += tanto * bonus;
		}

		Player_Canvas.AtualizaPontos (cash);
	}

	public static bool Comprar (int preco){
		if(cash - preco >= 0) {
			Player.eu.sound.PlayOneShot(eu.comprou);
			cash -= preco;
			Player_Canvas.AtualizaPontos (cash);
			return true;
		} else {
			Player.eu.sound.PlayOneShot(eu.semCash);
			return false;
		}
	}

	public IEnumerator Dobro (int tempo){
		dobro = true;
		Player_Canvas.MostraPowerup((int)TipoDoPowerup.dobro, tempo);
		Camera_Flasher.eu.Flash("preto", 0.4f, 0.8f);
		Camera_Flasher.eu.Overlay("preto", 5, 1);
		yield return new WaitForSeconds (tempo);
		dobro = false;
	}
	public void PegaBomba (){
		Player.eu.sound.PlayOneShot(pegou);
		bombas++;
		Player_Objetivo.eu.MostraDica (1);
		if(bombas > 2) {
			nukes++;
			bombas = 0;
			Player_Canvas.AtualizaEquipamentos (1, nukes);
			Player_Objetivo.eu.MostraDica (2);
		}
		Player_Canvas.AtualizaEquipamentos (0, bombas);
		Player_Objetivo.eu.CompletaDicaEspecifica (0);
	}

	public static void SoltaNuke (){
		eu.SoltaNukeMethod ();
	}
	private void SoltaNukeMethod() {
		if(nukes > 0) {
			nukes--;
			Instantiate(nukeObj, new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z), transform.rotation);
			Player_Canvas.AtualizaEquipamentos (1, nukes);
			Player_Objetivo.eu.CompletaDicaEspecifica (2);
		}
	}

	public static void SoltaBomba (){
		eu.SoltaNukeMethod ();
	}
	private void SoltaBombaMethod() {
		if(bombas > 0) {
			bombas--;
			GameObject b = Instantiate(bombaObj, new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z), transform.rotation) as GameObject;
			b.GetComponent<Enemy_Bomba>().boa = false;
			Player_Canvas.AtualizaEquipamentos (0, bombas);
			Player_Objetivo.eu.CompletaDicaEspecifica (1);
		}
	}
}
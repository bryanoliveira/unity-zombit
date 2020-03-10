using UnityEngine;
using System.Collections;

public class Player_Objetivo : MonoBehaviour {

	[System.Serializable]
	public class Objetivo {
		public string titulo;
		public string textoCompleto;
		public string dica;
	}
	[SerializeField] private Objetivo[] objetivos; // todas as missoes criadas

	[SerializeField] private string[] dicaAleatoria;
	[SerializeField] private string[] dicaEspecifica;
	private string dicaAtual;

	[SerializeField] private Sprite[] sprControleDica;
	[SerializeField] private Sprite[] sprDicaEspecifica;

	private bool[] dicaCompletada;
	private bool[] objetivosCompletados;
	private bool[] dicaEspecificaCompletada;

	public static int objetivoAtual = 0;
	private int indiceDica = 0;

	private float tempoSemDica = 0;

	public static Player_Objetivo eu;

	private void Awake() {
		eu = this;
		objetivoAtual = 0;
		objetivosCompletados = new bool[objetivos.Length];
		dicaCompletada = new bool[dicaAleatoria.Length];
		dicaEspecificaCompletada = new bool[dicaEspecifica.Length];
	}

	private void Start (){
		Player_Canvas.AtualizaObjetivo (objetivos [0].titulo);
		Player_Canvas.AtualizaDica (objetivos [0].dica);
		dicaAtual = objetivos [0].dica;
	}

	private void Update() {
		tempoSemDica += Time.deltaTime;
		if (tempoSemDica > 15) {
			if (objetivoAtual < objetivos.Length && dicaAtual != objetivos [objetivoAtual].dica) {
				dicaAtual = objetivos [objetivoAtual].dica;
				Player_Canvas.AtualizaDica (dicaAtual);
			} else if(indiceDica < dicaAleatoria.Length) {
				DicaAleatoria();
			}
		}
	}

	public void CompletaObjetivo(int qual) {
		Player_Canvas.AvisaNoMeio (objetivos [qual].textoCompleto, 0.5f);
		objetivosCompletados [qual] = true;

		if (dicaAtual == objetivos[objetivoAtual].titulo) {
			while (objetivoAtual < objetivos.Length) {
				if (objetivosCompletados [objetivoAtual]) {
					objetivoAtual++;
				} else {
					break;
				}
			}
			if (objetivoAtual == objetivos.Length) {
				dicaAtual = "Cidade conquistada! Sobreviva!";
				Player_Canvas.AtualizaObjetivo ("Cidade conquistada! Sobreviva!");
				Player_Canvas.AvisaNoMeio (objetivos [qual].textoCompleto, 0.5f);
			} else {
				dicaAtual = objetivos [objetivoAtual].titulo;
				Player_Canvas.AtualizaObjetivo (dicaAtual);
			}
		} else {
			objetivoAtual++;
		}
	}

	public void CompletaDica(int qual) {
		dicaCompletada [qual] = true;

		if (dicaAtual == dicaAleatoria [qual]) {
			while (indiceDica < dicaAleatoria.Length) {
				if (dicaCompletada [indiceDica]) {
					indiceDica++;
				} else {
					break;
				}
			}
			if (indiceDica >= dicaAleatoria.Length) {
				Player_Canvas.AtualizaDica ("");
				dicaAtual = "";
			} else {
				dicaAtual = dicaAleatoria [indiceDica];
				Player_Canvas.AtualizaDica (dicaAtual, sprControleDica [indiceDica]);
				tempoSemDica = 0;
			}
		} else {
			indiceDica++;
		}
	}
	private void DicaAleatoria() {
		if (indiceDica >= dicaAleatoria.Length) {
			dicaAtual = "";
			Player_Canvas.AtualizaDica ("");
		} else {
			dicaAtual = dicaAleatoria[indiceDica];
			Player_Canvas.AtualizaDica (dicaAtual, sprControleDica[indiceDica]);
			tempoSemDica = 0;
		}
	}

	public void MostraDica(int qual) {
		if (dicaEspecificaCompletada [qual])
			return;
		dicaAtual = dicaEspecifica [qual];
		Player_Canvas.AtualizaDica (dicaAtual, sprDicaEspecifica[qual]);
		tempoSemDica = 0;
	}
	public void CompletaDicaEspecifica(int qual) {
		dicaEspecificaCompletada [qual] = true;

		if (dicaAtual == dicaEspecifica [qual]) {
			DicaAleatoria ();
		}
	}
}
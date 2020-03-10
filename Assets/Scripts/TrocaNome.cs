using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TrocaNome : MonoBehaviour {

	[SerializeField]
	private Text txtNome;
	[SerializeField]
	private Text txtLetra;

	[SerializeField]
	private GameObject telaResumo;
	[SerializeField]
	private GameObject telaFim;
	[SerializeField]
	private GameObject telaPausa;

	private char letraAtual = (char) 97;

	private void Start() {
		txtNome.text = Player.nome;
	}

	private void Update() {
		if (Game_Controles.upC) {
			SobeLetra ();
		} else if (Game_Controles.downC) {
			DesceLetra ();
		}
		if (Game_Controles.inserir) {
			Inserir ();
		} else if (Game_Controles.acao) {
			Apagar ();
		} else if (!Player_Saude.morto) {
	    	if (Game_Controles.start) {
				Salva ();
			} else if (Game_Controles.voltar) {
				Cancela ();
			}
		}
	}

	private void SobeLetra() {
		letraAtual++;
		if (letraAtual == 123) {
			letraAtual = (char) 32;
		} else if (letraAtual == 33) {
			letraAtual = (char) 48;
		} else if (letraAtual == 58) {
			letraAtual = (char) 97;
		}
		txtLetra.text = letraAtual.ToString();
	}

	private void DesceLetra() {
		letraAtual--;
		if (letraAtual == 96) {
			letraAtual = (char) 57;
		} else if (letraAtual == 47) {
			letraAtual = (char) 32;
		} else if (letraAtual == 31) {
			letraAtual = (char) 122;
		}
		txtLetra.text = letraAtual.ToString();
	}

	public void Salva() {
		Player.nome = txtLetra.text;
		PlayerPrefs.SetString ("nome", txtNome.text.ToLower());
		Player_Canvas.AtualizaNome (txtNome.text);
		if (Player_Saude.morto) {
			Player_Canvas.eu.Liga (telaFim);
			Player_Canvas.eu.Desliga (telaPausa);
		} else {
			Player_Canvas.eu.Liga (telaResumo);
			Player_Canvas.eu.Desliga (gameObject);
		}
	}

	public void Cancela() {
		if (Player_Saude.morto) {
			Player_Canvas.eu.Liga (telaFim);
			Player_Canvas.eu.Desliga (telaPausa);
		} else {
			Player_Canvas.eu.Liga (telaResumo);
			Player_Canvas.eu.Desliga (gameObject);
		}
	}

	public void Inserir() {
		txtNome.text += letraAtual;
	}

	public void Apagar() {
		if(txtNome.text.Length > 0)
			txtNome.text = txtNome.text.Remove (txtNome.text.Length - 1);
	}

}

using UnityEngine;
using System.Collections;

public class Player_GerenciadorDePersonagem : MonoBehaviour {
	[SerializeField] private SpriteRenderer personagem;
	[SerializeField] private Player_Personagem osPersonagens;

	enum Tipo {faca, pistola, metralhadora, dual, braco, monstro};
	[SerializeField] private Tipo tipo;

	void Start (){
		Atualiza ();
	}

	public void Atualiza() {
		if(tipo == Tipo.dual) {
			personagem.sprite = osPersonagens.personagens[7*Player.personagem]; // QuantidadeDeSpritesPorPersonagens * PersonagemAtual + EstadoDoPersonagem
		}
		else if(tipo == Tipo.metralhadora) {
			personagem.sprite = osPersonagens.personagens[7*Player.personagem + 1];
		}
		else if(tipo == Tipo.pistola) {
			personagem.sprite = osPersonagens.personagens[7*Player.personagem + 2];
		}
		else if(tipo == Tipo.faca) {
			personagem.sprite = osPersonagens.personagens[7*Player.personagem + 3];
		}
		else if(tipo == Tipo.braco) {
			personagem.sprite = osPersonagens.personagens[7*Player.personagem + 4];
		}
		else if(tipo == Tipo.monstro) {
			personagem.sprite = osPersonagens.personagens[7*Player.personagem + 6];
		}
	}

	public void setSprite(int qual) {
		personagem.sprite = osPersonagens.personagens [7*Player.personagem + qual];
	}
}
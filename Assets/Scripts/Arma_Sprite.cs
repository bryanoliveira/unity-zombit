using UnityEngine;
using System.Collections;

public class Arma_Sprite : MonoBehaviour {
	[SerializeField] private SpriteRenderer sprite;

	[SerializeField] private Sprite[] melhoriaImg;

	[SerializeField] private Arma arma;

	void Start (){
		Trocar();
	}
	void OnEnable (){
		Trocar();
	}

	public void Trocar (){
		sprite.sprite = melhoriaImg[PlayerPrefs.GetInt("arma" + arma.id)];
		arma.dano = (int)arma.melhoria[PlayerPrefs.GetInt("arma" + arma.id)].x;
		arma.alcance = arma.melhoria[PlayerPrefs.GetInt("arma" + arma.id)].y;
		arma.fireRate = arma.melhoria[PlayerPrefs.GetInt("arma" + arma.id)].z;
		arma.maxBalas = (int)arma.melhoria[PlayerPrefs.GetInt("arma" + arma.id)].w;
	}
}
using UnityEngine;
using System.Collections;

public class Player_Walk : MonoBehaviour {

	public SpriteRenderer sprite;
	public Sprite[] pernas;
	public float delay = 0.5f;
	private int comeco = 0;
	private int fim = 7;
	private int i = 0;
	private float timer;

	void Start () {
		timer = delay;
		comeco = Player.personagem * 8;
		fim = comeco + 7;
		i = comeco;
	}
	
	void Update () {
		if(timer < delay)
			timer += Player.time;
	}
	
	public void para() {
		if(timer >= delay) {
			sprite.sprite = pernas[fim];
			timer = 1;
		}
	}
	
	public void anda() {
		if(timer >= delay) {
			if(i <= fim) {
				sprite.sprite = pernas[i];
				i++;
			}
			else {
				sprite.sprite = pernas[comeco];
				i = comeco;
			}
			timer = 0;
		}
	}

	public void setPersonagem(int per) {
		comeco = per * 8;
		fim = comeco + 7;
		i = comeco;
		timer = delay;
	}
}

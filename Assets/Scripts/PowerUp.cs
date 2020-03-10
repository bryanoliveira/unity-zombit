using UnityEngine;
using System.Collections;

enum TipoDoPowerup {
	speed,
	municaoInfinita,
	invenc,
	slowMotion,
	dobro,
	municao,
	saude,
}

public class PowerUp : MonoBehaviour {
	
	[SerializeField] private TipoDoPowerup tipo;
	// Audio
	[SerializeField] private AudioClip pegou;

	void OnCollisionEnter (Collision col){
		if(col.gameObject.tag == "Player") {
			col.gameObject.GetComponent<AudioSource>().PlayOneShot(pegou);
			if(tipo == TipoDoPowerup.municao) {
				Player_Armas.eu.MaisBalas(10);
			}
			else if(tipo == TipoDoPowerup.saude) {
				Player_Saude.AddVida(20);
			}
			else if(tipo == TipoDoPowerup.speed) {
				Player_Movimento.SuperVelocidade(5);
			}
			else if(tipo == TipoDoPowerup.invenc) {
				Player_Saude.Invencibilidade(5);
			}
			else if(tipo == TipoDoPowerup.municaoInfinita) {
				Player_Armas.eu.MunicaoInfinita(5);
			}
			else if(tipo == TipoDoPowerup.slowMotion) {
				Player_Saude.SlowMotion(5);
			}
			else if(tipo == TipoDoPowerup.dobro) {
				Player_Score.eu.Dobro(5);
			}
			Destroy(gameObject);
		}
	}
}
using UnityEngine;
using System.Collections;

public class Bala_Dano : MonoBehaviour {

	public bool jaAtravessou = false;
	public int dano = 100;
	[SerializeField] private bool bazooka = false;
	[SerializeField] private GameObject explosao;

	public static bool jaSpawnouCorvo = false;
	public static int vida = 1000;

	void OnTriggerEnter (Collider other){
		if (other.gameObject.tag == "Enemy") {
			if (bazooka) {
				Instantiate (explosao, transform.position, transform.rotation);
			}
			other.gameObject.GetComponent<Enemy_Saude> ().aplicarDano (dano);
			if (jaAtravessou) {
				Destroy (gameObject);
			}
			jaAtravessou = true;
		} else if (other.gameObject.tag == "Obstacle") {
			this.dano -= 50;
			if(bazooka)
				Instantiate (explosao, transform.position, transform.rotation);
			if(dano <= 0)
				Destroy (gameObject);
		} else if (other.tag == "Arvore") {
			if (!jaSpawnouCorvo) {
				vida -= 100;
				if (vida == 0) {
					Spawner.SpawnaCorvo (GetComponentInChildren<Transform> ());
					jaSpawnouCorvo = true;
					Destroy (gameObject);
				} else if(vida == 100) {
					Camera_Follow.Mostra (other.transform, 1);
				}
			}
		}
	}  
}
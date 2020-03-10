using UnityEngine;
using System.Collections;

public class Destroi_Fade : MonoBehaviour {

	[SerializeField] private float tempo = 1;
	public SpriteRenderer meuSprite;

	void Start (){
		meuSprite.color = new Color(0.5f, 0.44f, 0.68f, 0.8f);
		StartCoroutine(destroy());
	}

	void Update (){
		if(tempo > 0) {
			if(tempo <= 0.8f) 
				meuSprite.color = new Color(0.5f, 0.44f, 0.68f, tempo);
			tempo -= Player.time;
		}
	}

	IEnumerator destroy (){
		yield return new WaitForSeconds(tempo);
		Destroy(gameObject);
	}
}
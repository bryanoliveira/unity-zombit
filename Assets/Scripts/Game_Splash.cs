using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Game_Splash : MonoBehaviour {
    [SerializeField]
    private float tempo = 3;

    [SerializeField]
    private Animator animFade;

    private Coroutine carregador = null;

    void Awake() {
		UnityEngine.Cursor.visible = false;
	}
	
	void Start() {
		StartCoroutine(Inicia());
		Time.timeScale = 1;
	}
	
	void Update() {
		if(Input.anyKey) {
            if (carregador == null) {
                StopAllCoroutines();
                carregador = StartCoroutine(Carrega());
            }
        }
	}

	private IEnumerator Inicia() {
        yield return new WaitForSeconds(tempo);
        StartCoroutine(Carrega());
    }
    private IEnumerator Carrega() {
        animFade.SetTrigger("FadeOut");
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Menu");
    }
}

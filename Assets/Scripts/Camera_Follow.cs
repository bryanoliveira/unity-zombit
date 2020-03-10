using UnityEngine;
using System.Collections;

public class Camera_Follow : MonoBehaviour {

	[SerializeField] private Transform player;
	private Transform alvo;
	[SerializeField] private Camera cam;

	// para o SmoothDamp
	private Vector3 velocity;

	private bool flashed = false;
	private bool downblur = true;

	private float smooth = 0.3f; // maciez da camera
	private float blurTimer = 4.0f;
	private float campoVisao = 13.39f;

	public static Camera_Follow eu;

	[SerializeField] private UnityStandardAssets.ImageEffects.BlurOptimized blur;
	[SerializeField] private UnityStandardAssets.ImageEffects.MotionBlur motion;

	private void Awake() {
		eu = this;
		alvo = player;
	}

	private void Start (){	
		if(QualitySettings.GetQualityLevel() > 2)
			motion.enabled = true;
		else
			motion.enabled = false;
			
		if(PlayerPrefs.GetInt("personagem") == 2)
			campoVisao = 17;

		cam.orthographicSize = 45;
	}
	private void Update (){
		if(!flashed) {
			if(cam.orthographicSize > campoVisao) {
				cam.orthographicSize -= Player.time * (700/cam.orthographicSize);
			} else {
				flashed = true;

				Camera_Flasher.eu.Flash("branco", 0.8f, 1.2f);
				Camera_Flasher.eu.Zoom(0.5f, 0.5f);

				Player_Canvas.AtualizaRound (1);
				Soundtrack.eu.Comeca();
				Resources.UnloadUnusedAssets();
			}
		}
		if(downblur) {
			if (!blur.enabled)
				blur.enabled = true;
			blurTimer -= Player.time * 3;
			blur.blurSize = blurTimer;
			blur.blurIterations = (int)blurTimer;
			if(blurTimer < 0.5f) {
				blur.blurSize = 0;
				blur.blurIterations = 4;
				blur.enabled = false;
				motion.enabled = false;
				downblur = false;
			}
		}
	}
	private void LateUpdate (){
		transform.position = new Vector3(Mathf.SmoothDamp(transform.position.x, alvo.position.x, ref velocity.x, smooth), transform.position.y, Mathf.SmoothDamp(transform.position.z, alvo.position.z, ref velocity.z, smooth));
	}

	public static void Mostra(Transform oque, float tempo, float timeScale) {
		eu.StartCoroutine (eu.MostraMethod (oque, tempo, timeScale));
	}
	public static void Mostra(Transform oque, float tempo) {
		eu.StartCoroutine (eu.MostraMethod (oque, tempo));
	}
	private IEnumerator MostraMethod(Transform oque, float tempo) {
		alvo = oque;
		Player_Saude.invencivel = true;
		yield return new WaitForSeconds (tempo);
		alvo = player;
		Player_Saude.invencivel = false;
	}
	private IEnumerator MostraMethod(Transform oque, float tempo, float timeScale) {
		alvo = oque;
		Player_Saude.invencivel = true;
		Player.timeScale = timeScale;

		yield return new WaitForSeconds (tempo);

		if (!Player.pausado) {
			if (Player_Saude.slowMotion)
				Player.timeScale = 0.5f;
			else
				Player.timeScale = 1;
		}
		alvo = player;
		Player_Saude.invencivel = false;
	}
}
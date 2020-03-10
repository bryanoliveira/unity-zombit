using UnityEngine;
using System.Collections;

public class Player_Movimento : MonoBehaviour {

	// Variaveis de controle

	public static int joystick = 0;
	public static int inclinacao = 3; // public para ser visto e alterado pelo Pausemenu

	private float speed = 10;
	private float oSpeed;
	private float autoSpeed = 4; // public para aumentar com o Slow Motion pelo Saude.cs

	private float x;
	private float z;
	private float v;
	private float h;

    private float referencia = 0.0f;

    // Referencias
    [HideInInspector] public Transform alvo; // public para o Tiro.cs alterar quando um zumbi entrar na mira
	[SerializeField] private Transform rotacao;
	[SerializeField] private Player_Walk pernasAnim;

	public static Player_Movimento eu;

	private void Awake() {
		eu = this;
	}

	private void Start (){
		if(PlayerPrefs.GetInt("personagem") == 1) {
			speed = 12;
		}
		oSpeed = speed;
		
		inclinacao = PlayerPrefs.GetInt("inclinacao");
	}

	private void Update (){
        // Setas ou WASD
        x = Game_Controles.movimento_x * Player.time * speed;
        z = Game_Controles.movimento_y * Player.time * speed;

        // Animaçoes
        if (x > 0.1f || z > 0.1f)
            pernasAnim.anda();
        else
            pernasAnim.para();

        // Atualiza a posiçao
        transform.Translate(x, 0, z);

#if UNITY_ANDROID        
        // Mira automatica
        if(alvo == null) {
			GetAlvo();
		}
		// Rotaçao em direçao ao inimigo
		Quaternion rot = Quaternion.LookRotation(alvo.position - rotacao.position);
	    rot.x = 0;
	    rot.z = 0;
		rotacao.rotation = Quaternion.Slerp(rotacao.rotation, rot, autoSpeed * Player.time); 
#else
        if (Game_Controles.indiceControle > 0) { // joysticks			
			v = Game_Controles.rotacao_y * Player.time * speed * 3;
			h = Game_Controles.rotacao_x * Player.time * speed * 3;
	 		
	 		// Atualiza a rotaçao
			if(h != 0 || v != 0) {
				Player_Canvas.jaMirou = true;
				float angle = Mathf.Atan2(h, v) * Mathf.Rad2Deg;
				float novaRot = Mathf.SmoothDampAngle(rotacao.eulerAngles.y, angle, ref referencia, 0.15f) ;
				rotacao.rotation = Quaternion.Euler(new Vector3(0, novaRot, 0));
			}
			else if(alvo != null) {
			 	// Rotaçao em direçao ao inimigo
				Quaternion rot = Quaternion.LookRotation(alvo.position - rotacao.position);
		      	rot.x = 0;
		      	rot.z = 0;
				rotacao.rotation = Quaternion.Slerp(rotacao.rotation, rot, autoSpeed * Player.time);
			}
        }
		// Mouse
		else if(Game_Controles.indiceControle == 0) { // teclado e mouse
			// Rotaçao em direçao ao mouse
			Plane playerPlane = new Plane(Vector3.up, transform.position);
	    	Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
	    	float hitdist = 0.0f;
	    	if (playerPlane.Raycast (ray, out hitdist)) { // Rotaçao em direçao ao mouse
				Vector3 targetPoint= ray.GetPoint(hitdist);
				Quaternion targetRotation= Quaternion.LookRotation(targetPoint - rotacao.position);
				rotacao.rotation = Quaternion.Slerp(rotacao.rotation, targetRotation, speed * Player.time);
	    	}
        }
		
#endif
	} // Update

	public static void SuperVelocidade (int tempo) {
		eu.StopCoroutine ("SuperVelocidadeMethod");
		eu.StartCoroutine (eu.SuperVelocidadeMethod(tempo));
	}
	private IEnumerator SuperVelocidadeMethod (int tempo){
		autoSpeed = 10;
		pernasAnim.delay = 0.04f;
		speed = oSpeed + tempo;

		Camera_Flasher.eu.Overlay("verde", 5, 1);
		Player_Canvas.MostraPowerup((int)TipoDoPowerup.speed, tempo);

		yield return new WaitForSeconds (tempo);

		autoSpeed = 4;
		pernasAnim.delay = 0.07f;
		speed = oSpeed;
	}

	public void SetSpeed(int quanto) {
		speed = quanto;
		oSpeed = quanto;
	}

	private void GetAlvo (){
		float menorDistancia = (Spawner.zumbis[0].transform.position - transform.position).sqrMagnitude; 
		for (int i = 1; i < Spawner.zumbis.Count; i++) { 
			float estaDistancia = (Spawner.zumbis[i].transform.position - transform.position).sqrMagnitude; 
			if (estaDistancia < menorDistancia && estaDistancia < 13) { 
				menorDistancia = estaDistancia; 
				alvo = Spawner.zumbis[i].transform;
			} 
		} 
	}
}
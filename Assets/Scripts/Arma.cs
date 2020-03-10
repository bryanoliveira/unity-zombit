using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
#if UNITY_STANDALONE
using XInputDotNetPure;
#endif

public enum TipoArma {
	branca = 0,
	fogo = 1
}

public class Arma : MonoBehaviour {
	// tudo é publico pra ter o editor customizado

	public TipoArma tipo;
	// -- booleans
	// tipo de tiro (falso = semi)
	public bool auto = false;
	// tipo de bala
	public bool shotgun = false;
	// tipo de arma (tudo falso indica arma simples
	public bool dual = false;
	public bool serra = false;

	private bool tocaSerra = false;
	private bool tiro2Aux = false;

	#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1
	public static bool canhoto = false;
	private bool podeAtirar = true;
	#endif

	// Inteiros e Floats
	public int id = 1;
	public int dano = 80;
	public int balas = 10; // contador de balas
	public int maxBalas = 20;
	public int rajada = 1;
	public int melhoriaAtual = 0;
	public float rajadaDelay = 0.2f;
	public float fireRate = 0.5f; // tempo para o proximo tiro
	public float alcance = 0.1f;

	private int shotEspalha = 7; // quantos spread tera o tiro da shotgun
	private int rajadaAux = 1;
	private float forca = 0.7f; // força da bala
	private float rate = 1; // auxiliar do firerate

	// Efeitos sonoros
	public AudioClip shot; // som do tiro
	public AudioClip vazio; // som sem bala
	public AudioClip[] zap; // som das facadas no ar
	public AudioClip[] hit; // som das facadas no zumbi

	// Melhorias
	public Quaternion[] melhoria; // x = dano, y = alcance, z = rate, w = max balas
	public string[] melhoriaNome;
	public Sprite[] melhoriaImg;

	// Referencias
	public SpriteRenderer flareSprite;
	public SpriteRenderer flareSprite2;
	public Light flareLight;
	public Light flareLight2;
	public Arma_Sprite sprite;
	public Arma_Sprite sprite2;
	public GameObject prefabBala;
	public Transform rotacao; // rotaçao do torso do player
	public Player_Armas armas;
	public AudioSource sound;
	public Animator anim;

	void Start (){
		#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1
		if(PlayerPrefs.GetInt("canhoto") == 1)
			canhoto = true;
		#endif

		melhoriaAtual = PlayerPrefs.GetInt ("arma" + id);
	}

	void OnEnable() {
		if (tipo == TipoArma.branca && serra) {
			sound.Play ();
		}

		// mostra quantidade de balas no canvas
		if(tipo == TipoArma.fogo)
			Player_Canvas.AtualizaBalas(balas);
		else
			Player_Canvas.AtualizaBalas(-1);
	}
	void OnDisable() {
		if (tipo == TipoArma.branca && serra) {
			sound.Stop ();
		}
	}

	void Update (){
		if(!Player.pausado && !Player_Saude.morto && !Player_Saude.quaseMorto) {
			#if UNITY_ANDROID || UNITY_IOS || UNITY_WP8 || UNITY_WP8_1
			if(Player_Movimento.joystick != 3 && Input.touchCount >= 1 && Input.touchCount < 4) {
			if(Player_Movimento.joystick == 4) {
					if(podeAtirar && rate > fireRate) {
						StartCoroutine(Ataca());
		        		if(!auto && !serra)
			     			podeAtirar = false;
			     	}
		     		foreach(var touch in Input.touches) {
		     			if (touch.phase == TouchPhase.Ended) {
		        			podeAtirar = true;
							if(tocaSerra) {
								tocaSerra = false;
							}
		     			}
		     		}
     			} 
     			else if(canhoto) {
     				foreach(var touch in Input.touches) {
		     			if (podeAtirar && rate > fireRate && touch.position.x < Screen.width/2 && touch.position.y < Screen.height - 50) {
							StartCoroutine(Ataca());
		        			if(!auto && !serra)
		     					podeAtirar = false;
		     			}
		     			if (touch.phase == TouchPhase.Ended) {
		        			podeAtirar = true;
							if(tocaSerra) {
								tocaSerra = false;
							}
		     			}
		     		}
		     	}
     			else {
	     			foreach(var touch in Input.touches) {
		     			if (podeAtirar && rate > fireRate && touch.position.x > Screen.width/2 && touch.position.y < Screen.height - 50) {
							StartCoroutine(Ataca());
		        			if(!auto && !serra)
		     					podeAtirar = false;
		     			}
		     			if (touch.phase == TouchPhase.Ended) {
		        			podeAtirar = true;
							if(tocaSerra) {
								tocaSerra = false;
							}
		     			}
		     		}
		     	}
 			}
			#else
			// Se pc
			if(rate > fireRate) {
				if(auto || serra) {
					if(Game_Controles.fireC) {
						StartCoroutine(Ataca());
					}
					else if(tocaSerra) {
						tocaSerra = false;
					}
				} else {
					if(Game_Controles.fire) {
						StartCoroutine(Ataca());
					} 
					else if(Game_Controles.fireUp) {
						rate = 10;
					}
				}
			}
			#endif
		} // se !pausado && !morto && !quasemorto
		else if(serra && sound.volume > 0)
			sound.volume = 0;
			
		if(rate < fireRate)
			rate += Player.time;

		if(serra) {
			if(tocaSerra) {
				if(sound.pitch < 1.1f) {
					anim.SetFloat("Velocidade", (sound.pitch - 0.6f) * 5);
					sound.pitch += Player.time;
					sound.volume = sound.pitch - 0.5f;
				}
			}
			else {
				if(sound.pitch > 0.6f) {
					anim.SetFloat("Velocidade", (sound.pitch - 0.6f) * 5);
					sound.pitch -= Player.time;
					sound.volume = sound.pitch - 0.5f;
				}
			}
		}
	} // update

	private IEnumerator Ataca (){
		if (rate < fireRate)
			yield break;
		
		if (tipo == TipoArma.branca) {
			RaycastHit rayHit;

			if (serra) {
				rate = 0;

				#if UNITY_ANDROID
				if(saude.vibracao)
				Vibration.Vibrate(20);
				#else
				StartCoroutine (Vibra (0, 0.4f, 0.1f));
				#endif

				if (Physics.Raycast (transform.position, rotacao.forward, out rayHit, alcance)) {
					if (rayHit.transform.tag == "Enemy") {
						sound.PlayOneShot (hit [Random.Range (0, hit.Length)]);
						rayHit.transform.GetComponent<Enemy_Saude> ().aplicarDano (dano);
						#if UNITY_ANDROID
						if(saude.vibracao)
							Vibration.Vibrate(20);
						#endif
					}
				} // raycast

				tocaSerra = true;
			} else {
				rate = 0;

				sound.PlayOneShot (zap[Random.Range(0, zap.Length)]);
				anim.SetTrigger ("esfaquear");

				#if UNITY_ANDROID
				if(saude.vibracao)
				Vibration.Vibrate(20);
				#else
				StartCoroutine (Vibra (0, 0.4f, 0.1f));
				#endif

				if (Physics.Raycast (transform.position, rotacao.forward, out rayHit, alcance)) {
					if (rayHit.transform.tag == "Enemy") {
						sound.PlayOneShot (hit[Random.Range(0, hit.Length)]);
						rayHit.transform.GetComponent<Enemy_Saude> ().aplicarDano (dano);
						#if UNITY_ANDROID
						if(saude.vibracao)
						Vibration.Vibrate(20);
						#else
						StartCoroutine (Vibra (0.8f, 0, 0.1f));
						#endif
					}
				}
			}
		}
		else {
			rajadaAux = rajada;
			do {
				if (balas > 0 || Player_Armas.municaoInfinita) {
					#if UNITY_ANDROID
					if(Player_Saude.vibracao)
						Vibration.Vibrate(20);
					#else
					StartCoroutine (Vibra ());
					#endif

					if (shotgun) {
						for (float i = 0; i < shotEspalha; i++) { // para cada spread
							float aleatorio = Random.Range (-0.7f, 0.7f);
							GameObject spread = Instantiate (prefabBala, flareLight.transform.position, rotacao.transform.rotation) as GameObject; 
							spread.GetComponent<Rigidbody> ().AddForce (rotacao.transform.TransformDirection (new Vector3 (aleatorio, aleatorio, 1)) * forca);
							spread.GetComponent<Bala_Dano> ().dano = dano;
							spread.GetComponent<Destroi> ().tempo = alcance;
						}
					} else if (dual) {
						Transform tiroTransform;
						if (tiro2Aux) {
							tiroTransform = flareLight.transform;
							tiro2Aux = false;
						} else {
							tiroTransform = flareLight2.transform;
							tiro2Aux = true;
						}
						GameObject bala = Instantiate (prefabBala, tiroTransform.position, rotacao.transform.rotation) as GameObject;
						bala.GetComponent<Rigidbody> ().AddForce (rotacao.transform.forward * forca);
						bala.GetComponent<Bala_Dano> ().dano = dano;
					} else {
						GameObject bal = (GameObject)Instantiate (prefabBala, flareLight.transform.position, rotacao.transform.rotation);
						bal.GetComponent<Rigidbody> ().AddForce (rotacao.transform.forward * forca);
						bal.GetComponent<Bala_Dano> ().dano = dano;
					}
					if (!Player_Armas.municaoInfinita) {
						balas--;
						Player_Canvas.AtualizaBalas(balas);
					}						

					rate = 0;
					anim.SetTrigger ("recoil");
					sound.PlayOneShot (shot);
					StartCoroutine (MostraFlares ());
		 		
				} else {
					sound.PlayOneShot (vazio);
					yield return new WaitForSeconds (0.1f);
					armas.Trocar (-2); // troca pra a proxima arma com balas
					break;
				}
			
				rajadaAux--;
				if (rajadaAux > 0)
					yield return new WaitForSeconds (rajadaDelay);
			} while(rajadaAux > 0);
		}
	}

	public void AddBalas (int quantas){
		if(quantas + balas > maxBalas)
			balas += maxBalas - balas;
		else
			balas += quantas;

		if(gameObject.activeSelf)
			Player_Canvas.AtualizaBalas(balas);
	}

	private IEnumerator MostraFlares (){
		if(dual) {
			if(tiro2Aux) {
				flareSprite2.enabled = true;
				flareLight2.enabled = true;
			}
			else {
				flareSprite.enabled = true;
				flareLight.enabled = true;
			}
		}
		else {
			flareSprite.enabled = true;
			flareLight.enabled = true;
		}
		yield return new WaitForSeconds(0.05f);
		flareSprite.enabled = false;
		flareLight.enabled = false;
		if(dual) {
			flareSprite2.enabled = false;
			flareLight2.enabled = false;
		}
	}
	public void DesligaFlares (){
		if (tipo == TipoArma.fogo) {
			flareSprite.enabled = false;
			flareLight.enabled = false;
			if (dual) {
				flareSprite2.enabled = false;
				flareLight2.enabled = false;
			}
		} else if (serra) {
			sound.Stop ();
		}
	}

	public void Trocar (){
		sprite.Trocar();
		if(dual)
			sprite2.Trocar();
	}

	public string GetNome() {
		return melhoriaNome [melhoriaAtual];
	}
	public Sprite GetIcone() {
		return melhoriaImg [melhoriaAtual];
	}
		
	#if UNITY_STANDALONE
	private IEnumerator Vibra (){
		GamePad.SetVibration(0, 0, 1);
		yield return new WaitForSeconds(0.08f);
		GamePad.SetVibration(0, 0, 0);
	}
	private IEnumerator Vibra (float esquerda, float direita, float duracao){
		GamePad.SetVibration(0, esquerda, direita);
		yield return new WaitForSeconds(duracao);
		GamePad.SetVibration(0, 0, 0);
	}
	#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Arma))]
public class ArmaEditor : Editor {
	public override void OnInspectorGUI() {
		serializedObject.Update();
		var arma = target as Arma;

		arma.id = EditorGUILayout.IntField("ID", arma.id);
		arma.tipo = (TipoArma)EditorGUILayout.EnumPopup ("Tipo de arma", arma.tipo);
		EditorGUILayout.LabelField ("");
		EditorGUILayout.LabelField ("====== Dados gerais ======");
		arma.dano = EditorGUILayout.IntField("Dano", arma.dano);
		arma.fireRate = EditorGUILayout.FloatField("Fire Rate", arma.fireRate);

		switch (arma.tipo) {
		case TipoArma.branca :
			arma.serra = GUILayout.Toggle(arma.serra, "Serra");

			EditorGUILayout.LabelField ("");
			EditorGUILayout.LabelField ("====== Melhorias ======");
			// mostra array de melhorias
			SerializedProperty melhorias = serializedObject.FindProperty ("melhoria");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(melhorias, true);
			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
			// mostra array dos nomes das melhorias
			SerializedProperty nomeMelhorias = serializedObject.FindProperty ("melhoriaNome");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(nomeMelhorias, true);
			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
			// mostra array das imagens das melhorias
			SerializedProperty imagensMelhorias = serializedObject.FindProperty ("melhoriaImg");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(imagensMelhorias, true);
			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();

			EditorGUILayout.LabelField ("");
			EditorGUILayout.LabelField ("====== Referências ======");
			arma.sound = EditorGUILayout.ObjectField("Audio Source", arma.sound, typeof(AudioSource), true) as AudioSource;
			arma.anim = EditorGUILayout.ObjectField("Animator", arma.anim, typeof(Animator), true) as Animator;
			arma.sprite = EditorGUILayout.ObjectField("Arma_Sprite", arma.sprite, typeof(Arma_Sprite), true) as Arma_Sprite;
			arma.armas = EditorGUILayout.ObjectField("Player_Armas", arma.armas, typeof(Player_Armas), true) as Player_Armas;
			arma.rotacao = EditorGUILayout.ObjectField("Torso Transform", arma.rotacao, typeof(Transform), true) as Transform;
			EditorGUILayout.LabelField ("");
			EditorGUILayout.LabelField ("====== Sons ======");
			// mostra array de zaps
			SerializedProperty zaps = serializedObject.FindProperty ("zap");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(zaps, true);
			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
			// fim- mostra array de zaps
			// mostra array de hits
			SerializedProperty hits = serializedObject.FindProperty ("hit");
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(hits, true);
			if(EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
			// fim- mostra array de hits

			break;
		case TipoArma.fogo:
			arma.auto = GUILayout.Toggle (arma.auto, "Auto");
			arma.shotgun = GUILayout.Toggle (arma.shotgun, "Shotgun");
			arma.dual = GUILayout.Toggle (arma.dual, "Dual");

			arma.alcance = EditorGUILayout.FloatField("Alcance", arma.alcance);
			arma.balas = EditorGUILayout.IntField("Balas no pente", arma.balas);
			arma.maxBalas = EditorGUILayout.IntField("Máximo de balas", arma.maxBalas);
			arma.rajada = EditorGUILayout.IntField("Rajadas", arma.rajada);
			if(arma.rajada > 1)
				arma.rajadaDelay = EditorGUILayout.FloatField("Delay entre rajadas", arma.rajadaDelay);

			EditorGUILayout.LabelField ("");
			EditorGUILayout.LabelField ("====== Melhorias ======");
			// mostra array de melhorias
			SerializedProperty melhorias1 = serializedObject.FindProperty ("melhoria");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (melhorias1, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			// mostra array dos nomes das melhorias
			SerializedProperty nomeMelhorias1 = serializedObject.FindProperty ("melhoriaNome");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (nomeMelhorias1, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();
			// mostra array das imagens das melhorias
			SerializedProperty imagensMelhorias1 = serializedObject.FindProperty ("melhoriaImg");
			EditorGUI.BeginChangeCheck ();
			EditorGUILayout.PropertyField (imagensMelhorias1, true);
			if (EditorGUI.EndChangeCheck ())
				serializedObject.ApplyModifiedProperties ();

			EditorGUILayout.LabelField ("");
			EditorGUILayout.LabelField ("====== Referências ======");
			arma.sound = EditorGUILayout.ObjectField ("Audio Source", arma.sound, typeof(AudioSource), true) as AudioSource;
			arma.anim = EditorGUILayout.ObjectField ("Animator", arma.anim, typeof(Animator), true) as Animator;
			arma.sprite = EditorGUILayout.ObjectField ("Arma_Sprite", arma.sprite, typeof(Arma_Sprite), true) as Arma_Sprite;
			arma.armas = EditorGUILayout.ObjectField ("Player_Armas", arma.armas, typeof(Player_Armas), true) as Player_Armas;

			arma.prefabBala = EditorGUILayout.ObjectField ("Prefab da Bala", arma.prefabBala, typeof(GameObject), true) as GameObject;

			arma.flareSprite = EditorGUILayout.ObjectField ("Flare Sprite", arma.flareSprite, typeof(SpriteRenderer), true) as SpriteRenderer;
			arma.flareLight = EditorGUILayout.ObjectField ("Flare Light", arma.flareLight, typeof(Light), true) as Light;
			if (arma.dual) {
				arma.flareSprite2 = EditorGUILayout.ObjectField ("Flare Sprite 2", arma.flareSprite2, typeof(SpriteRenderer), true) as SpriteRenderer;
				arma.flareLight2 = EditorGUILayout.ObjectField ("Flare Light 2", arma.flareLight2, typeof(Light), true) as Light;
				arma.sprite2 = EditorGUILayout.ObjectField ("Arma Sprite 2", arma.sprite2, typeof(Arma_Sprite), true) as Arma_Sprite;
			}
			arma.rotacao = EditorGUILayout.ObjectField("Torso Transform", arma.rotacao, typeof(Transform), true) as Transform;
			EditorGUILayout.LabelField ("");
			EditorGUILayout.LabelField ("====== Sons ======");
			arma.shot = EditorGUILayout.ObjectField("Som de Tiro", arma.shot, typeof(AudioClip), true) as AudioClip;
			arma.vazio = EditorGUILayout.ObjectField("Som vazio", arma.vazio, typeof(AudioClip), true) as AudioClip;
			break;
		default:
			break;
		}
	}
}
#endif
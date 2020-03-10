using UnityEngine;
using System.Collections;

public enum enemyTypes {
	Zumbi,
	Death,
	Gordo,
	Bomba
}

public class Enemy_AI : MonoBehaviour {
	public enemyTypes tipo;
	[SerializeField] private int dano = 20;
	[SerializeField] private float minSpeedWalk = 4;
	[SerializeField] private float maxSpeedWalk = 7;
	[SerializeField] private float stopDistance;
	private float velocidade;
	private float timer = 0.0f;
	[SerializeField] private float delayDano = 0.5f;
	[SerializeField] private float rangeZumbi = 1.0f;
	private float curDistance;

	private bool podeAtacar = true;
	private bool isChasing = true;

	// Referencias
	private Transform target;
	[SerializeField] private GameObject blood;
	[SerializeField] private GameObject bloodSprite;
	[SerializeField] private Animator anim;
	[SerializeField] private UnityEngine.AI.NavMeshAgent NavComponent;

	public Transform minhaArvore;
	 
	void Start (){
		target = Player.eu.transform;
		
		if(tipo != enemyTypes.Death) {
			minSpeedWalk += Spawner.round * 0.5f;
			maxSpeedWalk += Spawner.round * 0.5f;
		}
		velocidade = Random.Range (minSpeedWalk, maxSpeedWalk);
		NavComponent.speed = velocidade; 
	    NavComponent.stoppingDistance = stopDistance;
	}

	void Update (){
        curDistance = Vector3.Distance(target.position, transform.position);
        
        if(curDistance <= stopDistance + rangeZumbi) {
	        if(podeAtacar){
	        	isChasing = false;
	        	Atacar();
	        }
	       	else {
	    		if(timer > delayDano) {
	    			podeAtacar = true;
	    			timer = 0;
	    		} else {
					timer += Player.time;
	    		}
	    	}
	    }

        if (isChasing){
        	NavComponent.SetDestination(target.position);
        	NavComponent.Resume();  
        }
        if (!isChasing){
        	NavComponent.Stop();
        	if (curDistance > stopDistance)
		    	isChasing = true;
        }
		NavComponent.speed = velocidade * Player.timeScale;
	}
	 
	void Atacar (){
		if(target.tag != "Player" || Player.pausado)
			return;
			
		anim.SetTrigger("Ataca");
		
		Player_Saude.eu.aplicarDano(dano);
		Instantiate(blood, new Vector3(target.position.x, target.position.y - 2, target.position.z), target.rotation);
		Instantiate(bloodSprite, new Vector3(target.position.x, target.position.y - 2, target.position.z), transform.rotation);
	    podeAtacar = false;
	}

	public void AtualizaTarget (){
		if(target == minhaArvore) {
			target = Player.eu.transform;
		}
		else {
			target = minhaArvore;
		}
	}
}
using System.Collections;
using UnityEngine;

public class Helicoptero : MonoBehaviour {

	[SerializeField]
	private int velocidade;

	public static bool disponivel = true;

	[SerializeField]
	private GameObject[] drops;

	public static Helicoptero eu;

    public Transform alvoAtual;
    [SerializeField]
    private Transform heliportoAtual;
	[SerializeField]
	private Transform heliportoProximo;

    private void Awake() {
        eu = this;
		gameObject.SetActive (false);
    }

    private void Update() {
		transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(alvoAtual.position - transform.position), 3 * Player.time);

        transform.position = Vector3.MoveTowards(transform.position, alvoAtual.position, velocidade * Player.time);
    }

    private void OnTriggerEnter(Collider obj) {
		if (obj.tag == "Heliporto" && !disponivel) {
			heliportoProximo = heliportoAtual;
			heliportoAtual = obj.transform;
			disponivel = true;
			gameObject.SetActive (false);
			Debug.Log ("chegou");
		} else if (obj.tag == "Dropzone") {
			disponivel = false;
			GameObject drop = Instantiate (drops [Random.Range (0, drops.Length)], new Vector3(obj.transform.position.x, -1, obj.transform.position.z), transform.rotation) as GameObject;
			Camera_Follow.Mostra (drop.transform, 1);
			alvoAtual = heliportoProximo;
		}
	}
}

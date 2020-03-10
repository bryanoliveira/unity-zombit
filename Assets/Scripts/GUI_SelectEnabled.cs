using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class GUI_SelectEnabled : MonoBehaviour {
	void OnEnable() {
		StartCoroutine(OnEnableAsync());
	}

	private IEnumerator OnEnableAsync() {
		yield return new WaitForSeconds (1);
		EventSystem.current.SetSelectedGameObject(gameObject);
	}
}
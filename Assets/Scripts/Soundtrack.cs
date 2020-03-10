using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Soundtrack : MonoBehaviour {

	[SerializeField]
    private AudioClip[] musicas;

    private List<AudioClip> lista;

	[SerializeField]
    private AudioSource musicplayer;

	public static Soundtrack eu = null;

	void Awake (){
		if(eu != null && eu != this) {
			Destroy(gameObject);
			return;
		} else {
			eu = this;
			DontDestroyOnLoad(gameObject);
		}
	}

    void Start() {
        lista = new List<AudioClip>();

        if (!GameObject.Find("Player")) {
            Comeca();
        }
    }

	void Update (){
		if(!musicplayer.isPlaying && musicplayer.time > 0){
            Proxima();
		}
	}

	public void Comeca (){
		if (PlayerPrefs.HasKey("musicaVolume")) {
			musicplayer.volume = PlayerPrefs.GetFloat("musicaVolume");
		}
		else {
			PlayerPrefs.SetFloat("musicaVolume", 0.5f);
			musicplayer.volume = 0.5f;
		}
		if (musicplayer.isPlaying)
			return;
        ClipeAleatorio();
		musicplayer.Play();
	}
	public void Proxima (){
        ClipeAleatorio();
		musicplayer.Play();
	}

    private void ClipeAleatorio() {
        if (lista.Count <= 0) {
            CriaLista();
        }
        int index = Random.Range(0, lista.Count);
        musicplayer.clip = lista[index];
        lista.RemoveAt(index);
    }

    private void CriaLista() {
        foreach (AudioClip clip in musicas) {
            lista.Add(clip);
        }
    }

	public void SetPitch(float pitch) {
		eu.StartCoroutine (eu.SetPitchMethod (pitch));
	}
	private IEnumerator SetPitchMethod(float pitch) {
		if (pitch > musicplayer.pitch) {
			while (pitch > musicplayer.pitch) {
				musicplayer.pitch += 0.02f;
				yield return new WaitForSeconds (0.1f);
			}
		} else {
			while (pitch < musicplayer.pitch) {
				musicplayer.pitch -= 0.02f;
				yield return new WaitForSeconds (0.1f);
			}
		}
	}
	public float GetPitch() {
		return musicplayer.pitch;
	}
	public void AddPitch(float pitch, float max) {
		if ((pitch > 0 && musicplayer.pitch < max) || (pitch < 0 && musicplayer.pitch > max)) {
			musicplayer.pitch += pitch;
		}
	}

	public void SetVolume(float volume) {
		musicplayer.volume = volume;
	}
	public float GetVolume() {
		return musicplayer.volume;
	}
	public void AddVolume(float volume, float max) {
		if ((volume > 0 && musicplayer.volume < max) || (volume < 0 && musicplayer.volume > max)) {
			musicplayer.volume +=  volume;
		}
	}
}
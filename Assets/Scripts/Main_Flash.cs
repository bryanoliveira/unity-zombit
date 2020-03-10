using UnityEngine;
using System.Collections;

public class Main_Flash : MonoBehaviour {
	[SerializeField] private Texture2D branco;
	[SerializeField] private Camera cam;
	// Musica
	private AudioSource music;
	private bool  pode = true;
	//
	private int qSamples = 1024;  // tamanho do array
	private float rmsValue;   // nivel final
	private float[] samples; // samples
	//
	private float fator;

	void Start (){
		useGUILayout = false;
		samples = new float[qSamples];
		if(PlayerPrefs.GetInt("podePiscar") == 1)
		    pode = false;

		music = GameObject.FindWithTag("SoundTrack").GetComponent<AudioSource>();
	}

	void Update (){
		if(pode && QualitySettings.GetQualityLevel() > 1) {
            music.GetOutputData(samples, 0); // fill array with samples
            fator = 0.3f;

            int i;
            float sum = 0;
            for (i = 0; i < qSamples; i++) {
                sum += samples[i] * samples[i]; // sum squared samples
            }
            rmsValue = Mathf.Sqrt(sum / qSamples) + (fator); // rms = square root of average
            cam.orthographicSize = 45 - rmsValue * 10;
        }
	}
	void OnGUI (){
		if(pode) {	
			GUI.color = new Color(1, 1, 1, rmsValue / 8 - 0.1f);
			GUI.DrawTexture( new Rect(0,0,Screen.width,Screen.height), branco);
		}
	}

	void LigarFlash (bool sim){
        if (sim) {
            pode = true;
            PlayerPrefs.SetInt("podePiscar", 0);
        }
        else {
            pode = false;
            PlayerPrefs.SetInt("podePiscar", 1);
        }
	}
}
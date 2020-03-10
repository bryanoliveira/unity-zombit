using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Main_Menu : MonoBehaviour {

    private int indiceResolucao;

    [SerializeField]
    private GameObject btnSair;

    [SerializeField]
    private Animator animFade;

    [SerializeField]
    private AudioSource sfx;

    [SerializeField]
    private Dropdown dropQuality;
    [SerializeField]
    private Dropdown dropResolucoes;
    [SerializeField]
    private Dropdown dropQuickMenuToggle;

    [SerializeField]
    private Toggle tglMudo;
    [SerializeField]
    private Toggle tglHud;
    [SerializeField]
    private Toggle tglFlash;
    [SerializeField]
    private Toggle tglDistortion;
    [SerializeField]
    private Toggle tglTelaCheia;

    [SerializeField]
    private Slider sldMusica;
    [SerializeField]
    private Slider sldSFX;

    [SerializeField]
    private NoiseEffect noise;
    
    [SerializeField]
    private UnityStandardAssets.ImageEffects.ColorCorrectionCurves ccc;

    /************************/
    private void Awake() {
        Time.timeScale = 1;
    }
    private void Start() {
#if UNITY_ANDROID
        btnSair.SetActive(false);
#endif
        
        AudioListener.volume = 1 - PlayerPrefs.GetInt("emudecer");
        tglMudo.isOn = AudioListener.volume == 1;

		if (PlayerPrefs.HasKey("sfxVolume")) {
			sldMusica.value = Soundtrack.eu.GetVolume();

            sfx.volume = PlayerPrefs.GetFloat("sfxVolume");
            sldSFX.value = sfx.volume;
        }
        else {
            PlayerPrefs.SetFloat("sfxVolume", 1);
        }

        dropQuality.value = PlayerPrefs.GetInt("qualidade");
        SetQuality();

        if(PlayerPrefs.GetInt("hud") == 1) {
            tglHud.isOn = false;
        }
        if(PlayerPrefs.GetInt("distortion") == 1) {
            tglDistortion.isOn = false;
        }
        if(PlayerPrefs.GetInt("flash") == 1) {
            tglFlash.isOn = false;
        }

        tglTelaCheia.isOn = Screen.fullScreen;

        indiceResolucao = Screen.resolutions.Length - 1;
        for(int i = 0; i < Screen.resolutions.Length; i++) {
            Dropdown.OptionData opcao = new Dropdown.OptionData() { text = Screen.resolutions[i].width + "x" + Screen.resolutions[i].height };
            if (dropResolucoes.options.Contains(opcao))
                continue;
            // adiciona opção dessa resolução no dropdown
            dropResolucoes.options.Add(opcao);
            // verifica se essa posição é minha resolução atual
            if (Screen.width == Screen.resolutions[i].width && Screen.height == Screen.resolutions[i].height) {
                indiceResolucao = i;
            }
        }
        // seleciona minha resolução atual
        dropResolucoes.value = indiceResolucao;

        StartCoroutine(AjustarValor());
    } // start

	private void Update() {
		if (Game_Controles.inserir || Game_Controles.start) {
			Comeca ();
		}
	}
    /********* UI ***********/



    public void Liga(GameObject obj) {
        obj.SetActive(true);
    }

    public void Desliga(GameObject obj) {
        obj.SetActive(false);
    }

    public void Comeca() {
        StartCoroutine(ComecaRoutine());
    }

    public void Sair() {
        Application.Quit();
    }

    public void ToggleIdioma() {
        if(PlayerPrefs.GetString("lang") == "en" || Application.systemLanguage != SystemLanguage.Portuguese) {
            PlayerPrefs.SetString("lang", "pt");
        }
        else {
            PlayerPrefs.SetString("lang", "en");
        }
    }
    
    public void Toca(AudioClip som) {
        sfx.PlayOneShot(som);
    }

    public void Reseta() {
        PlayerPrefs.DeleteAll();
    }

    public void CheckMute() {
        if(tglMudo.isOn) {
            AudioListener.volume = 1;
            PlayerPrefs.SetInt("emudecer", 0);
        }
        else {
            AudioListener.volume = 0;
            PlayerPrefs.SetInt("emudecer", 1);
        }
    }

    public void SetSFXVolume() {
        sfx.volume = sldSFX.value;
        PlayerPrefs.SetFloat("sfxVolume", sldSFX.value);
    }
    public void SetMusicVolume() {
		Soundtrack.eu.SetVolume(sldMusica.value);
        PlayerPrefs.SetFloat("musicaVolume", sldMusica.value);
    }
    public void ToggleSFX() {
        if(sfx.volume > 0) {
            sfx.volume = 0;
            sldSFX.value = 0;
            PlayerPrefs.SetFloat("sfxVolume", 0);
        }
        else {
            sfx.volume = 1;
            sldSFX.value = 1;
            PlayerPrefs.SetFloat("sfxVolume", 1);
        }
    }
    public void ToggleMusic() {
		if (Soundtrack.eu.GetVolume() > 0) {
			Soundtrack.eu.SetVolume(0);
            sldMusica.value = 0;
            PlayerPrefs.SetFloat("musicaVolume", 0);
        }
        else {
			Soundtrack.eu.SetVolume(0.5f);
            sldMusica.value = 0.5f;
            PlayerPrefs.SetFloat("musicaVolume", 0.5f);
        }
    }

    public void SetQuality() {
        PlayerPrefs.SetInt("qualidade", dropQuality.value);
        if(dropQuality.value == 0) {
            QualitySettings.SetQualityLevel(5);
            if (PlayerPrefs.GetInt("noise") == 0)
                noise.enabled = true;
        }
        else if(dropQuality.value == 1) {
            QualitySettings.SetQualityLevel(3);
            if (PlayerPrefs.GetInt("noise") == 0)
                noise.enabled = true;
        }
        else if(dropQuality.value == 2) {
            QualitySettings.SetQualityLevel(0);
            noise.enabled = false;
        }
    }

    public void ToggleFlash() {
        if(tglFlash.isOn) {
            // efeitos que piscam a tela e dão zoom
            PlayerPrefs.SetInt("flash", 0);

            // pisca de acordo com a musica
            PlayerPrefs.SetInt("podePiscar", 0);
            SendMessage("LigarFlash", true);
        }
        else {
            // efeitos que piscam a tela e dão zoom
            PlayerPrefs.SetInt("flash", 1);

            // pisca de acordo com a musica
            PlayerPrefs.SetInt("podePiscar", 1);
            SendMessage("LigarFlash", false);
        }
    }
    public void ToggleDistortion() {
        if(tglDistortion.isOn) {
            PlayerPrefs.SetInt("distortion", 0);

            // vignetting
            PlayerPrefs.SetInt("vig", 0);

            // noise
            noise.enabled = true;
            PlayerPrefs.SetInt("noise", 0);
        }
        else {
            // toggle geral pra ser implementado depois em todo o jogo
            PlayerPrefs.SetInt("distortion", 1);
            
            // vignetting
            PlayerPrefs.SetInt("vig", 1);

            // noise
            noise.enabled = false;
            PlayerPrefs.SetInt("noise", 1);
        }
    }
    public void ToggleHud() {
        if(tglHud.isOn) {
            PlayerPrefs.SetInt("hud", 0);
        }
        else {
            PlayerPrefs.SetInt("hud", 1);
        }
    }

    public void ToggleResolution() {
        indiceResolucao = dropResolucoes.value;
        Screen.SetResolution(Screen.resolutions[indiceResolucao].width, Screen.resolutions[indiceResolucao].height, Screen.fullScreen);
    }

    public void ToggleFullscreen() {
        Screen.fullScreen = tglTelaCheia.isOn;
    }

    public void SetQuickMenuToggle() {
        PlayerPrefs.SetInt("quickMenuToggle", dropQuickMenuToggle.value);
    }

    /*********************/
    private IEnumerator ComecaRoutine() {
        animFade.SetTrigger("FadeOut");
		while (Soundtrack.eu.GetVolume () > 0) {
			Soundtrack.eu.AddVolume (-Time.deltaTime, 0);
			yield return new WaitForSeconds (0.02f);
		}
        yield return new WaitForSeconds(2);
        SceneManager.LoadScene("Game");
    }
    
    private IEnumerator AjustarValor() {
        ccc.blueChannel.MoveKey(0, new Keyframe(0, 0));
        ccc.redChannel.MoveKey(0, new Keyframe(0, 0));
        ccc.greenChannel.MoveKey(0, new Keyframe(0, 0));
        ccc.greenChannel.MoveKey(1, new Keyframe(1, 0));

        bool subindo = true;
        float slider = 0;

        for (;;) {
            if (subindo)
                slider += 0.02f;
            else
                slider -= 0.02f;

            if (slider >= 3.0)
                subindo = false;
            if (slider <= 0.0)
                subindo = true;

            float valor = slider / 1.5f;

            if (valor < 1) {
                ccc.blueChannel.MoveKey(1, new Keyframe(1, 1 - valor));
                ccc.redChannel.MoveKey(1, new Keyframe(1, valor));
            }
            else if (valor < 2) {
                ccc.redChannel.MoveKey(1, new Keyframe(1, 2 - valor));
                ccc.greenChannel.MoveKey(1, new Keyframe(1, valor - 1));
            }
            ccc.UpdateParameters();

            yield return new WaitForSeconds(0.2f);
        }
    }
}

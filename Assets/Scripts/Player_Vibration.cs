using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_STANDALONE
using XInputDotNetPure;
#endif

public class Player_Vibration : MonoBehaviour{
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject vibrationObj = VibrationActivity.activityObj.Get<AndroidJavaObject>("vibration");
#endif
	private static Player_Vibration eu;

	private void Awake() {
		eu = this;
	}
 
    public static void Vibra() {
		#if UNITY_ANDROID && !UNITY_EDITOR
        vibrationObj.Call("vibrate");
		#else
		eu.StartCoroutine(VibraMethod());
    }
	private static IEnumerator VibraMethod() {
		GamePad.SetVibration(0, 1, 0);
		yield return new WaitForSeconds(0.15f);
		GamePad.SetVibration(0, 0, 0);
		#endif
	}

	public static void Vibra(int milliseconds) {
		#if UNITY_ANDROID && !UNITY_EDITOR
        vibrationObj.Call("vibrate", milliseconds);
		#else 
		eu.StartCoroutine(VibraMethod(milliseconds));
    }
	private static IEnumerator VibraMethod(int milliseconds) {
		GamePad.SetVibration(0, 1, 0);
		yield return new WaitForSeconds(milliseconds / 1000);
		GamePad.SetVibration(0, 0, 0);
		#endif
	}

	public static void Vibra(long[] pattern, int repeat) {
		#if UNITY_ANDROID && !UNITY_EDITOR
        vibrationObj.Call("vibrate", pattern, repeat);
		#endif
    }

    public static bool HasVibrator() {
		#if UNITY_ANDROID && !UNITY_EDITOR
        if (Application.platform == RuntimePlatform.Android)
            return vibrationObj.Call<bool>("hasVibrator");
        else
		#endif
            return false;
    }

    public static void Cancel() {
		#if UNITY_ANDROID && !UNITY_EDITOR
        vibrationObj.Call("cancel");
		#endif
    }
}
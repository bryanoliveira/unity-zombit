using UnityEngine;
using System.Collections;

public class Camera_Cursor: MonoBehaviour {

	public static bool pode = true; // depende se o joystick está conectado ou não

	private Texture2D cursor;
	[SerializeField] private Texture2D ativo;
	[SerializeField] private Texture2D idle;

	void Start (){
#if UNITY_STANDALONE || UNITY_WEBGL || UNITY_WEBPLAYER
        Game_Controles.ControleMudou += OnControleMudou;
#else
        Destroy(this);
#endif

#if !UNITY_EDITOR
	    UnityEngine.Cursor.visible = false;
#endif
        cursor = idle;
	}

	void Update (){
		if(Input.GetButton("Atirar"))
			cursor = ativo;
		else
			cursor = idle;
	}

	void OnGUI (){
		if(pode)
	   		GUI.DrawTexture(new Rect(Input.mousePosition.x - 16, Screen.height - Input.mousePosition.y - 16, 32, 32), cursor);
	}

    void OnControleMudou() {
		if (Player.pausado)
			return;
		
        if (Game_Controles.indiceControle > 0) {
            pode = false;
        }
        else {
            pode = true;
        }
    }
}
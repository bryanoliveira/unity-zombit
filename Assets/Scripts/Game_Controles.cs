using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#if UNITY_STANDALONE
using XInputDotNetPure;
#endif

public static class Game_Controles {

    // controles > 1 = joysticks; < = teclado

    public static int indiceControle;
    private static int indiceControleAnterior;
    // controles
    public static float movimento_x;
    public static float movimento_y;
    public static float rotacao_x;
    public static float rotacao_y;
    public static bool fire;
    public static bool fireUp;
    public static bool fireC;
	public static bool armaSwitch;
    public static bool back;
    public static bool backUp;
    public static bool backC;
    public static bool start;
    //public static bool startUp;
    //public static bool startC;
    public static bool nuke;
    public static bool dinamite;
	public static bool acao;
	public static bool zoom;
	public static bool upC;
	public static bool downC;
	public static bool voltar;
	public static bool inserir;

    public delegate void RespostaBotaoMudou();
    public static event RespostaBotaoMudou ControleMudou;

	#if UNITY_STANDALONE
	private static List<GamePadState> state;
	private static List<GamePadState> prevState;
	#endif

	public static void Awake () {
        // inicializa minhas listas
        #if UNITY_STANDALONE
        state = new List<GamePadState>();
        prevState = new List<GamePadState>();
        #endif
	}

	public static void GetEntrada () {
        indiceControleAnterior = indiceControle;

        // reseta os valores anteriores
        movimento_x = movimento_y = rotacao_x = rotacao_y = 0;
		fire = fireUp = fireC = armaSwitch = back = backC = backUp = start = nuke = dinamite = acao = zoom = upC = inserir = downC = voltar = false;

        // mov x
        if (Input.GetAxis("Horizontal") != 0) {
            movimento_x = Input.GetAxis("Horizontal");
            indiceControle = 0;
        }
        // mov y
        if (Input.GetAxis("Vertical") != 0) {
            movimento_y = Input.GetAxis("Vertical");
            indiceControle = 0;
        }
        // rot x
        if (Input.GetAxis("RotacaoHorizontal") != 0) {
            rotacao_x = Input.GetAxis("RotacaoHorizontal");
            indiceControle = 0;
        }
        // rot y
        if(Input.GetAxis("RotacaoVertical") != 0) { 
            rotacao_y = Input.GetAxis("RotacaoVertical");
            indiceControle = 0;
        }
        // shoot single
        if (Input.GetButtonDown("Atirar")) {
            fire = true;
            indiceControle = 0;
        }
        // shoot reset
        if (Input.GetButtonUp("Atirar")) {
            fireUp = true;
            indiceControle = 0;
        }
        // shoot continuo
        if (Input.GetButton("Atirar")) {
            fireC = true;
            indiceControle = 0;
        }
        // troca arma
        if (Input.GetButtonDown("TrocarArma")) {
            armaSwitch = true;
            indiceControle = 0;
        }
        // select single
        if(Input.GetButtonDown("MenuDeAcoes")) {
            back = true;
            indiceControle = 0;
        }
        // select reset
        if (Input.GetButtonUp("MenuDeAcoes")) {
            backUp = true;
            indiceControle = 0;
        }
        // select continuo
        if (Input.GetButton("MenuDeAcoes")) {
            backC = true;
            indiceControle = 0;
        }
        // start single
        if(Input.GetButtonDown("Pausar")) {
            start = true;
            indiceControle = 0;
        }
        // start reset
        /*if (Input.GetButtonUp("Pausar")) {
            startUp = true;
            indiceControle = 0;
        }
        // start single
        if (Input.GetButton("Pausar")) {
            startC = true;
            indiceControle = 0;
        }*/
        // nuke
        if(Input.GetButtonDown("Nuke")) {
            nuke = true;
            indiceControle = 0;
        }
        // dinamite
        if (Input.GetButtonDown("Dinamite")) {
            dinamite = true;
            indiceControle = 0;
        }
		// acao
		if (Input.GetButtonDown ("Acao")) {
			acao = true;
			indiceControle = 0;
		}
		// zoom
		if (Input.GetButtonDown ("Zoom")) {
			zoom = true;
			indiceControle = 0;
		}
		// up
		if (Input.GetButtonDown ("Up")) {
			upC = true;
			indiceControle = 0;
		}
		// down
		if (Input.GetButtonDown ("Down")) {
			downC = true;
			indiceControle = 0;
		}
		// voltar
		if (Input.GetButtonDown ("Voltar")) {
			voltar = true;
			indiceControle = 0;
		}
		if (Input.GetButtonDown ("Inserir")) {
			inserir = true;
			indiceControle = 0;
		}

        // verifica os joysticks
        /*
        for (int i = 0; GamePad.GetState((XInputDotNetPure.PlayerIndex)i).IsConnected; i++) {
            // se minha contagem real de estados for menor que o indice do controle atual
            if (state.Count - 1 < i) {
                // adiciona o estado do controle atual (que ja vai ser o indice i pois é adicionado um a cada loop)
                state.Add(GamePad.GetState((XInputDotNetPure.PlayerIndex)i));
                // adiciona o estado atual como anterior também, não vai fazer diferença no primeiro frame
                prevState.Add(state[i]);
            }
            // se a contagem estiver correga (do segundo frame pra frente), só atualiza os estados
            else {
                prevState[i] = state[i];
                state[i] = GamePad.GetState((XInputDotNetPure.PlayerIndex)i);
            }

            // verifica estado e atualiza valores
            // pos x
            if (state[i].ThumbSticks.Left.X != 0) {
                movimento_x = state[i].ThumbSticks.Left.X;
                indiceControle = 1 + i;
            }
            // pos y
            if (state[i].ThumbSticks.Left.Y != 0) {
                movimento_y = state[i].ThumbSticks.Left.Y;
                indiceControle = 1 + i;
            }
            // rot x
            if (state[i].ThumbSticks.Right.X != 0) {
                rotacao_x = state[i].ThumbSticks.Right.X;
                indiceControle = 1 + i;
            }
            // rot y
            if (state[i].ThumbSticks.Right.Y != 0) {
                rotacao_y = state[i].ThumbSticks.Right.Y;
                indiceControle = 1 + i;
            }
            // shoot single
			if (state[i].Triggers.Right != 0 && prevState[i].Triggers.Right < 1) {
                fire = true;
                indiceControle = 1 + i;
            }
            // shoot reset
			else if (state[i].Triggers.Right < 1 && prevState[i].Triggers.Right != 0) {
                // o gatilho estava pressionado no frame anterior mas não está neste
                fire = false;
                fireUp = true;
                indiceControle = 1 + i;
            }
            // shoot continuo
            if (state[i].Triggers.Right != 0) {
                fireC = true;
                indiceControle = 1 + i;
            }
            // nuke
			if (prevState[i].Buttons.LeftShoulder == ButtonState.Released && state[i].Buttons.LeftShoulder == ButtonState.Pressed) {
				nuke = true;
                indiceControle = 1 + i;
            }
            // dinamite
            if (prevState[i].Buttons.RightShoulder == ButtonState.Released && state[i].Buttons.RightShoulder == ButtonState.Pressed) {
				dinamite = true;
                indiceControle = 1 + i;
            }
            // select single
            if (state[i].Buttons.Back == ButtonState.Pressed && prevState[i].Buttons.Back == ButtonState.Released) {
                back = true;
                indiceControle = 1 + i;
            }
            // select reset
            if (state[i].Buttons.Back == ButtonState.Released && prevState[i].Buttons.Back == ButtonState.Pressed) {
                backUp = true;
                indiceControle = 1 + i;
            }
            // select continuo
            if (state[i].Buttons.Back == ButtonState.Pressed) {
                backC = true;
                indiceControle = 1 + i;
            }
            // start single
			if (state[i].Buttons.Start == ButtonState.Pressed && prevState[i].Buttons.Start == ButtonState.Released) {
                start = true;
                indiceControle = 1 + i;
            }
            // start reset
            /*if (Input.GetButtonUp("Pausar")) {
                startUp = true;
                indiceControle = 1 + i;
            }
            // start single
            if (Input.GetButton("Pausar")) {
                startC = true;
                indiceControle = 1 + i;
            }*/ 
            /*
            // nuke
			if (prevState[i].Buttons.Y == ButtonState.Released && state[i].Buttons.Y == ButtonState.Pressed) {
				armaSwitch = true;
				indiceControle = 1 + i;
            }
			// acao
			if(prevState[i].Buttons.X == ButtonState.Released && state[i].Buttons.X == ButtonState.Pressed) {
				acao = true;
				indiceControle = 1 + i;
			}
			// zoom
			if (prevState [i].Triggers.Left != 0 && state [i].Triggers.Left != 0) {
				zoom = true;
				indiceControle = 1 + i;
			}
			// up
			if (prevState [i].DPad.Up == ButtonState.Released && state [i].DPad.Up == ButtonState.Pressed) {
				upC = true;
				indiceControle = 1 + i;
			}
			// down
			if (prevState [i].DPad.Down == ButtonState.Released && state [i].DPad.Down == ButtonState.Pressed) {
				downC = true;
				indiceControle = 1 + i;
			}
			// voltar
			if(prevState[i].Buttons.B == ButtonState.Released && state[i].Buttons.B == ButtonState.Pressed) {
				voltar = true;
				indiceControle = 1 + i;
			}
			// apagar
			if(prevState[i].Buttons.A == ButtonState.Released && state[i].Buttons.A == ButtonState.Pressed) {
				inserir = true;
				indiceControle = 1 + i;
			}
        }*/

        if(indiceControle != indiceControleAnterior) {
			if(ControleMudou != null)
            	ControleMudou();
        }
    }

	public static void ResetaControleMudou() {
		ControleMudou = null;
	}	
}
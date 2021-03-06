﻿using UnityEngine;
using System.Collections;
using XInputDotNetPure;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct Point {
	public int X;
	public int Y;

	public Point(int x, int y) {
		this.X = x;
		this.Y = y;
	}
}

public class Game_JoystickClicker : MonoBehaviour {

	[DllImport("user32.dll")]
	public static extern bool SetCursorPos (int X, int Y);
	[DllImport("user32.dll")]
	public static extern bool GetCursorPos (out Point pos);


	Point cursorPos = new Point();
	// Joystick
	GamePadState state;
	GamePadState prevState;
	// Update is called once per frame
	void Update () {
		state = GamePad.GetState(0);

		if (!state.IsConnected && !Player.pausado)
			return;

		GetCursorPos(out cursorPos);

		if (state.Buttons.A == ButtonState.Pressed && prevState.Buttons.A == ButtonState.Released) {
			MouseOperations.MouseEvent (MouseOperations.MouseEventFlags.LeftUp | MouseOperations.MouseEventFlags.LeftDown);
		}

		if(state.DPad.Down == ButtonState.Pressed) {
			if(cursorPos.Y < Screen.height - 5) {
				SetCursorPos (cursorPos.X, cursorPos.Y + 5);
			}
		}
		else if(state.DPad.Up == ButtonState.Pressed) {
			if(cursorPos.Y > 5) {
				SetCursorPos (cursorPos.X, cursorPos.Y - 5);
			}
		}

		if(state.DPad.Left == ButtonState.Pressed) {
			if(cursorPos.X > 5) {
				SetCursorPos (cursorPos.X - 5, cursorPos.Y);
			}
		}
		else if(state.DPad.Right == ButtonState.Pressed) {
			if(cursorPos.X < Screen.width - 5) {
				SetCursorPos (cursorPos.X + 5, cursorPos.Y);
			}
		}

		prevState = state;
	}


	public static void ResetaPosicao() {
		SetCursorPos ((int) Screen.width / 2, (int) Screen.height / 2);
	}
}

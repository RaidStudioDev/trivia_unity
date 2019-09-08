using UnityEngine;
using UnityEngine.UI;
using HeathenEngineering.OSK.v2;
using System.Collections;
using UnityEngine.EventSystems;

public class KeyboardController : MonoBehaviour {

	[HideInInspector] public Text outputText;

	public OnScreenKeyboard keyboard;

	private static KeyboardController keyboardController;
	public static KeyboardController Instance () {
		if (!keyboardController) {
			keyboardController = FindObjectOfType(typeof (KeyboardController)) as KeyboardController;
			if (!keyboardController)
				Debug.LogError ("There needs to be one active KeyboardController script on a GameObject in your scene.");
		}

		return keyboardController;
	}

	/*
	 * 
	 *    -> keyboard.KeyPressed += new KeyboardEventHandler(OnKeyboardKeyPressed);
	 * 
	 void OnKeyboardKeyPressed(OnScreenKeyboard sender, OnScreenKeyboardArguments args)
	{
		print ("OnKeyboardKeyPressed.type: " + args.KeyPressed.type);

		switch (args.KeyPressed.type) 
		{
			case KeyClass.Backspace:
				if (outputText.text.Length > 0) outputText.text = outputText.text.Substring(0, outputText.text.Length -1);
				break;
			case KeyClass.Return:
				break;
			case KeyClass.Shift:
				break;
			case KeyClass.String:
				outputText.text += args.KeyPressed.ToString();
				break;
		}
	}
	 */
}

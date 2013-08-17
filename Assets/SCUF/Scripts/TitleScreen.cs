using UnityEngine;
using System.Collections;

/// <summary>
/// Script for a basic title screen 
/// </summary>
public class TitleScreen : MonoBehaviour {

	public static TitleScreen Script;
	
	// Menu stuff
	private delegate void GUIMethod();
	private GUIMethod currentMenu;

	// Screen stuff
	float fScreenX;
	float fScreenY;
	float fMenuHeight = 100;
	float fMenuWidth = 400;

	// Background variables
	public GUISkin skin;
	public int guiDepth = 0;
	public Texture2D titleScreenBG;
	private Rect titleScreenBGPos = new Rect();
	
	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	// Use this for initialization
	void Start () {
	
		Script = this;
	}

	/// <summary>
	///
	/// </summary>
	void Awake() {

		currentMenu = MainMenu;

		// Get the menu size
		fScreenX = Screen.width * 0.5f - fMenuWidth * 0.5f;
		fScreenY = Screen.height * 0.5f - fMenuHeight * 0.5f;

		// Background
		titleScreenBGPos.x = 0;
		titleScreenBGPos.y = 0;

		titleScreenBGPos.width = Screen.width;
		titleScreenBGPos.height = Screen.height;
	}

	/// <summary>
	///
	/// </summary>
	void OnGUI() {

		GUI.skin = skin;

		// Draws the background texture
		GUI.DrawTexture(titleScreenBGPos, titleScreenBG);

		currentMenu();
	}

	/*
	 * ===========================================================================================================
	 * MENU'S DEFINITIONS
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Main Menu
	/// </summary>
	void MainMenu() {

		GUILayout.BeginArea(new Rect(fScreenX, fScreenY, fMenuWidth, fMenuHeight));
		{
			if(GUILayout.Button("Start game")) {

			}
			else if(GUILayout.Button("Options")) {

			}
			else if(GUILayout.Button("Tutorial")) {

			}
			else if(GUILayout.Button("Exit game")) {

				Application.Quit();
			}	
		}
		GUILayout.EndArea();
	}
}

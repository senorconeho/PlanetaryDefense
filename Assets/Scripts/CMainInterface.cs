using UnityEngine;
using System.Collections;

/// <summary>
/// Games' main interface
/// </summary>
public class CMainInterface : MonoBehaviour {

	public GUISkin skin; 
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI() {

		GUI.skin = skin;
		
		// "Health"
		GUI.Label(new Rect(10,Screen.height - (10+25),200,100),"Population: ");
	}
}

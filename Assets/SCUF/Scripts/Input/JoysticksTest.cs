using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class name and description
/// </summary>
public class JoysticksTest : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC

	// PRIVATE

	// PROTECTED

	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */
	//
	void Awake() {

	}

	// Use this for initialization
	void Start () {
	
		CheckJoysticks();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Physics
	void FixedUpdate() {

	}

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	/// <summary>
	/// Check if there joysticks connected to the system
	/// </summary>
	void CheckJoysticks() {

		if(Input.GetJoystickNames().Length < 1) {

			// DEBUG
			Debug.LogWarning("No joysticks found.");
		}
		else {

			int nIdx = 0;
			foreach(string stName in Input.GetJoystickNames()) {

				Debug.Log("Joystick [" + nIdx + "] : " + stName);
				
				//}
				nIdx++;
			}

		}
	}

	void OnGUI() {

		string stText = "";
		int nIdx = 0;
			foreach(string stName in Input.GetJoystickNames()) {

				stText += stName + "\n";

				for(int nAxis = 1; nAxis <= 7; nAxis++) {

					string stAxisName = "J" + (nIdx+1) + "_" + nAxis + "_axis";
					float fValue = Input.GetAxis(stAxisName);
					stText += " Axis " + nAxis + " " + String.Format("{0:0.0}", fValue) + "\n";
				}

				//}
				nIdx++;
			}
		
		GUI.Label(new Rect(50, 50, 500, 500), stText);
	}
}

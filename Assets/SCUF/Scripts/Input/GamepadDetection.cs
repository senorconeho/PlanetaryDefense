using UnityEngine;
using System.Collections;

/// <summary>
/// Recognizes a gamepad plugged in the system and configures it according to the OS
/// \author	Alexandre Ramos Coelho
/// \version 0.3
/// \date 21/10/2012
/// </summary>
public class GamepadDetection : MonoBehaviour {

	// PUBLIC
	public static GamepadDetection Script;
	public bool		bnUsingGamepad = false;

	// PRIVATE
	MiscSysDetection scriptSysDetection;
	string stGamepadName;
	int nGamepadIdx = -1;
	
	string stStickLX;	//< Name for the X axis on the left stick
	string stStickLY;	//< Name for the Y axis on the left stick
	string stStickRX; //< Name for the X axis on the right Stick
	string stStickRY; //< Name for the Y axis on the right Stick
	string stTriggerL;	//< Name for the axis for the Left Trigger
	string stTriggerR;	//< Name for the axis for the Right Trigger
	string stButtonShoulderL; //< Name for the left button on the shoulder
	string stButtonShoulderR; //< Name for the right button on the shoulder
	string stButtonA; //< Name for the A or Green button on the gamepad
	string stButtonB; //< Name for the B or Red button on the gamepad
	string stButtonX; //< Name for the X or Blue button on the gamepad
	string stButtonY; //< Name for the Y or Yellow button on the gamepad
	string stDPadX;	//< Digital Pad X axis
	string stDPadY; //< Digital Pad Y axis

	Vector2 v2LeftStickLastRead = Vector2.zero;
	Vector2 v2RightStickLastRead = Vector2.zero;
	float fStickSensitivity = 0.1f;
	float fTriggerSensitivity = 0.15f;	//< Minimum value to consider any of the trigger buttons pressed

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	void Awake() {

	}

	// Use this for initialization
	void Start () {

		Script = this;
	
		scriptSysDetection = this.GetComponent<MiscSysDetection>();
		if(!scriptSysDetection) {

			// DEBUG
			Debug.LogError("Failed to find the MiscSysDetection component.");
		}

		CheckJoysticks();
		// FIXME: now working only for XBox 360 gamepads
		//SetupXBox360Gamepad();
		SetupJoysticksAxis();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
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

				// DEBUG
				Debug.Log("Joystick [" + nIdx + "] : " + stName);
				
				// FIXME
				// For now, we only check for XBox 360 gamepads
				//if(stName == "XBOX 360") {

					stGamepadName = stName;
					nGamepadIdx = nIdx;

				//}
				nIdx++;
			}

			bnUsingGamepad = true;
		}
	}

	/// <summary>
	/// Setup the axis for all found joysticks
	/// </summary>
	void SetupJoysticksAxis() {

		int nIdx = 0;

		foreach(string stJoyName in Input.GetJoystickNames()) {

			// First, the known joysticks
			switch(stJoyName) {

				case "Controller (XBOX 360 For Windows)":
					SetupXBox360GamepadOnWindows(nIdx);
					break;

				case "USB Gamepad":
					SetupUSBGamepad(nIdx);
					break;

				case "Twin USB Joystick":
					SetupUSBGamepad(nIdx);
					break;

				default:
					SetupUSBGamepad(nIdx);
					break;
			}
		}
	}

	/// <summary>
	/// Setup the XBOX 360 Controller on Windows
	/// </summary>
	void SetupXBox360GamepadOnWindows(int nControllerIdx) {

		nControllerIdx += 1;
		// DEBUG
		Debug.Log("Controller [" + nControllerIdx + "]: Setting up XBOX 360 Windows");

		// Setup the gamepad on windows
		stStickLX		= "J" + nControllerIdx + "_1_axis";
		stStickLY		= "J" + nControllerIdx + "_2_axis";
		stStickRX		= "J" + nControllerIdx + "_4_axis";
		stStickRY		= "J" + nControllerIdx + "_5_axis";
		stTriggerL	= "J" + nControllerIdx + "_3_axis"; // Caution: -1 fully depressed, 0 not pressed
		stTriggerR	= "J" + nControllerIdx + "_3_axis"; // Caution: 1 fully depressed, 0 not pressed
		stDPadX			= "J" + nControllerIdx + "_6_axis";
		stDPadY			= "J" + nControllerIdx + "_7_axis";
	}

	/// <summary>
	/// Setup the XBOX 360 Controller on Windows
	/// </summary>
	void SetupUSBGamepad(int nControllerIdx) {

		nControllerIdx += 1;
		// DEBUG
		Debug.Log("Controller [" + nControllerIdx + "]: Setting up Generic USB Gamepad");

		// Setup the gamepad on windows
		stStickLX		= "J" + nControllerIdx + "_1_axis";
		stStickLY		= "J" + nControllerIdx + "_2_axis";
		stStickRX		= "J" + nControllerIdx + "_4_axis";
		stStickRY		= "J" + nControllerIdx + "_3_axis";
		stDPadX			= "J" + nControllerIdx + "_5_axis";
		stDPadY			= "J" + nControllerIdx + "_6_axis";
	}
	
	/// <summary>
	/// Setup the XBOX 360 Controller
	/// </summary>
	void SetupXBox360Gamepad() {

		if(scriptSysDetection.stRunningOS == "Windows") {

			// Setup the gamepad on windows
			stStickLX = "X axis";
			stStickLY = "Y axis";
			stStickRX = "4th axis";
			stStickRY = "5th axis";
			stTriggerL = "3rd axis"; // Caution: -1 fully depressed, 0 not pressed
			stTriggerR = "3rd axis"; // Caution: 1 fully depressed, 0 not pressed
			stButtonShoulderL = "joystick button 4";
			stButtonShoulderR = "joystick button 5";
			stButtonA = "joystick button 0";
			stButtonB = "joystick button 1";
			stButtonX = "joystick button 2";
			stButtonY = "joystick button 3";
		}
	}

	/// <summary>
	/// Reads the left stick input values
	/// </summary>
	/// <returns> A Vector2 with the input values for the x and y axis</returns>
	public Vector2 ReadLeftStick() {
		
		Vector2 v2ReadValue;
		Vector2 v2ReturnValue;

		v2ReadValue.x = Input.GetAxis(stStickLX);
		v2ReadValue.y = Input.GetAxis(stStickLY);
		
		if(Mathf.Abs(v2ReadValue.x) < fStickSensitivity)
			v2ReadValue.x = 0.0f;
		if(Mathf.Abs(v2ReadValue.y) < fStickSensitivity)
			v2ReadValue.y = 0.0f;

		if( (Mathf.Abs(v2ReadValue.x - v2LeftStickLastRead.x) < fStickSensitivity) || 
			(Mathf.Abs(v2ReadValue.y - v2LeftStickLastRead.y) < fStickSensitivity)) {
			
				v2ReturnValue = v2LeftStickLastRead;
		}
		else
			v2ReturnValue = v2ReadValue;

		v2LeftStickLastRead = v2ReadValue;

		return v2ReturnValue;
	}

	/// <summary>
	/// Reads the right stick input values
	/// </summary>
	/// <returns> A Vector2 with the input values for the x and y axis</returns>
	public Vector2 ReadRightStick() {
		
		Vector2 v2ReadValue;
		Vector2 v2ReturnValue;

		v2ReadValue.x = Input.GetAxis(stStickRX);
		v2ReadValue.y = Input.GetAxis(stStickRY);
		
		if(Mathf.Abs(v2ReadValue.x) < fStickSensitivity)
			v2ReadValue.x = 0.0f;
		if(Mathf.Abs(v2ReadValue.y) < fStickSensitivity)
			v2ReadValue.y = 0.0f;

		if( (Mathf.Abs(v2ReadValue.x - v2RightStickLastRead.x) < fStickSensitivity) || 
			(Mathf.Abs(v2ReadValue.y - v2RightStickLastRead.y) < fStickSensitivity)) {
			
				v2ReturnValue = v2RightStickLastRead;
		}
		else
			v2ReturnValue = v2ReadValue;

		v2RightStickLastRead = v2ReadValue;

		return v2ReturnValue;
	}

	/// <summary>
	/// Reads the shoulders buttons
	/// </summary>
	public bool ReadLeftButton() {

		bool rv = false;

		if(Input.GetButton(stButtonShoulderL))
			rv = true;

		return rv;
	}

	/// <summary>
	/// Reads the shoulders buttons
	/// </summary>
	public bool ReadRightButton() {

		bool rv = false;

		if(Input.GetButton(stButtonShoulderR))
			rv = true;

		return rv;
	}

	/// <summary>
	/// Reads the trigger button value. It's goes from 0 to 1 (fully pressed)
	/// </summary>
	/// <returns> A float with the value </returns>
	public float ReadRightTrigger() {

		return Input.GetAxis(stTriggerR);
	}

	/// <summary>
	/// Reads the trigger as a button (pressed or not). Uses a threshold value for this
	/// </summary>
	public bool ReadRightTriggerButton() {

		bool rv = false;

		if(Input.GetAxisRaw(stTriggerR) > fTriggerSensitivity)
			rv = true;

		return rv;
	}

	/// <summary>
	/// Reads the trigger button value. It's goes from -1 to 0 (fully pressed)
	/// </summary>
	/// <returns> A float with the value </returns>
	public float ReadLeftTrigger() {

		return Input.GetAxis(stTriggerL);
	}

	/// <summary>
	/// Reads the trigger as a button (pressed or not). Uses a threshold value for this
	/// </summary>
	public bool ReadLeftTriggerButton() {

		bool rv = false;

		float fReadValue = Input.GetAxisRaw(stTriggerL);

		if(fReadValue < -fTriggerSensitivity)
			rv = true;

		return rv;
	}
}

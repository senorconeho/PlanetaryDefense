using UnityEngine;
using System.Collections;

/// <summary>
/// Basic script for system detection. Checks the OS. Doxygen compatible formatting
/// \version 0.2
/// \author	Alexandre Ramos Coelho
///	\date	30-07-2012
/// </summary>
public class MiscSysDetection : MonoBehaviour {

	// PUBLIC
	public string stRunningOS = null; ///< Operating system that the application is running
	public static bool bnIsUnityPro; ///< Whether is Unity Pro or not
	public static bool bnIsRunningAtWeb; ///< Whether is a web application or not
	public static MiscSysDetection Script = null;	///< Shortcut to this script
	
	/*
	 * ===========================================================================================================
	 * UNITY'S STUFF
	 * ===========================================================================================================
	 */

	/// <summary>
	/// When the script is initialized
	/// </summary>
	void Awake () {
	
		Script = this;
		CheckRunningOS();
	}

	/// <summary>
	/// Use this for initialization
	/// </summary>
	void Start () {
	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// Can be replaced by LateUpdate and FixedUpdate
	/// </summary>
	void Update () {
	
	}

	/*
	 * ===========================================================================================================
	 * CLASS METHODS
	 * ===========================================================================================================
	 */

	/// <summary>
	/// Check the Operating System that we are running, and if we a standalone application or a embedded 
	/// application in a browser
	/// </summary>
	public void CheckRunningOS() {

		if(Application.platform == RuntimePlatform.WindowsPlayer || 
				Application.platform == RuntimePlatform.WindowsWebPlayer || 
				Application.platform == RuntimePlatform.WindowsEditor) {

			stRunningOS = "Windows";
		}
		else if(Application.platform == RuntimePlatform.OSXPlayer || 
				Application.platform == RuntimePlatform.OSXWebPlayer || 
				Application.platform == RuntimePlatform.OSXEditor) {

			stRunningOS = "OSX";
		}

		// Check is a web app
		if(Application.platform == RuntimePlatform.WindowsWebPlayer || 
				Application.platform == RuntimePlatform.OSXWebPlayer) {

			bnIsRunningAtWeb = true;
		}
	}
}

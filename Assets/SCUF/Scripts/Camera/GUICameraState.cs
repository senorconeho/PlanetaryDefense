using UnityEngine;
using System.Collections;

/// <summary>
/// Make a simple and auto-scaling menu interface
/// Taken from: http://www.doorapps.com/2012/07/21/easy-gui-with-nice-scaling/
/// </summary>
public class GUICameraState : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	public static float fCameraWidth;
	public static float fCameraHeight;
	// PRIVATE
	// PROTECTED
	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */
	// Awak
	void Awake() {

		// check that we are on a camera!
		if( camera == null ) {

			// DEBUG
			Debug.LogError("GUIcameraState must be used on a camera!");
			Destroy(this);
			return;
		}

		fCameraHeight = 2f * camera.orthographicSize;
		fCameraWidth = fCameraHeight * camera.aspect;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

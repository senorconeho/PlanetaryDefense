using UnityEngine;
using System.Collections;

/// <summary>
/// Make a simple and auto-scaling menu interface
/// Taken from: http://www.doorapps.com/2012/07/21/easy-gui-with-nice-scaling/
/// </summary>
public class FixGUIPlaneSize : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	// PRIVATE
	float			fWidthScale;
	float			fHeightScale;
	float			fScale;
	Transform	tr;
	// PROTECTED
	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */
	// Awak
	void Awake() {

	}

	// Use this for initialization
	void Start () {
	
		tr = this.transform;
		fWidthScale = GUICameraState.fCameraWidth / tr.localScale.x;
		fHeightScale = GUICameraState.fCameraHeight / tr.localScale.z;

		if(fHeightScale < fWidthScale)
			fScale = fHeightScale;
		else
			fScale = fWidthScale;

		transform.localScale *= fScale;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}

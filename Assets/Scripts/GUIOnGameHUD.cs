using UnityEngine;
using System.Collections;

/// <summary>
/// This script will take care of the main screen GUI, using NGUI elements
/// </summary>
public class GUIOnGameHUD : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	public Camera			GUICamera;				//< The camera from NGUI (usually 'UI Root (2D)/Camera')
	public Transform	trUI;							//< The main object for the NGUI (usually 'UI Root (2D)' in the hierarchy)
	
	// PRIVATE
	private Transform	trPopulationBar;		//< Transform for the population bar
	private UISlider	uiPopulationBar;		//< Script for the main population bar
	private Transform	trPopulationLabel;	//< Transform for the population bar
	
	// PROTECTED

	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */
	// Use this for initialization
	void Start () {

		if(!trUI) {

			// DEBUG
			Debug.LogError(this.transform + " trUI object not set!");
			return;
		}

		// Ok, find the objects in the hierarchy by itself
		trPopulationBar = trUI.Find("PopulationBar");
		if(!trPopulationBar) {

			// DEBUG
			Debug.Log(this.transform + " Cannot find the PopulationBar object");
		}
		uiPopulationBar = trPopulationBar.gameObject.GetComponent<UISlider>();	

		trPopulationLabel = trUI.Find("PopulationLabel");
		//GUICamera = trUI.Find("Camera").camera;

		PositionPopulationUI();
	}
	
	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	/// Updates the population bar a building is hit or someone dies.
	/// </summary>
	public void UpdatePopulationBar(float fNewValue) {

		uiPopulationBar.sliderValue = fNewValue;
	}

	/// <summary>
	/// Reposition the Population Bar and Label accordingly to the size of the screen
	/// </summary>
	public void PositionPopulationUI() {

		Vector3 v3Offset = new Vector3(0, 20, 0);

		// For now, we gonna put this bar in the top of the screen
		Vector2 v2ScreenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);

		v3Offset.y = v2ScreenCenter.y - v3Offset.y;

		trPopulationBar.localPosition += v3Offset;
		trPopulationLabel.localPosition += v3Offset;
	}
}

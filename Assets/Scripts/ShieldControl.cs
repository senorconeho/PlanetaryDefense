using UnityEngine;
using System.Collections;

/// <summary>
/// Controls the color shield
/// </summary>
public class ShieldControl : MonoBehaviour {

	public Transform trShield = null;
	public float fSpinSpeed = 30.0f;

	// Use this for initialization
	void Start () {
	
		
	}
	
	// Update is called once per frame
	void Update () {
	
		// Spin the shield
		if(trShield) {

			trShield.transform.Rotate(new Vector3(0,0,fSpinSpeed) * Time.deltaTime);
		}
	}


	/// <summary>
	/// Set the material color for all the shield object children
	/// </summary>
	public void SetMaterialColor(Color myNewColor) {

		foreach(Transform child in trShield) {

			child.gameObject.renderer.material.color = myNewColor;
		}
	}
}

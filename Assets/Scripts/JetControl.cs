using UnityEngine;
using System;
using System.Collections;

/// <summary>
/// Class name and description
/// </summary>
public class JetControl : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	public float fMaxForce = 300.0f;
	
	// Common forces
	public float fGravity = 9.8f;
	float fThrust = 0.0f;
	float fGravityTorque = 0.0f;
	Vector3 v3GravityForce = Vector3.zero;
	float fVerticalMovement = 0.0f;
	float fJointAnchorY = 0.0f;


	// PRIVATE
	private HingeJoint hingeJoint;
	

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

		hingeJoint = GetComponent<HingeJoint>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Physics
	void FixedUpdate() {

		fJointAnchorY = hingeJoint.anchor.y;

		// Calculate the gravity force - only on vertical axis
		fVerticalMovement = fGravity * Time.deltaTime;

		// Apply "gravity" on the y component of the anchor in the joint:w
		fJointAnchorY -= fVerticalMovement;

		Vector3 v3NewAnchorPosition = new Vector3(0, fJointAnchorY, 0);

		// Update the anchor
		//hingeJoint.anchor = v3NewAnchorPosition;



		if(Input.GetKey(KeyCode.Joystick1Button5) || Input.GetKey(KeyCode.Space)) {

			//rigidbody.AddForce(Vector3.up * fMaxForce);
			rigidbody.AddForce(Vector3.right * fMaxForce);
		}
	}

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	void OnGUI() {
		// DEBUG the forces
		string stText = "V: " + String.Format("{0:0.00}", fVerticalMovement) + "\n" 
			+ "Jy: " + String.Format("{0:0.00}",fJointAnchorY);

		GUI.Label(new Rect(0,0,100,100), stText);


	}

	void OnDrawGizmos() {

		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(transform.position, v3GravityForce);
	}
}

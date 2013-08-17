using UnityEngine;
using System.Collections;

/// <summary>
/// Makes the camera follow a target
/// The user must fill the target transform prior to running the game
/// </summary>
public class CameraFollow : MonoBehaviour {

	public Transform target;
	Vector3 v3OffsetStartingPosition;


	// Use this for initialization
	void Start () {
		
		// Check
		if(target == null) {

			// DEBUG
			Debug.LogError(this.transform + " Camera target not set");
		}

		v3OffsetStartingPosition = transform.position - target.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
	
		if(!target)
			return;

		Vector3 v3NewPosition = target.position + v3OffsetStartingPosition;
		transform.position = v3NewPosition;

		transform.LookAt(target);
	}
}

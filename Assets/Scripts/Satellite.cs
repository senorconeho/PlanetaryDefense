using UnityEngine;
using System.Collections;

/// <summary>
/// Class name and description
/// </summary>
public class Satellite : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */
	private ConfigurableJoint joint;

	public float rotationalTorque = 1;
	//How strong is the rotational force.
	public Transform orbitingBody;

	// Use this for initialization
	void Start () {
		joint = GetComponent<ConfigurableJoint>();
		JointDrive rotationDriver = joint.angularYZDrive;
		//tell Unity which rotation mode to use.
		rotationDriver.mode = JointDriveMode.Position;
		//We want to reach a specific rotation, not a velocity.
		rotationDriver.positionSpring = rotationalTorque;
		joint.angularYZDrive = rotationDriver;
		//reassign the joint.

	}

	// Update is called once per frame
	void Update () {
		Vector3 relativePos = transform.position - orbitingBody.position;
		relativePos = relativePos.normalized;

		float theta = Mathf.Acos(relativePos.x) * Mathf.Rad2Deg;
		if(relativePos.y < 0)
			theta = 360 - theta;

		Quaternion rotation = Quaternion.Euler(0 , 0 , 180 - theta);
		//The axis you use will depend on how your object is oriented. You might need to play around with this a little.
		joint.targetRotation = rotation;

	}
}

using UnityEngine;
using System.Collections;

/// <summary>
/// Simulates the dying people that runs off a 
/// </summary>
public class DyingPeople : MonoBehaviour {

	/* -----------------------------------------------------------------------------------------------------------
	 * VARIABLES DECLARATION
	 * -----------------------------------------------------------------------------------------------------------
	 */
	// Public
	[SerializeField]
	public GameObject				firePrefab 			= null;	//< Prefab to instantiate a fire in the person
	public static CBuilding	buildingScript 	= null; //< The building script itself will fill this
	// Protected
	// Private
	private float 		fMinLifeTime 	=	1.0f;	//< Person minimum life time in ms
	private float 		fMaxLifeTime 	= 4.0f;	//< Person maximum life time in ms
	float 						fLifeTime			= 0.0f;	//< Time to this person live
	int 							nDirection 		= -1;		//< Direction, -1 to the left, 1 to the right
	float							fMoveSpeed		=	3.0f;	//< Moving speed
	float							fTargetAngle;					//< computed angle, with life time and move speed
	Quaternion				qTargetRotation;			//< quaternion with the above target angle
	Vector3						v3FireOffset	= new Vector3(0,0.3f,0);	//< Offset to instantiate the fire over the person
	float							fSpawnTime;
	GameObject				goFire;								//< The fire particle object

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY'S STUFF HERE
	 * -----------------------------------------------------------------------------------------------------------
	 */

	void OnEnable() {

		fSpawnTime = Time.time;

		// Direction
		nDirection = UnityEngine.Random.Range(-1,1);
		if(nDirection == 0)
			nDirection = 1;

		// Life time
		fLifeTime = UnityEngine.Random.Range(fMinLifeTime, fMaxLifeTime);
	
		fTargetAngle = transform.parent.transform.rotation.z + ((float)nDirection * fMoveSpeed * fLifeTime);
		qTargetRotation = Quaternion.Euler(0,0,fTargetAngle);

		goFire = null;
	}

	void OnDisable() {

		goFire = null;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(goFire == null)
			CreateFire();

		// Check the lifetime
		if(Time.time > fSpawnTime + fLifeTime) {

			Die();
		}

		transform.parent.transform.rotation = 
			Quaternion.Slerp(transform.parent.transform.rotation, qTargetRotation, Time.deltaTime);
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * CLASS METHODS
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	///	Instantiate a fire particle effect over the person
	/// </summary>
	void CreateFire() {

		goFire = Spawner.Spawn(firePrefab, transform.position + v3FireOffset, Quaternion.identity) as GameObject;
		goFire.transform.parent = this.transform;
	}

	/// <summary>
	/// Kill this object
	/// </summary>
	private IEnumerator KillMyself() {

		yield return new WaitForSeconds(fLifeTime * 2);

		// Remove one from the people counter on the building
		buildingScript.OneManDown();

		Spawner.Destroy(this.transform.parent.gameObject);
	}

	private void Die() {

		// Remove one from the people counter on the building
		buildingScript.OneManDown();

		// Disables the fire
		goFire.transform.parent = null;
		Spawner.Destroy(goFire);

		// 'Destroy' itself
		Spawner.Destroy(this.transform.parent.gameObject);
	}
}

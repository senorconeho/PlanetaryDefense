using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

/// <summary>
/// Basic Class for an enemy in this game
/// </summary>
public class CEnemy : MonoBehaviour {
	
	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY'S STUFF HERE
	 * -----------------------------------------------------------------------------------------------------------
	 */

	public Color			myColor = Color.black;
	public Transform	trMesh;
	protected float		_fHealth = 1.0f;
	public float			fCurrentHealth{get{return _fHealth;}}
	protected float		_fMaxHealth = 1.5f;
	public float			fMaxHealth{get{return _fMaxHealth;}}
	float							fScoreValue = 10.0f; //< How much points this is worth

	public AudioClip	sfxHit; //< Played when this enemy is hit by the player
	public GUISkin		skin; //< For on screen text displaying
	bool							bnSetupReady = false;	//< Is this enemy already setup and done?
	private Vector3		v3MoveDirection = Vector3.zero;
	CharacterController ccController;
	float							fDistanceFromPlayer = 0.0f;
	float							fThrustDirection = 1.0f; //< Moves forward
	float							fMoveSpeed = 0.7f; //< Speed of movement

	float							fMoveTimer = 0.15f; //< How often the current movement target is recalculated
	float							fMoveCountTimer = Mathf.NegativeInfinity;	//< Timer to keep track of the last time we calculated the target

	float							fTimeToShootAgain = 1.5f;	//< When can we shoot again?
	float							fShootTimer = Mathf.NegativeInfinity;	//< Keep track of the last time we shot

	public float			fPeriod = 2.0f;
	public float			fAmplitude = 0.4f;
	
	Vector3						v3TargetPosition = Vector3.zero;	//< Position for the current target of movement
	Vector3						v3DirectionToPlayer = Vector3.zero;	//< Vector pointing to the player
	Vector3						v3ProjectPlayerOutsideRadius = Vector3.zero; //< Vector point to outside the orbit radius

	float							fOrbitingRadius = 4.0f; //< The 'orbit' is how close from the player we can get

	ShieldControl			myShieldScript;

	Transform					trTarget = null;
	Transform					trLevel = null;

	float							fAltitude = 35.0f;
	public GameObject projectilePrefab = null;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY'S STUFF
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Will execute when loaded
	/// </summary>
	void Awake() {

		myShieldScript = GetComponent<ShieldControl>();

		trLevel = GameObject.Find("Level").transform;
		
		if(trLevel == null) {

			// DEBUG
			Debug.LogError("Level object not found");
		}

		// Set up the rigidbody
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
	}

	/// <summary>
	/// Start will execute when an object with it is instantiated
	/// </summary>
	void Start () {
	
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {
	
		if(!bnSetupReady)
			return;

		TickTimers();

		// Check if we already have a target
		if(!trTarget) {

			trTarget = FindNearestTarget();

			//if(trTarget) {

			//	// DEBUG
			//	Debug.Log(this.transform + " Nearest target: " + trTarget);
			//}
		}

		// Do the movement
		DoMovement();

		// Shoot if we have a target
		if(trTarget) {

			DoShooting();
		}

	}
	
	/// <summary>
	///
	/// </summary>
	void OnGUI() {

		GUI.skin = skin;

		if(ShooterControl.bnDebugEnabled) {
			
			// DEBUG
			string stText = "H: " + String.Format("{0:0.0}", _fHealth) + "\nD: " + String.Format("{0:0.0}", fDistanceFromPlayer);

			Vector3 v3TextPosition = Camera.main.WorldToScreenPoint(trMesh.transform.position);
			GUI.Label(new Rect(v3TextPosition.x, Screen.height - v3TextPosition.y, 100, 100), stText);
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * MY STUFF HERE
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Tick the timers
	/// </summary>
	void TickTimers() {

		// Tick the movement timer.
		fMoveCountTimer -= Time.deltaTime;

		// Shooting timer
		fShootTimer -= Time.deltaTime;
	}


	/// <summary>
	/// Enemy takes damage
	/// </summary>
	/// <param name="fAmount"> A float with the amount of damage taken </param>
	public void TakeDamage(float fAmount) {

		_fHealth -= fAmount;

		if(_fHealth <= 0) {

			Die();
		}
	}

	/// <summary>
	/// Enemy heals or gain health somehow
	/// </summary>
	/// <param name="fAmount"> Amount of health gained </param>
	public void HealDamage(float fAmount) {

		_fHealth = Mathf.Min(_fHealth + fAmount, fMaxHealth);
	}

	/// <summary>
	/// Enemy dies
	/// </summary>
	protected void Die() {

		//ShooterControl.Script.scriptScore.SetInfoText( String.Format("{0:0.0}", fScoreValue), this.transform.position);
		Spawner.Destroy(gameObject);
	}

	/// <summary>
	/// Get this enemy color
	/// </summary>
	/// <returns> The color of this enemy. Useful when shot by a colored laser from the player </returns>
	public Color GetColor() {

		return myColor;
	}

	/// <summary>
	/// Set this enemy color. Actually, sets the color in it's material. The color is a key aspect of this game
	/// </summary>
	/// <param name="newColor"> The color index from the ColorWheel array</param>
	/// <param name="fAltitude"> The starting altitude of the enemy. Remember to take the planet radius in account</param>
	/// <param name="fAngle"> The starting angle of the enemy (the pivot is in the planet center)</param>
	public void Setup(Color newColor, float fAltitude, float fAngle) {

		myColor = newColor;
		trMesh.gameObject.renderer.material.color = myColor;

		if(myShieldScript) {

			myShieldScript.SetMaterialColor(myColor);
		}

		// Set the level as parent
		this.transform.parent = trLevel.transform;

		// Reposition the enemy. Actually, we place the mesh in the height we want and rotate it from the center
		Vector3 newPosition = new Vector3(0, fAltitude, 0);
		trMesh.transform.position = newPosition;
		// Initial rotation
		Quaternion newRotation = Quaternion.Euler(0, 0, fAngle);
		transform.rotation = newRotation; // The rotation is over the parent, not the mesh

		bnSetupReady = true;
	}

	/// <summary>
	/// This will run when this enemy is hit by a laser beam. It receives it's colors values
	/// </summary>
	/// <param name="hitColor"> The Color of the laser beam that hit us </param>
	public void HitBy(Color hitColor, float fDamage) {

		if(hitColor == myColor) {

			TakeDamage(fDamage);
		}
	}

	/// <summary>
	/// Do the movement for this unit
	/// </summary>
	void DoMovement() {

		// Move the enemy up and down, to create a sensation of movement
		trMesh.transform.Translate(Vector3.up * Time.deltaTime * Mathf.Sin(Time.time + UnityEngine.Random.Range(-0.4f, 0.4f)), Space.Self);

		if(trTarget) {


			// TODO: move the enemy to a position (angle) near the target position, not the exactly position
			// Also, it will be good if the enemy moves left and right too. Maybe put a timer here?

			// Move towards the target
			transform.rotation = Quaternion.Slerp(transform.rotation, trTarget.transform.parent.transform.rotation, Time.deltaTime);
		}
	}

	/// <summary>
	/// Shoot the enemy. It works on a timer
	/// </summary>
	void DoShooting() {

		if(fShootTimer > 0)
			return;

		// Resets the timer
		fShootTimer = fTimeToShootAgain + UnityEngine.Random.Range(-0.2f,2.0f);

		Quaternion qRotation = Quaternion.LookRotation(v3TargetPosition - trMesh.transform.position, Vector3.forward);
		GameObject goProjectile = Spawner.Spawn(projectilePrefab, trMesh.transform.position, qRotation) as GameObject;
		
		// Set the projectile color
		CBaseProjectile scriptProjectile = goProjectile.GetComponent<CBaseProjectile>();
		scriptProjectile.Setup(this.transform, myColor);
	}

	/// <summary>
	/// Find the nearest target from this enemy.
	/// </summary>
	/// <returns> The transform of the nearest target, or null if none is found </returns>
	Transform FindNearestTarget() {

		Transform trNearestTarget = null;
		float fMinDistance = Mathf.Infinity;

		foreach(Transform tr in ShooterControl.lBuildings) {

			float fDistance = (trMesh.transform.position - tr.position).sqrMagnitude;

			if(fDistance < fMinDistance) {

				// Closer than the previous
				fMinDistance = fDistance;
				trNearestTarget = tr;
			}
		}

		return trNearestTarget;
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * DEBUG STUFF
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Draw Gizmos
	/// </summary>
	void OnDrawGizmos() {

		Gizmos.color = Color.red;
		Gizmos.DrawRay(transform.position, transform.forward * 2);

		//Gizmos.color = Color.white;
		//Gizmos.DrawWireSphere(v3TargetPosition, 0.5f);

		Gizmos.DrawRay(transform.position, v3DirectionToPlayer.normalized * 1.2f);

	}
}

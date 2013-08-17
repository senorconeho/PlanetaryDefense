using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class for the game cannon
/// </summary>
public class CCannon : MonoBehaviour {

	/* -----------------------------------------------------------------------------------------------------------
	 * CLASS VARIABLES
	 * -----------------------------------------------------------------------------------------------------------
	 */

	// PUBLIC
	public static			CCannon Script; 						//< shortcut to this script
	public Transform	trCannon 						= null; //< The transform of the cannon model (which rotate to aim)
	public Transform	trShotOrigin				= null; //< Where the bullets are originated from
	public Transform	trCannonBody				= null;	//< The cannon body, which is ALWAYS perpendicular to the planet surface
	public Transform 	trPlayerControlling	= null; //< Who is controlling this cannon?
	public Texture 		txAimCursor;								//< Texture for the icon itself
	public GameObject	projectilePrefab 		= null;	//< Prefab for the projectile

	public bool 			bnIsManned { get { return m_bnIsManned;	} set {	m_bnIsManned = value;	}	}
	public int 				nIdx { get { return m_nIdx; }	set{ m_nIdx = value; } }


	// PRIVATE
	Vector2 					v2AimCursor 				= Vector2.zero;	//< Position of the cursor around the player
	Vector3						v3AimDirection 			= Vector3.zero; //< the direction where are we aiming
	float 						fAimCursorRadius 		= 100.0f;	//< Radius of the cursor around the player
	float 						fAimCursorSize 			= 32.0f;	//< Size of the cursor icon
	float 						fShootTimer 				= 0.0f;	//< Timer to control the shots
	float 						fProjectileInterval	= .2f; //< Time between one shot and another
	Quaternion				qStartRotation 			= Quaternion.identity;
	Vector2 					v2ScreenCenter 			= Vector2.zero;
	ShooterControl		mainScript 					= null;
	float							fCannonAngle				= 0f;	//< Current angle of the cannon with the up vector
	float							fMaxCannonAngle			= 75f;	//< Max cannon rotation (in degrees)

	// PROTECTED
	protected bool 		m_bnIsManned 				= false;	//< Are we controlling this cannon?
	protected int 		m_nIdx 							= 0;			//< This cannon index in the cannon list

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY'S STUFF HERE
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Will execute when loaded
	/// </summary>
	void Awake() {

		Script = this;

		// Get the main script
		mainScript = GameObject.Find("/GameCode").GetComponent<ShooterControl>();

		if(!mainScript) {

			// DEBUG
			Debug.LogError(this.transform + " could get the ShooterControl from Code");
		}

		RegisterMyselfWithMainScript();
	}

	/// <summary>
	/// Start will execute when an object with it is instantiated
	/// </summary>
	void Start () {
	
		qStartRotation = trCannon.transform.rotation;

		v2ScreenCenter.x = Screen.width * 0.5f;
		v2ScreenCenter.y = Screen.height * 0.5f;
	}
	
	// Update is called once per frame
	void Update () {

		TickTimers();

		if(!bnIsManned) {
			
			// Restore the cannon to it's initial aiming position
			//trCannon.rotation = Quaternion.Slerp(trCannon.rotation, qStartRotation, Time.deltaTime);

			return;
		}

		// Check to see where we are aiming
		CheckAim();
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * CLASS METHODS
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Check the input to show where we are aiming this cannon
	/// </summary>
	void CheckAim() {
		
		if(ShooterControl.bnUseMouse) {

			//Vector2 v2MousePosition = Input.mousePosition;
			//v2AimCursor.x = v2MousePosition.x - fAimCursorSize * 0.5f;
			//v2AimCursor.y = v2MousePosition.y + fAimCursorSize * 0.5f;

			// Mouse position in the world
			//Vector3 v3MousePositionInTheWorld = 
			//	Camera.main.camera.ScreenToWorldPoint(new Vector3(v2AimCursor.x, v2AimCursor.y, 0.0f));

			Vector3 v3MousePositionInTheWorld = 
				Camera.main.camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0.0f));

			// Creates the cannon aim line
			//v3AimDirection = new Vector3(v3MousePositionInTheWorld.x - trCannon.transform.position.x, 
			//		v3MousePositionInTheWorld.y - trCannon.transform.position.y, 0.0f);
			v3AimDirection = v3MousePositionInTheWorld - trCannon.transform.position;
			v3AimDirection.z = 0;

			v2AimCursor.y = Screen.height - v2AimCursor.y;

			// The angle between 
			fCannonAngle = Vector3.Angle(trCannonBody.up, v3AimDirection);
		}

		if(ShooterControl.bnUseGamepad) {

			//  Controller stuff
			Vector2 v2AimStickPosition = ShooterControl.scriptGamepad.ReadLeftStick();

			v3AimDirection = new Vector3(v2AimStickPosition.x, v2AimStickPosition.y, 0);
			// The angle created by the left stick
			fCannonAngle = Vector3.Angle(Vector3.up, v3AimDirection);
		}

		//if(v3AimDirection == Vector3.zero) {

		//	v3AimDirection = Vector3.up;
		//}
		//else {

		//	v3AimDirection.Normalize();
		//}

		//fCannonAngle = Vector3.Angle(trCannon.parent.up, v3AimDirection);
		// Trying a new method to set the cannon rotation: 
		// 1 - read the gamepad input
		// 2 - normalize the input vector
		// 3 - Calculate the input vector angle
		// 4 - Clamp the input vector within a desired angle
		// 5 - Create a quaternion rotation using this angle

		// Corrects the angle signal
		if(ShooterControl.bnUseMouse) {
			if(Mathf.Abs(fCannonAngle) > fMaxCannonAngle) {

				fCannonAngle = fMaxCannonAngle;
			}

			if(v3AimDirection.x > 0) {

				fCannonAngle *= -1;
			}

		}
		else {
			if(fCannonAngle > fMaxCannonAngle) {

				fCannonAngle = fMaxCannonAngle;
			}


			// Ok, we have the angle but we don't know if it is n degrees to the left or to the right
			// So, let's see how many degrees this aim vector have from the left vector
			float fCheckSignalAngle = Vector3.Angle(trCannonBody.right, v3AimDirection);
			if(fCheckSignalAngle < 90)
				fCannonAngle *=-1;
		}
		
			// Apply the rotation
			Quaternion qRotation = Quaternion.identity;
			qRotation.eulerAngles = new Vector3(0,0,fCannonAngle);
			trCannon.localRotation = qRotation;

	}

	/// <summary>
	/// Tick the game timers
	/// </summary>
	void TickTimers() {

		fShootTimer += Time.deltaTime;
	}

	/// <summary>
	/// Shoot this cannon
	/// </summary>
	/// <param name="shotColor"> The color to be shot </param>
	public void Shoot(Color shotColor) {

		if(fShootTimer < fProjectileInterval)
			return;

		fShootTimer = 0.0f;

		// Create an instance of the projectile
		if(projectilePrefab==null)
			return;

		Quaternion qRotation = Quaternion.identity;
		qRotation = trCannon.rotation * Quaternion.Euler(-90,0,0);

		// Create the bullet
		Vector3 v3ShotStartPosition = ((trShotOrigin == null) ? trCannon.position : trShotOrigin.position);
		GameObject goProjectile = Spawner.Spawn(projectilePrefab, v3ShotStartPosition, qRotation) as GameObject;

		if(goProjectile == null) {

			// DEBUG
			Debug.LogError(this.transform + " Couldn't instantiate the projectile " + projectilePrefab);
		}

		// Set the projectile color
		CBaseProjectile scriptProjectile = goProjectile.GetComponent<CBaseProjectile>();
		scriptProjectile.Setup(this.transform, shotColor);
	}

	/// <summary>
	/// Register this cannon with the main script. The main script will put it in a list and give him an unique ID
	/// <summary>
	void RegisterMyselfWithMainScript() {

		mainScript.RegisterCannon(this.transform, this);
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * CANNON CONTROL METHODS
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Puts a player in control of this cannon
	/// </summary>
	/// <param name="trNewPlayer"> The transform of the new player controlling this cannon</param>
	public void PutPlayerInControl(Transform trNewPlayer) {

		if(trPlayerControlling != null) {

			// DEBUG
			Debug.LogError(this.transform + " " 
					+ trNewPlayer + " is trying to control a cannon already controlled by " + trPlayerControlling);

			return;
		}

		trPlayerControlling = trNewPlayer;
		bnIsManned = true;
	}

	/// <summary>
	/// Remove the player in control of this cannon
	/// </summary>
	/// <param name="trCurrentPlayer"> The transform of the player trying to remove the current player </param>
	public void RemovePlayerFromControl(Transform trCurrentPlayer) {

		if(trPlayerControlling != trCurrentPlayer) {

			// DEBUG
			Debug.LogError(this.transform + " " 
					+ trCurrentPlayer + " is trying to remove the player " + trPlayerControlling);

			return;
		}

		bnIsManned = false;
		trPlayerControlling = null;
	}
	/* -----------------------------------------------------------------------------------------------------------
	 * DEBUG STUFF
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// </summary>
	void OnGUI() {

		if(!bnIsManned)
			return;

		// Show the cannon name
		Vector3 v3TextPosition = Camera.main.WorldToScreenPoint(trCannon.position);
		string stName = transform.parent.transform.gameObject.name;
		stName += "\nAim: " + v3AimDirection.ToString();
		stName += "\nAim Angle: " + String.Format("{0:0.00}", fCannonAngle);
		stName += "\ntrCannon: " + String.Format("{0:0.00}", trCannon.position);
		GUI.Label(new Rect(Screen.width - v3TextPosition.x, Screen.height - v3TextPosition.y, 200,150), stName);


		//if(ShooterControl.bnUseMouse) {
		//	// Draws the aim cursor on the screen
		//	GUI.DrawTexture(new Rect(v2AimCursor.x, v2AimCursor.y, fAimCursorSize, fAimCursorSize), 
		//			txAimCursor, ScaleMode.ScaleToFit, true, 0);
		//}
	}

	/// <summary>
	/// </summary>
	void OnDrawGizmos() {

		Gizmos.color = Color.green;
		Gizmos.DrawRay(trCannonBody.position, trCannonBody.up * 3);

	}
}

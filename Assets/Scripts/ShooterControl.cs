using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Main game code
/// </summary>
public class ShooterControl : MonoBehaviour {
	
	/* -----------------------------------------------------------------------------------------------------------
	 * PUBLIC VARIABLES
	 * -----------------------------------------------------------------------------------------------------------
	 */
	public static ShooterControl				Script;							//< shortcut to this script
	public static bool									bnDebugEnabled		= false;
	// XBOX360 Button colors
	public static Color[]								ColorWheel				= new Color[] { Color.green, Color.red, Color.blue, Color.yellow };
	public Transform										trPlayer;
	public Transform										trCamera					= null;
	public static bool									bnUseMouse				= true;
	public static bool									bnUseGamepad			= false;
	// Index of all needed layers
	public static int										enemyLayer				= 9;
	public static int										playerLayer				= 10;
	public static int										buildingLayer			= 11;
	public static int										radarLayer				= 12;
	public static int										GUILayer					= 13;
	// LISTS
	public static List<Transform>				lBuildings				= new List<Transform>();				//< List of all buildings
	public static List<Transform>				lCannons					= new List<Transform>();				//< List of all cannons
	public static List<CCannon>					lCannonsScripts		= new List<CCannon>();					//< List of all cannons scripts //FIXME
	public static List<CBuildingEntity>	lInGameBuildings	= new List<CBuildingEntity>();	//< List of all building in the game
	// PREFABS
	[SerializeField]
	public Transform										cannonPrefab			= null;	//< Prefab to instantiate a new cannon to populate the leve
	public GameObject										enemyPrefab				= null;

	public int													nNumberOfEnemies	= 4;	//< number of enemies in this level
	public int 													nNumberOfCannons	= 6;	//< number of cannons in this level
	public static GamepadDetection 			scriptGamepad			= null;	//< Pointer to the gamepad input script
	public Texture 											txAimCursor;	//< Texture for the icon itself

	Vector3[] v3CameraPositions;

	/* -----------------------------------------------------------------------------------------------------------
	 * PROTECTED VARIABLES
	 * -----------------------------------------------------------------------------------------------------------
	 */
	private int	nCurrentShotColor	= 0;	//< The index to the ColorWheel for the current color. Use with keyboard & mouse

	//Vector2 v2AimStickPosition = Vector2.zero;
	Vector2 v2ScreenCenter;
	LineRenderer lrAimLine;
	//Vector3 v3AimDirection = Vector3.zero;
	// Trigger Radius stuff
	//Vector2 v2AimCursor = Vector2.zero;	//< Position of the cursor around the player
	float fAimCursorRadius = 100.0f;	//< Radius of the cursor around the player
	float fAimCursorSize = 32.0f;	//< Size of the cursor icon

	// Shoot stuff

	float fBombShieldInterval = 0.5f;
	float fBombShieldTimer = 0.0f;

	int nCapturedScreenshots = 0;

	float fCameraRotationDiff = 0.0f;	//< the camera rotation while moving to the current cannon 

	Transform trLevel = null;

	int nPlayerCannonIdx = -1;
	Transform trCameraPivot = null;

	GUIOnGameHUD scriptPopulationBar = null;

	// Population stuff
	float fTotalPopulation = 0.0f; 	//< Sum of the population in all buildings
	float fCurrentPopulation;				//< Current population	
	float fNormalizedPopulation { get{ return fCurrentPopulation/fTotalPopulation; }}

	Camera myCamera;
	
	public class CameraZoomInfo {

		public Vector3 v3Position;
		public float fCameraOrthoSize;
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY'S STUFF HERE
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Will execute when loaded
	/// </summary>
	void Awake() {

		Script = this;
	}

	/// <summary>
	/// Start will execute when an object with it is instantiated
	/// </summary>
	void Start () {
	
		// Object for the gamepad detection script
		scriptGamepad = GameObject.Find("/GameCode").GetComponent<GamepadDetection>();

		if(!scriptGamepad) {

			// DEBUG
			Debug.LogError("Gamepad object controller not found.");
		}

		// Get the screen size
		v2ScreenCenter.x = Screen.width * 0.5f;
		v2ScreenCenter.y = Screen.height * 0.5f;

		// Do we have a gamepad attached?
		if(Input.GetJoystickNames().Length == 0) {

			bnUseMouse = true;
		}
		else {

			bnUseGamepad = true;
		}

		//
		trLevel = GameObject.Find("Level").transform;
		trCameraPivot = GameObject.Find("CameraPivot").transform;

		// Population Bar
		scriptPopulationBar = GetComponent<GUIOnGameHUD>();

		// Populate the level
		PopulateLevelWithEnemies(nNumberOfEnemies);
		PopulateLevelWithCannons(nNumberOfCannons);

		// Starts the DelayedStart coroutine
		StartCoroutine(DelayedStart());

		if(audio) {

			audio.Play();
		}

		myCamera = Camera.main.camera;
	}

	/// <summary>
	/// Delay the execution of this code, like a "delayed start"
	/// </summary>
	private IEnumerator DelayedStart() {

		yield return new WaitForSeconds(0.3F);

		// To be executed after n seconds
		
		// Get the total building health
		float fTotalHealth = 0.0f;
		foreach(CBuildingEntity building in lInGameBuildings) {

			fTotalHealth += building.script.fCurrentHealth;
		}
		// DEBUG
		Debug.Log("Total building health: " + fTotalHealth);

		// Set the total population
		fCurrentPopulation = fTotalPopulation = fTotalHealth;

		scriptPopulationBar.UpdatePopulationBar(fNormalizedPopulation);
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {

		TickTimers();

		CheckDebugKeys();
		ReadInputFromPlayer();
		RotateCamera();
	}

	/// <summary>
	///
	/// </summary>
	void OnGUI() {
		
		// Draws the aim cursor on the screen
	//	GUI.DrawTexture(new Rect(v2AimCursor.x, v2AimCursor.y, fAimCursorSize, fAimCursorSize), 
	//			txAimCursor, ScaleMode.ScaleToFit, true, 0);
	
		if(bnDebugEnabled) {

			// DEBUG
			/*string stText = "Cam:" + String.Format("{0:0.000000}", trCameraPivot.rotation.z) 
				+ "\nCannon: " + String.Format("{0:0.000000}", lCannons[nPlayerCannonIdx].transform.parent.transform.rotation.z) 
				+ "\nDiff: " + String.Format("{0:0.000000}", fCameraRotationDiff)
				+ "\nSelected Cannon: " + nPlayerCannonIdx;
				*/
			string stText = "Total Population: " + fTotalPopulation;

			//Vector3 v3TextPosition = Camera.main.WorldToScreenPoint(trMesh.transform.position);
			GUI.Label(new Rect(5, 5, 300, 200), stText);
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * MY STUFF HERE
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Tick the game timers
	/// </summary>
	void TickTimers() {

		fBombShieldTimer += Time.deltaTime;
	}

	/// <summary>
	/// Deploy a bomb shield. It work as a shield that starts small and grow in size, deflecting all
	/// enemy projectiles with the same color
	/// </summary>
	void DeployBombShield() {

//		if(fBombShieldTimer < fBombShieldInterval || scriptRGBChanger.GetCreatedColor() == Color.black)
//			return;
//
//		fBombShieldTimer = 0.0f;
//
//		GameObject goBombShield = Instantiate(Resources.Load("BombShield")) as GameObject;
//		BombShield scriptBombShield = goBombShield.GetComponent<BombShield>();
//		scriptBombShield.Setup(scriptRGBChanger.GetCreatedColor());
	}

	/// <summary>
	/// Check if we pressed some special keyboard key
	/// </summary>
	void CheckDebugKeys() {

		/// 'd' enables/disables debug messages
		if(Input.GetKeyDown(KeyCode.Tab)) {

			bnDebugEnabled = !bnDebugEnabled;
		}

		/// Get a screenshot from the game
		if(Input.GetKeyDown(KeyCode.F12)) {

			HelperCaptureScreenshot();
		}
	}

	/// <summary>
	/// Capture a screenshot from the game
	/// </summary>
	void HelperCaptureScreenshot() {

			string stFilename = "PlanetaryDefense_" + nCapturedScreenshots + ".png";
			Application.CaptureScreenshot(stFilename);
			nCapturedScreenshots++;

			// DEBUG
			Debug.Log("Captured screenshot: "  + stFilename);
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * REGISTER STUFF	
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Add a building to the building list
	/// </summary>
	/// <param name="trBuilding"> The transform for the new building </param>
	public void RegisterBuilding(Transform trBuilding) {

		lBuildings.Add(trBuilding);
	}

	/// <summary>
	/// Add a building to the building list
	/// </summary>
	/// <param name="trBuilding"> The transform for the new building </param>
	/// <param name="buildingScript"> The script for this new building </param>
	public void RegisterBuildingEntity(Transform trBuilding, CBuilding buildingScript) {
		int nCount = lInGameBuildings.Count;

		CBuildingEntity newBuilding = new CBuildingEntity();
		newBuilding.tr = trBuilding;
		newBuilding.script = buildingScript;
		lInGameBuildings.Add(newBuilding);

		// Give the building a new ID
		buildingScript.nID = nCount;
	}

	/// <summary>
	/// Remove a building from the building list
	/// </summary>
	public void DeregisterBuilding(Transform trBuilding) {

		lBuildings.Remove(trBuilding);
	}

	/// <summary>
	/// Add a cannon to the cannon list
	/// </summary>
	/// <param name="tr"> The transform for the new cannon </param>
	public void RegisterCannon(Transform tr, CCannon script) {

		int nCount = lCannons.Count;
		
		lCannons.Add(tr);
		lCannonsScripts.Add(script);

		script.nIdx = nCount;

		// FIXME: hack to make the player control a cannon
		if(nPlayerCannonIdx < 0) {

			PlayerControlCannon(trPlayer, nCount);
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * POPULATE
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Add the first enemies in the game, for testing purposes
	/// </summary>
	void PopulateLevelWithEnemies(int nNumberOfEnemies) {

		float fMinAltitude = 35.0f;
		float fMaxAltitude = 42.0f;

		for(int n=0; n<nNumberOfEnemies; n++) {
			
			// Instantiate a new enemy
			GameObject goEnemy = Spawner.Spawn(enemyPrefab, Vector3.zero, Quaternion.identity) as GameObject;
			CEnemy scriptEnemy = goEnemy.GetComponent<CEnemy>();

			int nRandomColor = UnityEngine.Random.Range(0, ColorWheel.Length);
			scriptEnemy.Setup(ColorWheel[nRandomColor],UnityEngine.Random.Range(fMinAltitude, fMaxAltitude), UnityEngine.Random.Range(0.0f, 60.0f));
		}
	}

	/// <summary>
	/// Add the cannons in the game, for testing purposes
	/// </summary>
	void PopulateLevelWithCannons(int nNumberOfCannons) {

		float fAngleOffset = 0;

		if(nNumberOfCannons <= 0)
			return;

		fAngleOffset = 360/nNumberOfCannons;

		for(int n=0; n<nNumberOfCannons; n++) {
			
			Quaternion qRotation = Quaternion.Euler(0,0, fAngleOffset * n);
			Transform trCannon = Instantiate(cannonPrefab, Vector3.zero, qRotation) as Transform;
			trCannon.name = "Cannon_" + n;
		}
	}

	/// <summary>
	/// A player wants to control a cannon
	/// </summary>
	void PlayerControlCannon(Transform trCurrentPlayer, int nCannonIdx ) {

		// 1 - Check if the desired cannon is free (not controlled by anyone)
		if(lCannonsScripts[nCannonIdx].trPlayerControlling != null) {

			// DEBUG
			Debug.LogWarning("Cannon " + nCannonIdx + " already controlled.");
			return;
		}

		// 2 - Remove the player from the current cannon, if any
		if(nPlayerCannonIdx >= 0) {

			lCannonsScripts[nPlayerCannonIdx].RemovePlayerFromControl(trPlayer);
		}

		// 3 - Add the player to the desired cannon
		lCannonsScripts[nCannonIdx].PutPlayerInControl(trPlayer);

		// 4 - Update the internal index
		nPlayerCannonIdx = nCannonIdx;
	}

	/// <summary>
	///
	/// </summary>
	void ChangeToCannonLeft() {

		int nIdx = nPlayerCannonIdx;

		nIdx--;

		if(nIdx < 0) {

			// Roll to the last cannon in the list
			nIdx = lCannons.Count-1;
		}

		if(nIdx != nPlayerCannonIdx) {
		
			PlayerControlCannon(trPlayer, nIdx);
		}
	}

	/// <summary>
	///
	/// </summary>
	void ChangeToCannonRight() {

		int nIdx = nPlayerCannonIdx;

		nIdx++;

		// Roll to the first cannon in the list
		if(nIdx > lCannons.Count-1) {

			nIdx = 0;
		}

		if(nIdx != nPlayerCannonIdx) {

			PlayerControlCannon(trPlayer, nIdx);
		}
	}

	/// <summary>
	/// Rotate the camera towards the angle of the current cannon
	/// </summary>
	void RotateCamera() {

		float fRotationThreshold = 0.00001f;
		float fRotationSpeed = 2.0f;

		if(nPlayerCannonIdx < 0)
			return;

		fCameraRotationDiff = 
			Mathf.Abs(trCameraPivot.rotation.z) - Mathf.Abs(lCannons[nPlayerCannonIdx].transform.parent.transform.rotation.z);
		// Trying to stabilize
		if( Mathf.Abs( fCameraRotationDiff) >	fRotationThreshold) {

			// Rotates the camera
			trCameraPivot.rotation = Quaternion.Slerp(trCameraPivot.rotation,
				lCannons[nPlayerCannonIdx].transform.parent.transform.rotation, Time.deltaTime * fRotationSpeed);
		}
		else {

			//trCameraPivot.rotation = lCannons[nPlayerCannonIdx].transform.parent.transform.rotation;
		}
	}

	/// <summary>
	/// Reads the inputs from the player
	/// </summary>
	void ReadInputFromPlayer() {


		if(Input.GetKey(KeyCode.Escape)) {

			// FIXME
			Application.Quit();
		}

		// ALEX HACK: The cannon can move in the planet' surface
		{

			float fCannonMoveSpeed = 30.0f;

			if( Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Joystick1Button4) ) {
			
				lCannons[nPlayerCannonIdx].transform.parent.transform.Rotate(
						new Vector3(0,0,-1) * fCannonMoveSpeed * Time.deltaTime, Space.Self);
			}
			if( Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.Joystick1Button5)) {
			
				lCannons[nPlayerCannonIdx].transform.parent.transform.Rotate(
						new Vector3(0,0, 1) * fCannonMoveSpeed * Time.deltaTime, Space.Self);
			}
		}

		// try to change the control between the cannons
		//if( Input.GetKeyUp(KeyCode.Z) || Input.GetAxis("Mouse ScrollWheel") < 0) {

		//	ChangeToCannonLeft();
		//}
		//if( Input.GetKeyUp(KeyCode.X) || Input.GetAxis("Mouse ScrollWheel") > 0) {

		//	ChangeToCannonRight();
		//}
		if(Input.GetAxis("Mouse ScrollWheel") < 0) {

		}
		if(Input.GetAxis("Mouse ScrollWheel") > 0) {

		}


		if(bnUseMouse) {

			// Read input from mouse & keyboard
			if(Input.GetKey(KeyCode.S))
					nCurrentShotColor = 0;	// Green
			if(Input.GetKey(KeyCode.D))
					nCurrentShotColor = 1;	// Red
			if(Input.GetKey(KeyCode.A))
					nCurrentShotColor = 2;	// Blue
			if(Input.GetKey(KeyCode.W))
					nCurrentShotColor = 3;	// Yellow

			if(Input.GetMouseButton(0)) {

				lCannonsScripts[nPlayerCannonIdx].Shoot(ColorWheel[nCurrentShotColor]);
			}
		}

		if(bnUseGamepad) {

			// Read input from the XBOX 360 gamepad as joystick 1
			// Uses the XBox gamepad to define the shot color
			if(Input.GetKey(KeyCode.Joystick1Button0)) {

				lCannonsScripts[nPlayerCannonIdx].Shoot(ColorWheel[0]);
			}
			if(Input.GetKey(KeyCode.Joystick1Button1) || Input.GetKey(KeyCode.S)) {

				lCannonsScripts[nPlayerCannonIdx].Shoot(ColorWheel[1]);
			}
			if(Input.GetKey(KeyCode.Joystick1Button2) || Input.GetKey(KeyCode.Q)) {

				lCannonsScripts[nPlayerCannonIdx].Shoot(ColorWheel[2]);
			}
			if(Input.GetKey(KeyCode.Joystick1Button3) || Input.GetKey(KeyCode.W)) {

				lCannonsScripts[nPlayerCannonIdx].Shoot(ColorWheel[3]);
			}
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * EVENTS
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Object enabled
	/// </summary>
	void OnEnable() {

		CBuilding.OnBuildingAttacked += OnBuildingAttacked;
	}
	
	/// <summary>
	/// Object disabled
	/// </summary>
	void OnDisable() {

		CBuilding.OnBuildingAttacked -= OnBuildingAttacked;
	}

	/// <summary>
	/// From the CBuilding class: what to do when a building is hit by an enemy. For now, we decrease the
	/// living population on the planet
	/// </summary>
	void OnBuildingAttacked(float fDamageTaken) {

		fCurrentPopulation -= fDamageTaken;

		// Update the population bar
		scriptPopulationBar.UpdatePopulationBar(fNormalizedPopulation);
	}
}

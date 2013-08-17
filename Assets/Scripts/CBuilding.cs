using UnityEngine;
using System.Collections;


/// <summary>
/// Defines the behaviour of a building in the game
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class CBuilding : CBaseEntity {

	/* -----------------------------------------------------------------------------------------------------------
	 * CLASS VARIABLES
	 * -----------------------------------------------------------------------------------------------------------
	 */
	// PUBLIC
	public AudioClip 	sfxHit; 										//< Played when hit by an enemy
	[SerializeField]
	public GameObject	peoplePrefab 				= null;	//< Prefab to the dying people off the building
	[SerializeField]
	public GameObject	buildingHitPrefab 	= null;	//< Prefab to the dying people off the building
	// PRIVATE
	private int				nNumOfPeopleRunning	= 0;	//< Number of people that runned off the building
	GameObject				goBuildingHitEffect;
	
	/* -----------------------------------------------------------------------------------------------------------
	 * EVENTS
	 * -----------------------------------------------------------------------------------------------------------
	 */
	public delegate void BuildingAttackedHandler(float fDamageAmount);
	public static event BuildingAttackedHandler OnBuildingAttacked;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY'S STUFF
	 * -----------------------------------------------------------------------------------------------------------
	 */

	//
	void OnEnable() {

		goBuildingHitEffect = null;
	}

	void OnDisable() {

		goBuildingHitEffect = null;

	}

	void Awake() {

		// Set up the rigidbody
		rigidbody.isKinematic = true;
		rigidbody.useGravity = false;
	}

	// Use this for initialization
	void Start () {
	
		// Register itself with the game
		ShooterControl.Script.RegisterBuilding(this.transform);
		
		// New version
		RegisterMyselfWithMainScript();
	}
	
	// Update is called once per frame
	void Update () {
	
		if(nNumOfPeopleRunning <= 0 && audio && audio.isPlaying) {

			// Ok, we have no more people running,  the screams go on
			audio.Stop();
		}
	}

	/// <summary>
	/// Takes damage
	/// </summary>
	/// <param name="fAmount"> A float with the amount of damage taken </param>
	public override void TakeDamage(float fAmount, Vector3 v3HitPosition) {

		if(sfxHit) {

			AudioSource.PlayClipAtPoint(sfxHit, transform.position);
		}

		if(audio && !audio.isPlaying) {

			// Randomize the starting point
			audio.Play();
			audio.time = UnityEngine.Random.Range(0, audio.clip.length);
		}

		// TODO: put some smoke and debris where the building is hit
		if(buildingHitPrefab) {

			goBuildingHitEffect = Spawner.Spawn(buildingHitPrefab, v3HitPosition, this.transform.rotation);
		}


		// Make some people come off the building
		SproutBurningPeople();

		// Call an event
		OnBuildingAttacked( Mathf.Min(_fHealth, fAmount));	//< if the health is less than the amount of damage, zeroes it

		// Execute the base method
		base.TakeDamage(fAmount, v3HitPosition);
	}

	/// <summary>
	/// </summary>
	protected override void Die() {

		// Remove-me from the buildings list
		ShooterControl.Script.DeregisterBuilding(this.transform);

		if(audio && audio.isPlaying) {

			audio.Stop();
		}

		// Remove the burning effect
		if(goBuildingHitEffect)
			Spawner.Destroy(goBuildingHitEffect);

		base.Die();
	}

	/// <summary>
	/// Register this entity with the main game script
	/// </summary>
	void RegisterMyselfWithMainScript() {

		ShooterControl.Script.RegisterBuildingEntity(this.transform, this);
	}
	
	/// <summary>
	///	Create a bunch of the people running away from the building, catching fire and screaming
	/// </summary>
	void SproutBurningPeople() {

		int nNumberOfPeople = UnityEngine.Random.Range(1,5);

		// update the people counter
		nNumOfPeopleRunning += nNumberOfPeople;

		for(int n = 0; n < nNumberOfPeople; n++) {

			if(peoplePrefab) {
				
				GameObject go = Spawner.Spawn(peoplePrefab, Vector3.zero, this.transform.rotation) as GameObject;
				DyingPeople peopleScript = go.transform.Find("Person").gameObject.GetComponent<DyingPeople>();
				DyingPeople.buildingScript = this;

				if(peopleScript == null) {

					// DEBUG
					Debug.LogError(this.transform + " Failed to get DyingPeople component or find the Person object");
				}
			}
		}
	}

	/// <summary>
	/// One people died, so we have to update the people counter
	/// </summary>
	public void OneManDown() {

		nNumOfPeopleRunning--;
	}
}

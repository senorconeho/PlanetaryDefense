using UnityEngine;
using System.Collections;

/// <summary>
/// Behaviour of a basic projectile
/// </summary>
public class CBaseProjectile : MonoBehaviour {

	// PUBLIC
	public float 			fSpeed			= 4.2f;			//< Travel speed
	public float 			fLifeTime		= 3.0f;			//< How long it lasts
	public float 			fDamageDone	= 0.25f;		//< Damage done when hit something
	public AudioClip	sfxShoot 		= null;			//< sound effect to be played when shot
	public GameObject	hitPrefab		=	null;			//< Effect to be created when the shot hit something
	
	// PRIVATE
	private float 		fSpawnTime 	= 0.0f;			//< internal TTL timer
	private Transform	tr;											//< pointer to this object transform
	private bool 			bnSetupReady	= false;	//< are we ready yet?
	private Color			myColor = Color.black;	//< projectile color
	private Transform trMesh = null;					//< pointer to the actual model in this object's hierarchy
	Transform					trShooter = null; 			//< Who shot this?

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY'S STUFF HERE
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// </summary>
	void Awake() {

		trMesh = GetMeshObject();
	}

	/// <summary>
	/// When this object is enabled
	/// </summary>
	void OnEnable() {

		tr = this.transform;
		fSpawnTime = Time.time;

		if(sfxShoot) {

			AudioSource.PlayClipAtPoint(sfxShoot, transform.position);
		}
	}

	/// <summary>
	/// </summary>
	void OnDisable() {

		bnSetupReady = false;
	}
	
	/// <summary>
	/// Main cycle - updated every frame
	/// </summary>
	void Update () {

		if(!bnSetupReady)
			return;

		DoMovement();

		if(Time.time > fSpawnTime + fLifeTime) {

			Die();
		}
	}

	/* -----------------------------------------------------------------------------------------------------------
	 *                   
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Setup this object
	/// </summary>
	/// <param name="color"> The color for this object </param>
	public virtual void Setup(Transform trOwner, Color color) {

		myColor = color;

		if(trMesh)
			trMesh.gameObject.renderer.material.color = myColor;
		
		this.trShooter = trOwner;
		bnSetupReady = true;
	}

	/// <summary>
	/// Do the movement
	/// </summary>
	public virtual void DoMovement() {

		// Basic movement
		transform.Translate(Vector3.forward * fSpeed * Time.deltaTime, Space.Self);
	}

	/// <summary>
	/// What happens when we hit something
	/// </summary>
	public virtual void Hit(Transform entityHit, Vector3 v3Position) {

		if(!trShooter) 
			return;

		// Ignore entities shooting themselves
		if(trShooter.gameObject.layer == entityHit.gameObject.layer)
			return;

		// Enemy hitting one of our buildings
		if(entityHit.gameObject.layer == ShooterControl.buildingLayer 
				&& trShooter.gameObject.layer == ShooterControl.enemyLayer) {

			// Get the building basic component
			CBaseEntity entityScript = entityHit.GetComponent<CBaseEntity>();
			
			if(entityScript) {

				entityScript.TakeDamage(fDamageDone, v3Position);
			}
			else {

				// DEBUG
				Debug.LogError(this.transform + " Missing CBaseEntity in " + entityHit);
			}
		}
		else if(entityHit.gameObject.layer == ShooterControl.enemyLayer 
				&& trShooter.gameObject.layer == ShooterControl.playerLayer) {

			CEnemy enemyScript = null;
			// FIXME
			// The enemy shield is filtered by the game object tag
			if(entityHit.gameObject.tag == "Shield") {

				// Our projectile hit the enemy shield, so we get the script from the "grandparent" :P
				enemyScript = entityHit.transform.parent.transform.parent.gameObject.GetComponent<CEnemy>();
				
				if(enemyScript.myColor == myColor) {

					// Same color? The shot pass through the shield
					return;
				}
			}
			else {

				// Our projectile hit an enemy
				enemyScript = entityHit.transform.parent.gameObject.GetComponent<CEnemy>();
				enemyScript.HitBy(myColor, fDamageDone);
			}

		}
		else return;

		// Destroy itself
		Die();
	}

	/// <summary>
	/// What to do when this object "dies"
	/// </summary>
	protected virtual void Die() {

		// Create a feedback
		//if(hitPrefab) {

		//	// Instantiate a particle system
		//	GameObject go = Spawner.Spawn(hitPrefab, transform.position, transform.rotation) as GameObject;

		//	// Change the color of the particles to the shot color
		//	go.GetComponent<ParticleSystem>().startColor = myColor;
		//}

		Spawner.Destroy(gameObject);
	}

	/// <summary>
	/// Find the mesh object in this object's hierarchy. For now, we're using the following structure:
	/// * Object name										-> root empty game object
	/// |-*	Mesh 												-> empty game object, act as a folder
	///   |-* Model name								-> the actual object model
	///     |-* Material								-> some material we could change the color
	/// </summary>
	protected Transform GetMeshObject() {

		// First, find the "Mesh" in the hierarchy
		Transform meshHierarchy = transform.Find("Mesh");

		// Now, get the children of the Mesh, This will be the real model
		if(meshHierarchy != null) {

			foreach(Transform child in meshHierarchy) {
				
				return child;
			}
		}

		return null;
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * COLLISION STUFF
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Detect collision with other objects.
	/// For now, we are only interested in collisions with the player object
	/// </summary>
	void OnTriggerEnter(Collider col) {

		Hit(col.gameObject.transform, this.transform.position);
	}
}

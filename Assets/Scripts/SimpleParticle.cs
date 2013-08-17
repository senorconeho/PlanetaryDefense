using UnityEngine;
using System.Collections;

/// <summary>
/// Class name and description
/// </summary>
public class SimpleParticle : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC

	// PRIVATE

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

		ParticleSystem ps = gameObject.GetComponent<ParticleSystem>();

		if(!ps) {

			// DEBUG
			Debug.LogError(this.transform + " could not get the ParticleSystem component");
		}

		float fDuration = ps.duration;

		StartCoroutine(DieAfterDuration(fDuration));
	}
	
	/* ==========================================================================================================
	 * CLASS METHODS
	 * ==========================================================================================================
	 */

	/// <summary>
	/// Wait for the duration of the particle system and then disable it, making it free for the Spawner
	/// </summary>
	IEnumerator DieAfterDuration(float fDuration) {

		yield return new WaitForSeconds(fDuration);

		Die();
	}

	/// <summary>
	/// What to do when this object "dies"
	/// </summary>
	void Die() {

		//	Using the spawner
		Spawner.Destroy(gameObject);
	}

}

using UnityEngine;
using System.Collections;

/// <summary>
/// Class to do a laser beam
/// </summary>
public class CLaserBeam : MonoBehaviour {
	
	private Vector3 v3Direction;
	private float fLifeSpan = 3.0f;
	private Color myColor = Color.black;
	public Transform trMesh;
	bool bnSetupReady = false;
	float fFadeInSpeed = 1.8f; //< Speed of the fade in
	float fFadeOutSpeed = -1.0f; //< Speed of the fade out. Must be negative so the alpha value is decreased
	float fFadeSpeed;

	public AudioClip sfxShoot; //< Played when this laser is shoot
	float fDamage = 0.5f;

	/* -----------------------------------------------------------------------------------------------------------
	 * UNITY'S STUFF HERE
	 * -----------------------------------------------------------------------------------------------------------
	 */
	/// <summary>
	/// Will execute when loaded
	/// </summary>
	void Awake() {

	}

	/// <summary>
	/// Start will execute when an object with it is instantiated
	/// </summary>
	void Start () {

		CheckHit();
		StartCoroutine(kill());
		
		if(sfxShoot) {

			AudioSource.PlayClipAtPoint(sfxShoot, transform.position);
		}

		// Set the fade speed
		fFadeSpeed = fFadeInSpeed;
	}
	
	/// <summary>
	/// Update is called once per frame
	/// </summary>
	void Update () {
	
		if(!bnSetupReady)
			return;

		if(myColor.a >= 1.0f) {
			
			fFadeSpeed = fFadeOutSpeed;
		}
		myColor.a += Time.deltaTime * fFadeSpeed;
		myColor.a = Mathf.Clamp01(myColor.a);
		//myColor.a = Mathf.PingPong(myColor.a - Time.deltaTime, 1.0f);

		trMesh.gameObject.renderer.material.color = myColor;
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * MY STUFF HERE
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	/// Kills the object after his life span, freeing up some memory
	/// </summary>
	/// <returns> A <see cref="IEnumerator"/></returns>
	protected IEnumerator kill() {

		yield return new WaitForSeconds(fLifeSpan);

		Destroy(gameObject);
	}

	/// <summary>
	/// Setup the Laser Beam with a color and a direction
	/// </summary>
	public void SetupLaserBeam(Color colShotColor, Vector3 v3ShotDirection) {

		myColor = colShotColor;
		v3Direction = v3ShotDirection;

		// Rotate the laser beam to the direction we're aiming
		transform.rotation = Quaternion.LookRotation(v3Direction);
		// Change the color of the beam to the color selected when we shooted
		myColor.a = 0.0f;
		trMesh.gameObject.renderer.material.color = myColor;

		bnSetupReady = true;
	}

	/// <summary>
	/// Check if this beam hit something
	/// </summary>
	void CheckHit() {

		// Throws a raycast
		RaycastHit hit;
		if(Physics.Raycast(this.transform.position, v3Direction, out hit, Mathf.Infinity, 1<<9)) {

			// Ok, we have hit an enemy. So, what was his color? 
			CEnemy enemyScript = hit.transform.parent.gameObject.GetComponent<CEnemy>();
			enemyScript.HitBy(myColor, fDamage);

			// DEBUG
			Debug.Log(this.transform + " Hit in " + hit.transform);
		}
	}
}

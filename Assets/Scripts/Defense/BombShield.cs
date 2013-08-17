using UnityEngine;
using System.Collections;

/// <summary>
/// Throws a shield that works like a bomb's blast: start at the center and grow until it completely fades
/// It has a color too, so can deflect from shots from the same color
/// </summary>
public class BombShield : MonoBehaviour {

	private Color myColor = Color.black;
	private float fMaxRadius = 4.0f;
	bool bnSetupReady = false;
	float fExpandSpeed = 5.0f;
	public Transform trMesh = null;

	public AudioClip sfxShoot;	//< Audio to be player when this is shot

	// Use this for initialization
	void Start () {
	
		// Play the sound effect
		if(sfxShoot) {

			AudioSource.PlayClipAtPoint(sfxShoot, transform.position);
		}
	}
	
	// Update is called once per frame
	void Update () {

		if(!bnSetupReady)
			return;

		// Sets the color of the object
		trMesh.gameObject.renderer.material.color = myColor;

		float fNewSize = trMesh.transform.localScale.x + Time.deltaTime * fExpandSpeed;

		if(fNewSize >= fMaxRadius) {

			Destroy(gameObject);
		}
		else {

			Vector3 v3NewScale = new Vector3(fNewSize, 1.0f, fNewSize);
			trMesh.transform.localScale = v3NewScale;
		}
	}

	/// <summary>
	/// Setup the shield with color and maximum radius
	/// </summary>
	/// <param name="colorShieldColor"> The color of this shield. Deflects all bullets with this color </summary>
	public void Setup(Color colorShieldColor) {

		myColor = colorShieldColor;
		myColor.a = 0.5f;

		bnSetupReady = true;
	}

	/// <summary>
	/// </summary>
	public Color GetColor() {

		return myColor;
	}
}

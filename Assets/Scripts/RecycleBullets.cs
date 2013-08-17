using UnityEngine;
using System.Collections;

/// <summary>
/// Recycle all bullets that touches this collider
/// </summary>
public class RecycleBullets : MonoBehaviour {

	/// <summary>
	///
	/// </summary>
	void OnTriggerEnter(Collider col) {

		if(col.transform.tag == "Projectile") {

			Spawner.Destroy(col.transform.gameObject);
		}
	}
}

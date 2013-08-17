using UnityEngine;
using System.Collections;

public class TimedDestroyObject : MonoBehaviour {

	public float fTimeToLive = 1.0f;


	// Use this for initialization
	void Start () {

		StartCoroutine(KillMyself());	
	}
	
	/// <summary>
	/// Kill this object
	/// </summary>
	private IEnumerator KillMyself() {

		yield return new WaitForSeconds(fTimeToLive);

		if(gameObject != null) {
			
			Destroy(gameObject);
		}
	}
}

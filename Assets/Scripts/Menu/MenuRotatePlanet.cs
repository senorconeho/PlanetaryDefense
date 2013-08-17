using UnityEngine;
using System.Collections;

public class MenuRotatePlanet : MonoBehaviour {

	public Transform	trPlanet				=	null;
	public float			fRotationSpeed	=	5.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		if(trPlanet != null) {

			trPlanet.Rotate(new Vector3(0,0,1) * Time.deltaTime * fRotationSpeed);
		}
	
	}
}

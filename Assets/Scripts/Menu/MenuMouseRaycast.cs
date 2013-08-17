using UnityEngine;
using System.Collections;

public class MenuMouseRaycast : MonoBehaviour {

	public Camera GUICamera = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
		// Check the "mouse over"
		Transform tr = ThrowRaycastAgainstMenu();
		CMenuItem menuScript = null;

		if(tr != null) {

			menuScript = tr.gameObject.GetComponent<CMenuItem>();
			menuScript.OnMouseOverItem();
		}

		// Check mouse click
		if( (Input.GetMouseButton(0)) && (tr != null) && (menuScript != null) ) {

			menuScript.OnMouseClickItem();
		}
	}

	/// <summary>
	/// Throws a raycast against the mouse position. Evaluates only menu itens
	/// </summary>
	/// <returns> The transform of the menu item hit by the raycast</returns>
	Transform ThrowRaycastAgainstMenu() {

		Transform tr = null;

		Ray ray = GUICamera.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		
		if(Physics.Raycast(ray, out hit, Mathf.Infinity, 1<<ShooterControl.GUILayer )) {

			// Filter only menu itens
			if(hit.transform.gameObject.tag == "MenuItem") {
			
				tr = hit.transform;
			}
		}

		return tr;
	}
}

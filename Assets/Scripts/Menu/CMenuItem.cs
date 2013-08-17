using UnityEngine;
using System.Collections;

/// <summary>
/// Class to perform actions with the menu item
/// </summary>
public class CMenuItem : MonoBehaviour {

	/* ==========================================================================================================
	 * CLASS VARIABLES
	 * ==========================================================================================================
	 */
	// PUBLIC
	public AudioClip	sfxMouseOverItem	= null;	//< Effect to play when the mouse is over the menu item
	public AudioClip	sfxMouseClickItem	= null;	//< Effect to play when the mouse is clicked over the menu item
	public enum eMenuTypes { MAIN_START, MAIN_OPTION, MAIN_QUIT, MAIN_NULL };
	public eMenuTypes	eMenuItemType			= eMenuTypes.MAIN_NULL;

	/* ==========================================================================================================
	 * UNITY METHODS
	 * ==========================================================================================================
	 */
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	/// <summary>
	/// What to do when the mouse is over this item
	/// </summary>
	public void OnMouseOverItem() {

		if( !animation.IsPlaying("MenuScale") ) {
			
			animation.Play("MenuScale");
		}
	}

	/// <summary>
	/// What to do when we click this item
	/// </summary>
	public void OnMouseClickItem() {

		if( sfxMouseClickItem != null && !audio.isPlaying) {

			AudioSource.PlayClipAtPoint(sfxMouseClickItem, transform.position);
		}

		DoButtonAction();
	}

	/// <summary>
	/// Do what this button is supposed to do when clicked
	/// <summary>
	void DoButtonAction() {

		switch (eMenuItemType) {

			case eMenuTypes.MAIN_START:
				// Load the game
				Application.LoadLevel(Application.loadedLevel+1);
				break;

			case eMenuTypes.MAIN_OPTION:
				break;

			case eMenuTypes.MAIN_QUIT:
				// Quits the game
				Application.Quit();
				break;
		}
	}
}

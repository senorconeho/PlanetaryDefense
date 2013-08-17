using UnityEngine;
using System;
using System.Collections;

public class CBaseEntity : MonoBehaviour {
	
	protected float _fHealth = 1.0f;
	public float fCurrentHealth{get{return _fHealth;}}
 
	protected float _fMaxHealth = 1.0f;
	public float fMaxHealth{get{return _fMaxHealth;}}
	
	public float _fMoveSpeed = 10.0f;
  public float fMoveSpeed{get{return _fMoveSpeed;}}
 
	public Transform trMesh;

	protected int m_nID = -1;	//< this entity id number
	public int nID {
		get { return m_nID; }
		set { m_nID = value; }
	}


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public virtual void TakeDamage(float amount, Vector3 v3Position) {
		_fHealth -= amount;
  
		if(_fHealth <= 0) {
			Die();
		}
	}	
	
	public virtual void HealDamage(float amount) {
		
		_fHealth = Mathf.Min(_fHealth + amount, _fMaxHealth);
	}
	
	protected virtual void Die() {
		
		// DEBUG
		Debug.Log("CBaseEntity: calling Die() for " + this.transform);

		//Spawner.Destroy(gameObject);
	}

	/* -----------------------------------------------------------------------------------------------------------
	 * DEBUG
	 * -----------------------------------------------------------------------------------------------------------
	 */

	/// <summary>
	///
	/// </summary>
	void OnGUI() {

		if(ShooterControl.bnDebugEnabled) {
			
			// DEBUG
			string stText = "H: " + String.Format("{0:0.0}", _fHealth);

			Vector3 v3TextPosition = Camera.main.WorldToScreenPoint(this.transform.position);
			GUI.Label(new Rect(v3TextPosition.x, Screen.height - v3TextPosition.y, 100, 100), stText);
		}
	}
	
}

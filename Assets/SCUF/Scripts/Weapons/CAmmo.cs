using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Class to hold 
/// An "ammo" is any kind of ammo that goes in a weapon. It has :
/// - Quantity: all the ammo available for this kind of projectile. Like in Doom, many weapons shares the
/// same ammo type
/// - a projectile (the model representing the bullet), with movement behaviour, etc.
/// - 
/// </summary>
[Serializable]
public class CAmmo {

	// Total ammo quantity
	public int nQuantity;
	// Projectile prefab
	public Transform prefab;
}

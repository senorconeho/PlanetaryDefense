using UnityEngine;
using System;

[Serializable]
/// <summary>
/// Basic representation of a building in the game. Have it's transform and a pointer to it' script
/// </summary>
public class CBuildingEntity {

	public Transform tr;
	public CBuilding script;
}

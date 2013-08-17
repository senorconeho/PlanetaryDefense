using UnityEngine;

/// <summary>
/// Class that extends the MonoBehaviour class, making our life easier
/// </summary>
internal static class ExtensionMethods {

	/// <summary>
	/// Set only the x value o a Vector3 
	/// </summary>
	/// <param name="tr"> Transform of the object </param>
	/// <param name="x"> New x value for the position </param>
	public static void SetX(this Transform tr, float x) {

		tr.position = new Vector3(x, tr.position.y, tr.position.z);
	}

	/// <summary>
	/// Set only the z value o a Vector3 
	/// </summary>
	/// <param name="tr"> Transform of the object </param>
	/// <param name="z"> New z value for the position </param>
	public static void SetZ(this Transform tr, float z) {

		tr.position = new Vector3(tr.position.x, tr.position.y, z);
	}
}

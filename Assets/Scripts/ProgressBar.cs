using UnityEngine;
using System.Collections;

/// <summary>
/// This class creates a progress bar that fills (left to right) from time to time.
/// </summary>
public class ProgressBar : MonoBehaviour {

	protected float m_fTotalTime = 1.0f;
	public float fTotalTime { get { return m_fTotalTime; } set{ m_fTotalTime = value; } }

	//< Sound to play when the bar completes
	public AudioClip sfxBarComplete;

	/// <summary>
	/// Updates the bar, filling it (left to right)
	/// </summary>
	/// <param name="fCurrentTime"> Current timer (or % done until now) </param>
	/// <param name="fTargetTime"> Time needed to fill the bar (100% completion) </param>
	public void IncreaseBar(float fCurrentTime, float fTargetTime) {
		
		renderer.material.SetFloat("_Cutoff", 1-Mathf.InverseLerp(0, fTargetTime, fCurrentTime));

		// Bar completed?
		if(fCurrentTime >= fTargetTime) {

			// FIXME: the bar will soon be destroyed. Should we use PlayOneShot() instead?
			if(sfxBarComplete)
				AudioSource.PlayClipAtPoint(sfxBarComplete, transform.position);
		}
	}
	/// <summary>
	/// Updates the bar, filling it (left to right)
	/// </summary>
	/// <param name="fCurrentTime"> Current timer (or % done until now) </param>
	public void IncreaseBar(float fCurrentTime) {
		
		renderer.material.SetFloat("_Cutoff", 1-Mathf.InverseLerp(0, fTotalTime, fCurrentTime));
	}

	/// <summary>
	/// Updates the bar, decreasing the fill bar (right to left)
	/// </summary>
	/// <param name="fCurrentTime"> Current timer (or % done until now) </param>
	/// <param name="fTargetTime"> Time needed to fill the bar (100% completion) </param>
	public void DecreaseBar(float fCurrentTime, float fTargetTime) {
		
		renderer.material.SetFloat("_Cutoff", Mathf.InverseLerp(0, fTargetTime, fCurrentTime));

		// Bar completed?
		if(fCurrentTime >= fTargetTime) {

			// FIXME: the bar will soon be destroyed. Should we use PlayOneShot() instead?
			if(sfxBarComplete)
				AudioSource.PlayClipAtPoint(sfxBarComplete, transform.position);
		}
	}

	/// <summary>
	/// Updates the bar, decreasing the fill bar (right to left)
	/// </summary>
	/// <param name="fCurrentTime"> Current timer (or % done until now) </param>
	public void DecreaseBar(float fCurrentTime) {
		
		renderer.material.SetFloat("_Cutoff", 1-Mathf.InverseLerp(0, fTotalTime, fCurrentTime));
	}
}

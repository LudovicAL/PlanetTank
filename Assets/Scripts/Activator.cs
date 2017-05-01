using UnityEngine;

//This scripts makes sure the GameManager has been initialized before activating every scripts/components on the current GameObject.
public class Activator : MonoBehaviour {

	private bool activated;

	// Use this for initialization
	void Start () {
		activated = false;
		Activate ();
	}

	/// <summary>
	/// Checks if the GameManager has been initialized already and, if so, activates every components on the current gameobject.
	/// </summary>
	public void Activate() {
		if (!activated) {
			GameObject scriptsBucket = GameObject.Find ("ScriptsBucket");
			if (scriptsBucket == null) {
				return;
			}
			MonoBehaviour[] scripts = this.gameObject.GetComponents<MonoBehaviour>();
			foreach (MonoBehaviour script in scripts) {
				script.enabled = true;
			}
			activated = true;
		}
	}
}
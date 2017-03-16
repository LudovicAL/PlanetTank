using UnityEngine;

public class Cleaner : MonoBehaviour {

	[Tooltip("Duration in seconds after which the GameObject should be destroyed.")] public float cleanAfterDuration;

	void Update () {
		cleanAfterDuration -= Time.fixedDeltaTime;
		if (cleanAfterDuration <= 0.0f)
			GameObject.DestroyObject(gameObject);
	}
}

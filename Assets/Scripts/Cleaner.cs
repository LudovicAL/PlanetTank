using UnityEngine;

public class Cleaner : MonoBehaviour {

	public float cleanAfterDuration;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		cleanAfterDuration -= Time.fixedDeltaTime;
		if (cleanAfterDuration <= 0.0f)
			GameObject.DestroyObject(gameObject);
	}
}

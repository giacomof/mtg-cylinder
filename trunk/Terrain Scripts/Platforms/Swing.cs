using UnityEngine;
using System.Collections;

public class Swing : MonoBehaviour {
	
	public float amplitude = 1.0f;
	public float frequency = 1.0f;
	public float phase = 0.0f;
	public float oscillationAngle = 30.0f;
	private float position;
	
	public bool swingOnX = true;
	public bool swingOnZ = false;
	
	// Update is called once per frame
	void Update () {
		
		float actualY = transform.rotation.eulerAngles.y;
		
		position = amplitude * Mathf.Sin(Time.realtimeSinceStartup * frequency + phase);
		Quaternion rotation = Quaternion.identity;
		if(swingOnX) {
			rotation.eulerAngles = new Vector3(position*oscillationAngle, actualY, 0.0f);
			if(swingOnZ) {
				rotation.eulerAngles = new Vector3(position*oscillationAngle, actualY, position*oscillationAngle);
			}
		} else if(swingOnZ)
			rotation.eulerAngles = new Vector3(0.0f, actualY, position*oscillationAngle);
		else
			rotation.eulerAngles = new Vector3(transform.rotation.x, transform.rotation.y, transform.rotation.z);
			
		transform.rotation = rotation;
	}
}

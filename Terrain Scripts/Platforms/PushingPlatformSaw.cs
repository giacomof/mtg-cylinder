using UnityEngine;
using System.Collections;

public class PushingPlatformSaw : MonoBehaviour {
	
	public float amplitude = 1.0f;
	public float frequency = 1.0f;
	public float phase = 0.0f;
	public bool reverse = false;
	public bool active = true;
	private float position;
	private float originalY;
	private float time;
	
	void Start () {
		
		originalY = transform.position.y;
	}
	
	
	// Update is called once per frame
	void Update () {
		
		if(active) {
		
			time += Time.deltaTime * frequency + phase;
			
			float actualX = transform.position.x;
			float actualZ = transform.position.z;
			
			position = amplitude * (time - Mathf.Floor(time));
			Vector3 newPosition;
			if(reverse)
				newPosition = new Vector3(actualX, originalY - position, actualZ);
			else
				newPosition = new Vector3(actualX, originalY + position, actualZ);
				
			transform.position = newPosition;
			
		}
	}
}

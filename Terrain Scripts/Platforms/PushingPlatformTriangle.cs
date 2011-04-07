using UnityEngine;
using System.Collections;

public class PushingPlatformTriangle : MonoBehaviour {
	
	public float amplitude = 1.0f;
	public float frequency = 1.0f;
	public float phase = 0.0f;
	public bool active = true;
	private float position;
	private float originalY;
	private float time = 0;
	
	void Start () {
		
		originalY = transform.position.y;
	}
	
	
	// Update is called once per frame
	void Update () {
		
		if(active) {
			
			time += Time.deltaTime;
			
			float actualX = transform.position.x;
			float actualZ = transform.position.z;
			
			position = amplitude * Mathf.Asin(Mathf.Sin(time*frequency + phase));
			
			Vector3 newPosition = new Vector3(actualX, originalY + position, actualZ);
				
			transform.position = newPosition;
		}
	}
}

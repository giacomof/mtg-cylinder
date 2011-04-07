using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class PushingPlatformSinRB : MonoBehaviour {
	
	public float amplitude = 1.0f;
	public float frequency = 1.0f;
	public float phase = 0.0f;
	public bool active = true;
	private float position;
	private float originalY;
	private float time = 0;
	private Rigidbody rb;
	
	void Start () {
		
		originalY = transform.position.y;
		rb = this.rigidbody;
	}
	
	
	// Update is called once per frame
	void Update () {
		
		if(active) {
			time += Time.deltaTime;
			
			float actualX = transform.position.x;
			float actualZ = transform.position.z;
			
			position = amplitude * Mathf.Sin(time * frequency + phase);
			
			/*
			Vector3 newPosition = new Vector3(actualX, originalY + position, actualZ);
				
			transform.position = newPosition;
			*/
			
			print(position);
			
			rb.velocity = (position * Vector3.forward);
		}
	}
}

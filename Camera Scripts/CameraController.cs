using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
	
	public enum CameraFollowMode{
		StandardFollow, Override	
	}
	
	public CylController cylinder;
	public CameraFollowMode followMode = CameraFollowMode.StandardFollow;
	
	public float cameraDistanceBackward = 3, cameraDistanceUpward = 2, cameraMiniumVelocity = 10f;
	
	
	private Transform t, cylinderTransform;
	private Rigidbody cylinderRigidbody;
	private Vector3 cameraTarget, overrideTarget, forwardOrientation;
	private Camera c;
	
	private float targetFOV;
	
	// Use this for initialization
	void Start () {
		t = transform;
		cylinderTransform = (Transform)cylinder.GetComponent(typeof(Transform));
		cylinderRigidbody = (Rigidbody)cylinder.GetComponent(typeof(Rigidbody));
		c = camera;
		targetFOV = c.fov;
	}
	
	void Update () {
		// Update the camera movement
        t.position = Vector3.MoveTowards(t.position, cameraTarget, Time.deltaTime * Mathf.Max(cylinderRigidbody.velocity.magnitude, cameraMiniumVelocity));
        t.LookAt(cylinderTransform);
		
		// Update the field of vision:
		if (c.fov > targetFOV){
			c.fov -= Time.deltaTime;
			if (c.fov < targetFOV) c.fov = targetFOV;
		}
		else if (c.fov < targetFOV){
			c.fov += Time.deltaTime;
			if (c.fov > targetFOV) c.fov = targetFOV;
		}
	}
	
	void FixedUpdate() {
		switch(followMode){
		case CameraFollowMode.StandardFollow:
			forwardOrientation = Vector3.Cross(cylinder.cylinderOrientation, Vector3.up).normalized;
			cameraTarget = cylinderTransform.position - (cameraDistanceBackward * forwardOrientation) + (cameraDistanceUpward * Vector3.up);
			break;
		case CameraFollowMode.Override:
			cameraTarget = overrideTarget;
			break;
		}
	}
	
	public void Override(Vector3 position){
		followMode = CameraFollowMode.Override;
		overrideTarget = position;
	}
	
	
	
	public void setFOV(float fov){
		targetFOV = fov;
	}
	
	//For handling camera issues:
	void OnCollisionEnter(Collision collision){
		
	}
	
	

}

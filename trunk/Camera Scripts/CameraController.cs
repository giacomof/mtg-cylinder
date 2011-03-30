using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour {
	
	public enum CameraFollowMode{
		StandardFollow, Override, FixedSpeed	
	}
	
	public CylController cylinder;
	public CameraFollowMode followMode = CameraFollowMode.StandardFollow;
	
	public float cameraDistanceBackward = 3, cameraDistanceUpward = 2, cameraMiniumVelocity = 10f;
	
	
	private Transform t, cylinderTransform;
	private Rigidbody cylinderRigidbody;
	private Vector3 cameraTarget, overrideTarget, forwardOrientation;
	private Camera c;
	
	private float targetFOV, fixSpeed;
	
	// Use this for initialization
	void Start () {
		t = transform;
		cylinderTransform = (Transform)cylinder.GetComponent(typeof(Transform));
		cylinderRigidbody = (Rigidbody)cylinder.GetComponent(typeof(Rigidbody));
		c = camera;
		targetFOV = c.fov;
	}
	
	void Update () {
		float speed;
		if (followMode == CameraController.CameraFollowMode.FixedSpeed){
			speed = fixSpeed;
		}
		else{
			speed = (cameraTarget - t.position).magnitude;
		}
		
		// Update the camera movement
        t.position = Vector3.MoveTowards(t.position, cameraTarget, Time.deltaTime * speed * 3.5f);
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
		case CameraFollowMode.Override:
			cameraTarget = overrideTarget;
			break;
		default:
			forwardOrientation = Vector3.Cross(cylinder.cylinderOrientation, Vector3.up).normalized;
			cameraTarget = cylinderTransform.position - (cameraDistanceBackward * forwardOrientation) + (cameraDistanceUpward * Vector3.up);
			break;
		}
	}
	
	public void Override(Vector3 position){
		if (followMode != CameraFollowMode.Override){
			followMode = CameraFollowMode.Override;
			overrideTarget = position;
		}
	}
	
	public void Revert(){
		followMode = CameraFollowMode.StandardFollow;	
	}
	
	public Vector3 getCurrentTarget(){
		return cameraTarget;	
	}
	
	public void setFOV(float fov){
		targetFOV = fov;
	}
	
	public void setSpeed(float speed){
		fixSpeed = speed;
		followMode = CameraController.CameraFollowMode.FixedSpeed;
	}
	
	//For handling camera issues:
	void OnCollisionEnter(Collision collision){
		
	}
	
	

}

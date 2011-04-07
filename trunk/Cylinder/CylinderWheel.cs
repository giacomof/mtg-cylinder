using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CylinderWheel : MonoBehaviour
{
    public CylController cylinder;
	public Transform wheelModel;
	public Vector3 forward_orientation, downward_orientation, orientation_right;
	private Vector3[] forward_vectors;
	
    private bool _onGround = false, _gravityAffects = false, _isJumping = false;
    private Rigidbody rb;
	private Rigidbody relativeGround;
	private Vector3 addedVelo, collisionNormal = Vector3.up;
	private ContactPoint[] contactPoints;
	
	private int layerMask;
	private MeshRenderer mRend;
	private RaycastHit closestHit = new RaycastHit();
	
	private float currentThrottle = 0;
	private float targetVelocity = 0, diff;
	
	void Start()
    {
		forward_vectors = new Vector3[20];
        rb = this.rigidbody;
		mRend = (MeshRenderer)wheelModel.GetComponent(typeof(MeshRenderer));
		// Create the layermask for detecting magnetic surfaces
		layerMask = 1 << 9;
    }

    void Update()
    {	
		animateWheel();
		splitVelocity();
		
		if (_onGround && !_gravityAffects && !_isJumping){
			// On ground:
			
			//throttle:
			diff = targetVelocity - currentThrottle;
			if (diff > 0){
				
				if (targetVelocity * currentThrottle <= 0){
					//decceleration
					currentThrottle = CylController.ceil(currentThrottle + (cylinder.decceleration * Time.deltaTime), targetVelocity);
				}
				else {
					//acceleration
					currentThrottle = CylController.ceil(currentThrottle + (cylinder.acceleration * Time.deltaTime), targetVelocity);	
				}
				
			}
			else if (diff < 0){
				
				if (targetVelocity * currentThrottle <= 0){
					//decceleration
					currentThrottle = CylController.floor(currentThrottle - (cylinder.decceleration * Time.deltaTime), targetVelocity);
				}
				else {
					//acceleration
					currentThrottle = CylController.floor(currentThrottle - (cylinder.acceleration * Time.deltaTime), targetVelocity);	
				}
				
			}
			//assign velocity:
			rb.velocity = (currentThrottle * forward_orientation);
			
			if (relativeGround){
				rb.velocity += relativeGround.velocity;
				Debug.Log("Added force: " + relativeGround.velocity);
			}
		}
	}
	
	
	private void splitVelocity(){
		if (relativeGround){
				rb.velocity -= relativeGround.velocity;
				Debug.Log("Added force: " + relativeGround.velocity);
			}
		currentThrottle = Vector3.Dot(rb.velocity, forward_orientation);
		//limit the throttle by max value:
		if (currentThrottle > cylinder.maxThrottlePower){
			currentThrottle = cylinder.maxThrottlePower;
		}
	}
	
	private void animateWheel(){
		//animate the wheels:
		wheelModel.RotateAroundLocal(Vector3.up, targetVelocity * Time.deltaTime * cylinder.animationSpeed);
	}
	
	
	
    void FixedUpdate() {
        computeOrientation();
		applyExternalForces();
    }
	
	private void computeOrientation()
	{
        //Calculate orientation vectors:
		
        forward_orientation = Vector3.Cross(cylinder.cylinderOrientation, collisionNormal).normalized;
        downward_orientation = -collisionNormal;
		orientation_right = Vector3.Cross(forward_orientation, downward_orientation).normalized;
    }
	
	private void applyExternalForces()
	{
		//Gravity and magnetism:
		switch (cylinder.cylinderMode){
		case CylController.CylinderMode.Normal:
			if (collisionNormal.y < .3f || !_onGround){
				applyGravity();
				_gravityAffects = true;
			}
			else _gravityAffects = false;
			break;
		case CylController.CylinderMode.Magnetic:
			if (!_onGround){
				float distance = cylinder.magnetismRadius + 1;
				for (int i = 0; i < 8; i++){
					RaycastHit[] hits = Physics.RaycastAll(rb.position, getRaycastDir(i), cylinder.magnetismRadius, layerMask);
					for (int h = 0; h < hits.Length; h++){
						if (distance > hits[h].distance){
							closestHit = hits[h];
							distance = closestHit.distance;
						}
					}
				}
				if (closestHit.normal.magnitude > 0){
					rb.AddForce((-closestHit.normal * cylinder.magnetismForce) + (-closestHit.normal * cylinder.getRigidbody().velocity.magnitude));
				}
				else{
					applyGravity();
				}
				_gravityAffects = true;
			}
			else _gravityAffects = false;
			break;
		}
	}
	
	private void applyGravity(){
		//Apply the custom gravity:
		rb.AddForce(Vector3.down * cylinder.gravityPower);
		cylinder.getRigidbody().AddForce(Vector3.down * cylinder.gravityPower * .5f);
		_isJumping = false;
	}
	
	private Vector3 getRaycastDir(int index){
		switch(index){
		case 0:
			return collisionNormal;
		case 1:
			return -collisionNormal;
		case 2:
			return forward_orientation;
		case 3:
			return -forward_orientation;
		case 4:
			return (collisionNormal + forward_orientation);
		case 5:
			return (-collisionNormal + forward_orientation);
		case 6:
			return (collisionNormal - forward_orientation);
		default:
			return (-collisionNormal - forward_orientation);
		}
	}
	
	public void doJump() {
		if (_onGround){
			_isJumping = true;
			rb.AddForce(collisionNormal * cylinder.jumpPower, ForceMode.Impulse);
		}
	}
	
	public void throttleTo(float velocity){
		targetVelocity = velocity;
	}

    
	private char[] separator = new char[]{' '};
    void OnCollisionStay(Collision collision)
    {
		string[] tags = collision.gameObject.tag.Split(separator);
		for (int i = 0; i < tags.Length; i++){
			
			switch (tags[i]){
			case "Ground":
				//We have touched ground:
				_onGround = true;
				//Get the collision normal (awesome feature!!!!!):
				collisionNormal = collision.contacts[0].normal;
				break;
				
			case "Moving":
				
				relativeGround = collision.rigidbody;
				break;
			}
		}
    }

    void OnCollisionExit(Collision collision)
    {
		string[] tags = collision.gameObject.tag.Split(separator);
		for (int i = 0; i < tags.Length; i++){
			
			switch (tags[i]){
			case "Ground":
				//We have left the ground:
				_onGround = false;
				break;
			case "Moving":
				relativeGround = null;
				break;
			}
		}
    }
	
	public bool OnGround() {
		return _onGround;
	}
	
	public void setMaterial(Material mat){
		mRend.material = mat;	
	}
}

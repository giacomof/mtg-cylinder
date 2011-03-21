using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CylinderWheel : MonoBehaviour
{
    public CylController cylinder;
	public Transform wheelModel;
	public Vector3 forward_orientation, downward_orientation, orientation_right;
	
	public float throttleValue;
	
    bool _onGround = false;
    private Rigidbody rb;
	private Vector3 addedVelo, collisionNormal = Vector3.up;
	
	// Force applyed to the wheel when jumping
	public float jumpForce = 100.0f;
	private bool haveToJump = false;
	//Air steering
	private bool airSteering = false;
	private float airPower = 2f;	//Should probably be in the range of 1-2
	private int airDirection = 0;
	
	
    void Start()
    {
        rb = this.rigidbody;
    }

    void Update()
    {
		//animate the wheels:
		wheelModel.RotateAroundLocal(Vector3.up, throttleValue * Time.deltaTime * cylinder.animationSpeed);
    }

    void FixedUpdate() {
        computeOrientation();
        if (_onGround) {
			if (haveToJump) {
				rb.AddForce(collisionNormal * jumpForce, ForceMode.Impulse);
				haveToJump = false;
			}
			
			if (throttleValue == 0) {
				// No throttle applied:
				rb.drag = cylinder.brakeDrag;
			}
			else {
				addedVelo = forward_orientation * throttleValue;
				
				// Throttle applied:
				rb.drag = cylinder.accelerationDrag;
				rb.AddForce(addedVelo);
			}
        }
		else {
            rb.drag = cylinder.accelerationDrag;
			if (airSteering) {
				rb.AddForce(airDirection * orientation_right * airPower, ForceMode.Impulse);
				airSteering = false;
			}
        }
		
		// If the key is pressed and we are onGround, 
		// then apply the jump force on the collisionNormal direction
		//if (Input.GetKey("j") && _onGround)
    }
	
	public void doJump() {
		haveToJump = true;
	}
	
	public void doSteer(bool goingLeft) {
		airSteering = true;
		if (goingLeft)
			airDirection = -1;
		else
			airDirection = 1;
	}

    void computeOrientation()
	{
        //Calculate orientation vectors:
        forward_orientation = Vector3.Cross(cylinder.cylinderOrientation, collisionNormal).normalized;
        downward_orientation = -collisionNormal;
		orientation_right = Vector3.Cross(forward_orientation, downward_orientation);
		print(orientation_right);
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == cylinder.ground_tag)
        {
            //We have touched ground:
            _onGround = true;
			//Debug.DrawLine(rb.position, rb.position + (collision.contacts[0].normal * 3), new Color(0, 0, 0));
			//Get the collision normal (awesome feature!!!!!):
			collisionNormal = collision.contacts[0].normal;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == cylinder.ground_tag)
        {
            //We have left the ground:
            _onGround = false;
			
        }
    }
	
	public bool OnGround() {
		return _onGround;
	}	
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CylinderWheel : MonoBehaviour
{



    public CylController cylinder;
	public Vector3 forward_orientation, backward_orientation, downward_orientation/*, upward_orientation*/;
    public Transform orientationPointFront, orientationPointRear;
	public float throttleValue;
	public Transform wheelModel;
	
    bool _onGround = false;
    private Rigidbody rb;
	private Vector3 addedVelo;

    void Start()
    {
        rb = this.rigidbody;
    }

    void Update()
    {
		//animate the wheels:
		wheelModel.RotateAroundLocal(Vector3.up, throttleValue * Time.deltaTime * cylinder.animationSpeed);
    }

    void FixedUpdate()
    {
        computeOrientation();
        if (_onGround)
        {
			if (throttleValue == 0)
			{
				// No throttle applied:
				rb.drag = cylinder.brakeDrag;
			}
			else{
				addedVelo = Vector3.zero;
				if (throttleValue > 0)
					addedVelo += forward_orientation * throttleValue;
				if (throttleValue < 0) 
					addedVelo -= backward_orientation * throttleValue;
				
				// Throttle applied:
				rb.drag = cylinder.accelerationDrag;
				rb.AddForce(addedVelo);
			}
			
			// Apply some custom inertia:
            rb.AddForce(downward_orientation * rb.velocity.magnitude * cylinder.downwardForceFactor);
        }
        else
        {
            rb.drag = cylinder.accelerationDrag;
        }
    }

    void computeOrientation()
    {
        Vector3 forward = Vector3.Cross(cylinder.cylinderOrientation, Vector3.up).normalized;
        RaycastHit hit = new RaycastHit();

        //Forward orientation:
        orientationPointFront.position = rb.position + forward + (Vector3.up * .85f);
        if (Physics.Raycast(orientationPointFront.position, Vector3.down, out hit, 2))
        {
            orientationPointFront.Translate(Vector3.down * hit.distance, Space.World);
            orientationPointFront.Translate(Vector3.up * .6f, Space.World);
        }
        else
        {
            orientationPointFront.position = rb.position + forward;
        }

        //Backward orientation:
        orientationPointRear.position = rb.position - forward + (Vector3.up * .85f);
        if (Physics.Raycast(orientationPointRear.position, Vector3.down, out hit, 2))
        {
            orientationPointRear.Translate(Vector3.down * hit.distance, Space.World);
            orientationPointRear.Translate(Vector3.up * .6f, Space.World);
        }
        else
        {
            orientationPointRear.position = rb.position - forward;
        }

        //Calculate orientation vectors:
        forward_orientation = (orientationPointFront.position - rb.position).normalized;
        backward_orientation = (orientationPointRear.position - rb.position).normalized;
        downward_orientation = Vector3.Cross(cylinder.cylinderOrientation, forward_orientation).normalized;
		//upward_orientation = Vector3.Cross(cylinder.cylinderOrientation, backward_orientation).normalized;
    }

    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == cylinder.ground_tag)
        {
            //We have touched ground:
            _onGround = true;
            //Debug.Log(gameObject.name + " ground touched!");
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == cylinder.ground_tag)
        {
            //We have left the ground:
            _onGround = false;
            //Debug.Log(gameObject.name + " ground left!");
        }
    }
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CylinderWheel : MonoBehaviour
{
    public CylController cylinder;
	public Transform wheelModel;
	public Vector3 forward_orientation, downward_orientation, orientation_right;
	
	public float throttleValue;
	
    bool _onGround = false, _onMagnet = false;
    private Rigidbody rb;
	private Vector3 addedVelo, collisionNormal = Vector3.up;
	
	// Force applyed to the wheel when jumping
	public float jumpForce = 100.0f;
	private bool haveToJump = false;
	//Air steering
	private bool airSteering = false;
	private bool justAttached = false;
	private float airPower = 2f;	//Should probably be in the range of 1-2
	private int airDirection = 0;
	
	private int layerMask;
	private MeshRenderer mRend;
	private ContactPoint[] contactPoints;
    void Start()
    {
        rb = this.rigidbody;
		mRend = (MeshRenderer)wheelModel.GetComponent(typeof(MeshRenderer));
		
		layerMask = 1 << 9;
    }

    void Update()
    {
		//animate the wheels:
		wheelModel.RotateAroundLocal(Vector3.up, throttleValue * Time.deltaTime * cylinder.animationSpeed);
    }
	
	private RaycastHit hit = new RaycastHit();
	
    void FixedUpdate() {
        computeOrientation();
		
		
		if (cylinder.cylinderMode == CylController.CylinderMode.Magnetic){
			mRend.material = cylinder.magnetMaterial;
			rb.drag = cylinder.magnetModeDrag;
			if (_onMagnet){
				if (collisionNormal.y < .3f){
					rb.useGravity = false;
				}
				else{
					rb.useGravity = true;	
				}	
			}
			else{
				rb.useGravity = true;	
			}
		}
		else{
			mRend.material = cylinder.standardMaterial;	
			rb.useGravity = true;
		}
		
        if (_onGround) {
			// On ground:
			
			//Jump behaviour:
			if (haveToJump) {
				rb.AddForce(collisionNormal * jumpForce, ForceMode.Impulse);
				haveToJump = false;
			}
			
			
			
			
			//Normal behaviour:
			if (throttleValue == 0) {
				// No throttle applied:
				rb.drag = cylinder.brakeDrag;
			}
			else {
				rb.drag = cylinder.accelerationDrag;
				addedVelo = forward_orientation * throttleValue;
				rb.AddForce(addedVelo);
				
			}	
        }
		else {
			// In air:
			rb.drag = cylinder.accelerationDrag;
			//Magnetic behaviour:
			if (cylinder.cylinderMode == CylController.CylinderMode.Magnetic){
				float distance = cylinder.magnetismRadius + 1;
				RaycastHit closestHit = hit;
				for (int i = 0; i < 8; i++){
					RaycastHit[] hits = Physics.RaycastAll(rb.position, getRaycastDir(i), cylinder.magnetismRadius, layerMask);
					for (int h = 0; h < hits.Length; h++){
						if (distance > hits[h].distance){
							closestHit = hits[h];
							distance = closestHit.distance;
						}
					}
				}
				
				for (int i = 0; i < contactPoints.Length; i++){
					RaycastHit[] hits = Physics.RaycastAll(rb.position, contactPoints[i].point - rb.position, cylinder.magnetismRadius, layerMask);
					for (int h = 0; h < hits.Length; h++){
						if (distance > hits[h].distance){
							closestHit = hits[h];
							distance = closestHit.distance;
						}
					}
				}
				
				if (closestHit.normal.magnitude == 0){
					
				}
				else{
					rb.AddForce((-closestHit.normal * cylinder.magnetismForce) + (-closestHit.normal * cylinder.getRigidbody().velocity.magnitude));
				}
			}
			
			//Gravity hack:
			if (rb.useGravity){
				rb.AddForce(Vector3.down * rb.mass * 50);
			}
			
            //Air steering:
			if (airSteering) {
				rb.AddForce(airDirection * orientation_right * airPower, ForceMode.Impulse);
				airSteering = false;
			}
        }
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
    }
	
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == cylinder.ground_tag)
        {
			//We have touched magnet:
			if (collision.gameObject.layer == 9){
				_onMagnet = true;
			}
            //We have touched ground:
            _onGround = true;
			
			//Get the collision normal (awesome feature!!!!!):
			collisionNormal = collision.contacts[0].normal;
			
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == cylinder.ground_tag)
        {
			if (collision.gameObject.layer == 9){
				contactPoints = collision.contacts;
				_onMagnet = false;
			}
            //We have left the ground:
            _onGround = false;
        }
    }
	
	public bool OnGround() {
		return _onGround;
	}
}

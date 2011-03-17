using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CylController : MonoBehaviour
{
	// Enum for the different input devices
    public enum ControlScheme
    {
        Keyboard, RumblePad, XBoxController
	}
	
	
    public enum CylinderMode
    {
        Normal, Spiky
    }
	
	// Wheels variables
    public Rigidbody leftWheel, rightWheel;
    private Transform lw_t, rw_t;
	
	// Movement variables
    public float accelerationDrag = 0, brakeDrag = 10, jumpForce, throttleForce = 10, wheelMass = 4, downwardForceFactor = 2.0f;
	private Transform t, ct;
    private Rigidbody rb;
    private CylinderWheel l_cyl_w, r_cyl_w;
	
	// Control variables
	public ControlScheme controlScheme = ControlScheme.Keyboard;
	public float minAnalogStickDifference = .2f;
	
	//Camera variables
    public Camera followingCamera;
    public float cameraDistanceBackward = 3, cameraDistanceUpward = 2, cameraMiniumVelocity = 10f;
	private Vector3 cameraTarget;
    private Vector3 orientation_forward, orientation_downward;
	
	// Ground recognition tag
    public string ground_tag = "Ground";
    public Vector3 cylinderOrientation;
	public float animationSpeed = .1f;
	

    void Start()
    {
		// Assign all the used variables
        lw_t = leftWheel.transform;
        rw_t = rightWheel.transform;
        leftWheel.mass = wheelMass;
        rightWheel.mass = wheelMass;
        t = this.transform;
        rb = this.rigidbody;
        ct = followingCamera.transform;

        l_cyl_w = (CylinderWheel)leftWheel.GetComponent(typeof(CylinderWheel));
        r_cyl_w = (CylinderWheel)rightWheel.GetComponent(typeof(CylinderWheel));
    }
	

    void Update()
    {
        // Update the camera movement
        ct.position = Vector3.MoveTowards(ct.position, cameraTarget, Time.deltaTime * Mathf.Max(rb.velocity.magnitude, cameraMiniumVelocity));
        ct.LookAt(t);
    }

    void FixedUpdate()
    {
		// Calculate the orientation and camera position
        computeOrientation();
        cameraTarget = computeCameraPosition();
		// Calculate the axis values
		calculateAxis();
		
        if (Input.GetButtonDown("Fire1"))	print("Button 1");
        if (Input.GetButtonDown("Fire2"))	print("Button 2");
        if (Input.GetButtonDown("Fire3"))	print("Button 3");
        if (Input.GetButtonDown("Jump"))	print("Button 4");
        //if (Input.GetButtonDown("Fire4"))	print("Button 5");
        //if (Input.GetButtonDown("Fire5"))	print("Button 6");
        //if (Input.GetButtonDown("Fire6"))	print("Button 7");
        if (Input.GetButtonDown("Fire7"))	//print("Button 8");
			rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        if (Input.GetButtonDown("Fire8"))	print("Button 9");
        if (Input.GetButtonDown("Fire9"))	//print("Button 10");
            Application.LoadLevel(0);
        //}
		//if (Input.GetButtonDown("Fire10"))	print("Button 11");
		//if (Input.GetButtonDown("Fire11"))	print("Button 12");
        //else if (cylinder.controlScheme == CylController.ControlScheme.XBoxController)
    }
	
	private void calculateAxis(){
		
		float leftWheelAxisValue = 0, rightWheelAxisValue = 0;
		
		// Different input systems related to the devices
		switch (controlScheme){
		case ControlScheme.Keyboard:
			if (Input.GetKey("q"))
				leftWheelAxisValue += 1;
			if (Input.GetKey("a"))
				leftWheelAxisValue -= 1;
			if (Input.GetKey("e"))
				rightWheelAxisValue += 1;
			if (Input.GetKey("d"))
				rightWheelAxisValue -= 1;
			break;
		case ControlScheme.RumblePad:
			leftWheelAxisValue = Input.GetAxis("Vertical");
			rightWheelAxisValue = Input.GetAxis("VerticalRight");
			break;
		case ControlScheme.XBoxController:
			leftWheelAxisValue = Input.GetAxis("Vertical");
			rightWheelAxisValue = Input.GetAxis("VerticalRightXBox");
			break;
		}
		
		// Code for uniforming the wheel speed to make easier to go straight ahead
		if (leftWheelAxisValue != 0 && rightWheelAxisValue != 0){
			float difference = Mathf.Abs(leftWheelAxisValue - rightWheelAxisValue);
			
			if (difference < minAnalogStickDifference){
				leftWheelAxisValue = Mathf.Max(leftWheelAxisValue, rightWheelAxisValue);
				rightWheelAxisValue = leftWheelAxisValue;
			}	
		}
		
		// Assign to the wheels the right velocity
		l_cyl_w.throttleValue = leftWheelAxisValue * throttleForce;
		r_cyl_w.throttleValue = rightWheelAxisValue * throttleForce;
	}
	
	// Calculate the camera position in relation to the orientation points
    private Vector3 computeCameraPosition()
    {
        Vector3 forward = (l_cyl_w.forward_orientation - l_cyl_w.backward_orientation + r_cyl_w.forward_orientation - r_cyl_w.backward_orientation).normalized;
        Vector3 downward = (l_cyl_w.downward_orientation + r_cyl_w.downward_orientation).normalized;
        return t.position + (forward * -cameraDistanceBackward) + (downward * -cameraDistanceUpward);
    }
	
	// Find the right orientation for the camera direction
    private void computeOrientation()
    {
        this.cylinderOrientation = rw_t.position - lw_t.position;
    }


}

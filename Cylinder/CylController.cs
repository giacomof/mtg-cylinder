using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CylController : MonoBehaviour
{
	// Enum for the different input devices
    public enum ControlScheme {
        Keyboard, RumblePad, XBoxController
	}
	
    public enum CylinderMode {
        Normal, Spiky
    }
	
	/*
    // Variables to change size on the fly
    private int stepSize = 2;
    private Vector3 localScale;
    private bool sizeKeyPressed = false;
    private bool sizeChanged = false;
    */
	
	// Variables to change speed on the fly
	private int stepSpeed = 0;
	private bool speedKeyPressed = false;
	private bool speedChanged = false;
	
	// Wheels variables
    public Rigidbody leftWheel, rightWheel;
    private Transform lw_t, rw_t;
	
	// Movement variables
    public float accelerationDrag = 0, brakeDrag = 10, throttleForce = 10, wheelMass = 4, downwardForceFactor = 2.0f;
	private Transform t, ct;
    private Rigidbody rb;
    private CylinderWheel l_cyl_w, r_cyl_w;
	// Turbo variables
	public float turboDuration = 2.0f;
	public float turboRechargeDuration = 1.0f;
	public float turboFOV = 90.0f;
	public float originalFOV = 60.0f;
	public float fovIncrement = 0.5f;
	private int previousSpeedStep;
	private bool canTurbo = true;
	private bool alreadyTurbo = false;
	private float turboTimer;
	
	// Control variables
	public ControlScheme controlScheme/* = ControlScheme.Keyboard*/;
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
	
	//Axis input
	float leftWheelXValue = 0, leftWheelYValue = 0, rightWheelXValue = 0, rightWheelYValue = 0;
	
    void Start() {
		// Assign all the used variables
        lw_t = leftWheel.transform;
        rw_t = rightWheel.transform;
        leftWheel.mass = wheelMass;
        rightWheel.mass = wheelMass;
        t = this.transform;
        rb = this.rigidbody;
        ct = followingCamera.transform;
		
		/*
        // Copy the local scale to use it later
        localScale = t.localScale;
        */	

        l_cyl_w = (CylinderWheel)leftWheel.GetComponent(typeof(CylinderWheel));
        r_cyl_w = (CylinderWheel)rightWheel.GetComponent(typeof(CylinderWheel));
    }
	
    void Update() {
        // Update the camera movement
        ct.position = Vector3.MoveTowards(ct.position, cameraTarget, Time.deltaTime * Mathf.Max(rb.velocity.magnitude, cameraMiniumVelocity));
        ct.LookAt(t);
		
		//Adjust for Air Steering
		AirSteering();
		
		/*
        // If the size changed than apply it to the trasformation of the object
        if (sizeChanged)
        {
            switch (stepSize)
            {
                case 0:
                    localScale.y = 0.8f;
                    break;
                case 1:
                    localScale.y = 1.2f;
                    break;
                case 2:
                    localScale.y = 1.668569f;
                    break;
                case 3:
                    localScale.y = 2.0f;
                    break;
                case 4:
                    localScale.y = 2.5f;
                    break;
            }

            t.localScale = localScale;
            sizeChanged = false;
        }
        */
		
		//Turbo code
		if((canTurbo && !alreadyTurbo) && Input.GetKey("t")) {
			alreadyTurbo = true;
			turboTimer = Time.realtimeSinceStartup;
			speedChanged = true;
			throttleForce = 1600;
		} else if (Time.realtimeSinceStartup - turboTimer > turboDuration) {
			alreadyTurbo = false;
			canTurbo = false;
			turboTimer = Time.realtimeSinceStartup;
			speedChanged = true;
			stepSpeed = previousSpeedStep;
		}
		
		if(!alreadyTurbo && followingCamera.fov > originalFOV)
			followingCamera.fov -= fovIncrement;
		else if(alreadyTurbo && followingCamera.fov < turboFOV)
			followingCamera.fov += fovIncrement;
		
		if(!canTurbo) {
			if (Time.realtimeSinceStartup - turboTimer > turboRechargeDuration)
				canTurbo = true;
		}
			
			
		
		
		// If the size changed than apply it to the trasformation of the object
		if(speedChanged) {
			switch(stepSpeed) {
				case 0:
					throttleForce = 800;
					break;
				case 1:
					throttleForce = 1200;
					break;
				case 2:
					throttleForce = 1600;
					break;
				case 3:
					throttleForce = 2000;
					break;
				case 4:
					throttleForce = 2400;
					break;
			}
			speedChanged = false;
		}
		
		/*
        // Input managing for changing size
        if (!sizeKeyPressed)
        {
            if (Input.GetKey("p"))
            {
                if (stepSize < 4)
                    stepSize += 1;
                sizeKeyPressed = true;
                sizeChanged = true;
            }
            if (Input.GetKey("l"))
            {
                if (stepSize > 0)
                    stepSize -= 1;
                sizeKeyPressed = true;
                sizeChanged = true;
            }
        }
        else if (!Input.GetKey("p") && !Input.GetKey("l"))
          sizeKeyPressed = false;
        */
		
		// Input managing for changing speed
		if (!speedKeyPressed) {
			if (Input.GetKey("o")) {
					if (stepSpeed < 4) {
						stepSpeed += 1;
						previousSpeedStep = stepSpeed;
					}
					speedKeyPressed = true;
					speedChanged = true;
			}
			if (Input.GetKey("k")) {
					if (stepSpeed > 0) {
						stepSpeed -= 1;
						previousSpeedStep = stepSpeed;
					}
					speedKeyPressed = true;
					speedChanged = true;
			}
		}
		else if (!Input.GetKey("o") && !Input.GetKey("k"))
			speedKeyPressed = false;
		
		
		
    }

    void FixedUpdate()
    {
		// Calculate the orientation and camera position
        computeOrientation();
        cameraTarget = computeCameraPosition();
		// Calculate the axis values
		calculateAxis();
		
		switch (controlScheme) {
			case ControlScheme.Keyboard:
				if (Input.GetKey("j")) {
					l_cyl_w.doJump();
					r_cyl_w.doJump();
				}
				if (Input.GetKey("u")) {
					l_cyl_w.doJump();
				}
				if (Input.GetKey("i")) {
					r_cyl_w.doJump();
				}
				break;
			case ControlScheme.RumblePad:
				if (Input.GetButtonDown("Fire1"))	print("Button 1");
				if (Input.GetButtonDown("Fire2"))	print("Button 2");
				if (Input.GetButtonDown("Fire3"))	print("Button 3");
				if (Input.GetButtonDown("Jump"))	print("Button 4");
				if (Input.GetButtonDown("Fire4"))	//print("Button 5");
					l_cyl_w.doJump();
				if (Input.GetButtonDown("Fire5"))	//print("Button 6");
					r_cyl_w.doJump();
				if (Input.GetButtonDown("Fire6"))	print("Button 7");
				if (Input.GetButtonDown("Fire7")) {//print("Button 8");
					l_cyl_w.doJump();
					r_cyl_w.doJump();
				}
				if (Input.GetButtonDown("Fire8"))	print("Button 9");
				if (Input.GetButtonDown("Fire9"))	//print("Button 10");
					Application.LoadLevel(0);
				if (Input.GetButtonDown("Fire10"))	print("Button 11");
				if (Input.GetButtonDown("Fire11"))	print("Button 12");
				break;
			case ControlScheme.XBoxController:
				if (Input.GetButtonDown("Fire1"))	print("Button 1");
				if (Input.GetButtonDown("Fire2"))	print("Button 2");
				if (Input.GetButtonDown("Fire3"))	print("Button 3");
				if (Input.GetButtonDown("Jump"))	print("Button 4");
				if (Input.GetButtonDown("Fire4"))	print("Button 5");
				if (Input.GetButtonDown("Fire5"))	print("Button 6");
				if (Input.GetButtonDown("Fire6"))	print("Button 7");
				if (Input.GetButtonDown("Fire7"))	print("Button 8");
				if (Input.GetButtonDown("Fire8"))	print("Button 9");
				if (Input.GetButtonDown("Fire9"))	print("Button 10");
				if (Input.GetButtonDown("Fire10"))	print("Button 11");
				if (Input.GetButtonDown("Fire11"))	print("Button 12");				
				break;
		}
    }
	
	private void calculateAxis(){
			leftWheelXValue = 0;
			leftWheelYValue = 0; 
			rightWheelXValue = 0;
			rightWheelYValue = 0;
		
		// Different input systems related to the devices
		switch (controlScheme){
		case ControlScheme.Keyboard:
			if (alreadyTurbo) {
				leftWheelYValue += 1;
				rightWheelYValue += 1;
			} else {
				if (Input.GetKey("q"))
				leftWheelYValue += 1;
				if (Input.GetKey("a"))
				leftWheelYValue -= 1;
				if (Input.GetKey("e"))
				rightWheelYValue += 1;
				if (Input.GetKey("d"))
				rightWheelYValue -= 1;
			}
			break;
		case ControlScheme.RumblePad:
			leftWheelXValue	= Input.GetAxis("Horizontal");
			leftWheelYValue	= Input.GetAxis("Vertical");
			rightWheelXValue	= Input.GetAxis("HorizontalRight");
			rightWheelYValue	= Input.GetAxis("VerticalRight");
			break;
		case ControlScheme.XBoxController:
			leftWheelYValue 	= Input.GetAxis("Vertical");
			rightWheelYValue = Input.GetAxis("VerticalRightXBox");
			break;
		}
		
		// Code for uniforming the wheel speed to make easier to go straight ahead
		if (leftWheelYValue != 0 && rightWheelYValue != 0){
			float difference = Mathf.Abs(leftWheelYValue - rightWheelYValue);
			
			if (difference < minAnalogStickDifference){
				leftWheelYValue = Mathf.Max(leftWheelYValue, rightWheelYValue);
				rightWheelYValue = leftWheelYValue;
			}	
		}
			
		
		// Assign to the wheels the right velocity
		l_cyl_w.throttleValue = leftWheelYValue * throttleForce;
		r_cyl_w.throttleValue = rightWheelYValue * throttleForce;
	}
	
	// Calculate the camera position in relation to the orientation points
    private Vector3 computeCameraPosition()
    {
        Vector3 forward = (l_cyl_w.forward_orientation + r_cyl_w.forward_orientation).normalized;
        Vector3 downward = (l_cyl_w.downward_orientation + r_cyl_w.downward_orientation).normalized;
        return t.position + (forward * -cameraDistanceBackward) + (downward * -cameraDistanceUpward);
    }
	
	// Find the right orientation for the camera direction
    private void computeOrientation()
    {
        this.cylinderOrientation = rw_t.position - lw_t.position;
    }
	
	void AirSteering() {
		if (l_cyl_w.OnGround() && r_cyl_w.OnGround()) {
		}
		else {
			if (leftWheelXValue < 0 && leftWheelXValue < 0) {
				l_cyl_w.doSteer(true);
				r_cyl_w.doSteer(true);
			}
			if (leftWheelXValue > 0 && rightWheelXValue > 0) {
				l_cyl_w.doSteer(false);
				r_cyl_w.doSteer(false);
			}
		}
	}
}

using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class CylController : MonoBehaviour
{
	// Enum for the different input devices
	public enum ControlScheme {
		Keyboard, RumblePad, XBoxController
	}
	
	// Enum for the different cylinder modes
	public enum CylinderMode {
		Normal, Magnetic
	}
	
	private Transform t;
	private Rigidbody rb;
	private CylinderWheel l_cyl_w, r_cyl_w;
	private Transform leftWheel_Field, rightWheel_Field;
	
	// Button booleans
	private bool jumpButtonPressed = false;
	private bool cylinderModeChanged = false;
	
	// Wheels variables
	public Rigidbody leftWheel, rightWheel;
	private Transform lw_t, rw_t;
	
	// Movement variables
	public float throttlePower = 40, magnetThrottlePower = 20, maxThrottlePower = 120, turboThrottlePower = 80, jumpPower = 100;
	public float gravityPower = 150;
	public float acceleration = 55, decceleration = 80;
	
	// Turbo variables
	public float turboDuration = 2000.0f;
	public float turboRechargeDuration = 1000.0f;
	public float originalFOV = 60.0f, turboFOV = 90.0f;
	
	private bool isTurbo = false;
	private float turboTimer = -1;
	private float turboRechargeTimer = -1;
	
	// Control variables
	public ControlScheme controlScheme;
	public CylinderMode cylinderMode = CylinderMode.Normal;
	public float analogStickThreshold = .01f;
	public float uniformSteeringThreshold = .87f;
	public float speedDiffThreshold = .77f;
	
	//Camera variables
	public CameraController cameraController;
	
	// Ground recognition tag
	public string ground_tag = "Ground";
	public Vector3 cylinderOrientation;
	public float animationSpeed = .1f;
	
	public float magnetismRadius = 7.0f;
	public float magnetismForce = 600;
	public Material standardMaterial, magnetMaterial;
	
	//Axis input
	float leftWheelXValue = 0, leftWheelYValue = 0, rightWheelXValue = 0, rightWheelYValue = 0;
	
	void Start() {
		// Assign all the used variables
		lw_t = leftWheel.transform;
		rw_t = rightWheel.transform;
		t = this.transform;
		rb = this.rigidbody;
		
		l_cyl_w = (CylinderWheel)leftWheel.GetComponent(typeof(CylinderWheel));
		r_cyl_w = (CylinderWheel)rightWheel.GetComponent(typeof(CylinderWheel));
		
		leftWheel_Field = transform.Find("LeftWheelField");
		rightWheel_Field = transform.Find("RightWheelField");
	}
	
	public Rigidbody getRigidbody(){
		return rb;	
	}
	
	public Transform getTransform(){
		return t;	
	}
	
	void Update() {
		if(turboTimer > 0) {
			turboTimer -= Time.deltaTime;
			Debug.Log("Turbo: " + turboTimer);
			if (turboTimer <= 0){
				turboRechargeTimer = turboRechargeDuration;
			}
		}
		else if (turboRechargeTimer > 0){
			turboRechargeTimer -= Time.deltaTime;
			Debug.Log("Recharge: " + turboRechargeTimer);
		}
		
	}
	
	void FixedUpdate()
	{
		// Calculate the orientation and camera position
		computeOrientation();
		
		// Calculate the axis values
		calculateAxis();
		
		// Quit the app:
		if (Input.GetKeyDown(KeyCode.Escape)) {    
			Application.Quit();    
		}
		
		if(!isTurbo)
			cameraController.setFOV(originalFOV);
		else {
			cameraController.setFOV(turboFOV);	
		}
	
		
	}
	
	private void calculateAxis(){
		leftWheelXValue = 0;
		leftWheelYValue = 0; 
		rightWheelXValue = 0;
		rightWheelYValue = 0;
		
		// Different input systems related to the devices
		if(turboTimer <= 0) {
			switch (controlScheme){
			case ControlScheme.Keyboard:
				if (Input.GetKey("q"))
					leftWheelYValue += 1;
				if (Input.GetKey("a"))
					leftWheelYValue -= 1;
				if (Input.GetKey("e"))
					rightWheelYValue += 1;
				if (Input.GetKey("d"))
					rightWheelYValue -= 1;			
				//Buttons:
				jump(Input.GetKey("j")); 
				switchMagnetism(Input.GetKey("m"));
				switchTurbo(Input.GetKey("t"));
				
				break;
			case ControlScheme.RumblePad:
				//Axis:
				leftWheelXValue	= Input.GetAxis("Horizontal");
				leftWheelYValue	= Input.GetAxis("Vertical");
				rightWheelXValue	= Input.GetAxis("HorizontalRight");
				rightWheelYValue	= Input.GetAxis("VerticalRight");
				
				//Buttons:
				switchMagnetism(Input.GetButton("Fire4"));
				jump(Input.GetButton("Fire5"));
				switchTurbo(Input.GetButton("Fire6"));
				if (Input.GetButton("Fire9"))
					Application.LoadLevel(0);
				break;
			case ControlScheme.XBoxController:
				//Axis:
				leftWheelXValue	= Input.GetAxis("Horizontal");
				leftWheelYValue 	= Input.GetAxis("Vertical");
				rightWheelXValue = Input.GetAxis("HorizontalRightXBox");
				rightWheelYValue = Input.GetAxis("VerticalRightXBox");
				
				//Buttons:
				switchMagnetism(Input.GetButton("Fire4"));
				jump(Input.GetButton("Fire5"));
				switchTurbo(Input.GetButton("Fire6"));
				if (Input.GetButton("Fire7"))
					Application.LoadLevel(0);
				break;
			}
			
			if (leftWheelYValue == 0 && rightWheelYValue == 0){
				cameraController.Revert();
			}
			else if (leftWheelYValue * rightWheelYValue > 0){
				//The analog sticks are facing the same direction, apply the thresholds:
				float leftWheelAbs = Mathf.Abs(leftWheelYValue);
				float rightWheelAbs = Mathf.Abs(rightWheelYValue);
				if (leftWheelAbs > rightWheelAbs){
					rightWheelYValue = applyThresholds(leftWheelAbs, rightWheelAbs, rightWheelYValue);
				}
				else {
					leftWheelYValue = applyThresholds(rightWheelAbs, leftWheelAbs, leftWheelYValue);
				}
				
				//Set the camera to keep following the cylinder:
				cameraController.Revert();
			}
			else {
				
					float limit;
					
					if (rightWheelYValue > 0){
						limit = (rightWheelYValue - leftWheelYValue) * .4f;
						rightWheelYValue = ceil(rightWheelYValue, limit);
						leftWheelYValue = floor(leftWheelYValue, -limit);
					}
					else{
						limit = (leftWheelYValue - rightWheelYValue) * .4f;
						leftWheelYValue = ceil(leftWheelYValue, limit);
						rightWheelYValue = floor(rightWheelYValue, -limit);
					}
				if (leftWheelYValue == 0 || rightWheelYValue == 0) cameraController.setSpeed(4);
				else cameraController.Override(cameraController.getCurrentTarget());
				
				
				//Let the camera rest:
				
			}
		
			
		/*
		//Debug:
		Debug.Log("Left value: " + leftWheelYValue +
		          " Right value: " + rightWheelYValue); 
		*/
		
		
		// Assign velocity to the wheels
		if (cylinderMode == CylinderMode.Magnetic){
			l_cyl_w.throttleTo(leftWheelYValue * magnetThrottlePower);
			r_cyl_w.throttleTo(rightWheelYValue * magnetThrottlePower);
		}
		else if (cylinderMode == CylinderMode.Normal){
			l_cyl_w.throttleTo(leftWheelYValue * throttlePower);
			r_cyl_w.throttleTo(rightWheelYValue * throttlePower);
		}
	} else {
		l_cyl_w.throttleTo(turboThrottlePower);
		r_cyl_w.throttleTo(turboThrottlePower);
	}
		
	}
	
	//Function for applying the controller thresholds to the throttle variables:
	private float applyThresholds(float fastWheelAbs, float slowWheelAbs, float slowWheel){
		float threshold = fastWheelAbs * speedDiffThreshold;
		float uniformThr = threshold + ((fastWheelAbs - threshold) * uniformSteeringThreshold);
		
		if (slowWheelAbs > analogStickThreshold){
			float factor;
			factor = (slowWheelAbs - analogStickThreshold) / (1.0f - analogStickThreshold);
			slowWheelAbs = (threshold + (factor * (1 - threshold)));
			
			//uniform steering:
			if (slowWheelAbs > uniformThr) slowWheelAbs = fastWheelAbs;
			if (slowWheel > 0)
				return slowWheelAbs;	
			else
				return -slowWheelAbs;		
			
		}
		else{
			return (slowWheel / analogStickThreshold) * threshold;
		}
	}
	
	
	//Helper functions for limiting values:
	public static float ceil(float val, float maxVal){
		if (val > maxVal) return maxVal;
		else return val;
	}
	
	public static float floor(float val, float minVal){
		if (val < minVal) return minVal;
		else return val;
	}
	
	private void jump(bool keyDown){
		if (keyDown){
			if (!jumpButtonPressed){
				cylinderMode = CylController.CylinderMode.Normal;
				//l_cyl_w.setMaterial(standardMaterial);
				//r_cyl_w.setMaterial(standardMaterial);
				jumpButtonPressed = true;
				l_cyl_w.doJump();
				r_cyl_w.doJump();
			}
		}
		else{
			jumpButtonPressed = false;
		}
	}
	
	private void switchMagnetism(bool keyDown){
		if (keyDown){
			if (!cylinderModeChanged){
				cylinderModeChanged = true;
				if (cylinderMode == CylController.CylinderMode.Normal){
					cylinderMode = CylController.CylinderMode.Magnetic;
					leftWheel_Field.renderer.enabled = true;
					rightWheel_Field.renderer.enabled = true;
					//l_cyl_w.setMaterial(magnetMaterial);
					//r_cyl_w.setMaterial(magnetMaterial);
				}
				else{
					cylinderMode = CylController.CylinderMode.Normal;	
					leftWheel_Field.renderer.enabled = false;
					rightWheel_Field.renderer.enabled = false;
					//l_cyl_w.setMaterial(standardMaterial);
					//r_cyl_w.setMaterial(standardMaterial);
				}
			}
		}
		else{
			cylinderModeChanged = false;
		}
	}
	
	// Find out which way the cylinder is facing
	private void computeOrientation()
	{
		this.cylinderOrientation = rw_t.position - lw_t.position;
	}
	
	private void switchTurbo(bool keyDown) {
		if(keyDown) {
			if(turboRechargeDuration <= 0) {
				turboTimer = turboDuration;
			}
		}
	}
}

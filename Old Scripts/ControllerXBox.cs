using UnityEngine;
using System.Collections;

public class ControllerXBox : MonoBehaviour {
	public Rigidbody cylinder, LeftWheel, RightWheel;
	public int rotSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	void FixedUpdate() {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis("Horizontal") > 0) print ("H-positive");
		if (Input.GetAxis("Horizontal") < 0) print ("H-negative");
		
		if (Input.GetAxis("Vertical") > 0) {	//print ("V-positive");
			LeftWheel.AddRelativeTorque(0, rotSpeed, 0);
		}
		if (Input.GetAxis("Vertical") < 0) { //print ("V-negative");
			LeftWheel.AddRelativeTorque(0, -rotSpeed, 0);
		}
		
		if (Input.GetAxis("ThrottleLeft") > 0) print ("left-positive");
		if (Input.GetAxis("ThrottleLeft") < 0) print ("left-negative");
		if (Input.GetAxis("ThrottleRight") > 0) print ("right-positive");
		if (Input.GetAxis("ThrottleRight") < 0) print ("right-negative");
				
		if (Input.GetButtonDown("Fire1")) print("A-button");
		if (Input.GetButtonDown("Fire2")) print("B-button");
		if (Input.GetButtonDown("Fire3")) print("X-button");
		if (Input.GetButtonDown("Jump")) print("Y-button");
		
		
		
	}
}
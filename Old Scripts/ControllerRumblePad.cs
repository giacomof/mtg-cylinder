using UnityEngine;
using System.Collections;

public class ControllerRumblePad : MonoBehaviour {
	public Rigidbody cylinder, LeftWheel, RightWheel;
	public int jumpPower, rotSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void FixedUpdate() {
		if (Input.GetAxis("Vertical") > 0)		//print ("VL: " + Input.GetAxis("Vertical"));
			LeftWheel.AddRelativeTorque(0, rotSpeed, 0);
		if (Input.GetAxis("Vertical") < 0) 		//print ("VL: " + Input.GetAxis("Vertical"));
			LeftWheel.AddRelativeTorque(0, -rotSpeed, 0);
		if (Input.GetAxis("VerticalRight") > 0) //print ("VR: " + Input.GetAxis("VerticalRight"));
			RightWheel.AddRelativeTorque(0, rotSpeed, 0);
		if (Input.GetAxis("VerticalRight") < 0) //print ("VR: " + Input.GetAxis("VerticalRight"));
			RightWheel.AddRelativeTorque(0, -rotSpeed, 0);
		
		if (Input.GetButtonDown("Fire1")) print("Button 1");
		if (Input.GetButtonDown("Fire2")) print("Button 2");
		if (Input.GetButtonDown("Fire3")) print("Button 3");
		if (Input.GetButtonDown("Jump")) print("Button 4");
		if (Input.GetButtonDown("Fire4")) print("Button 5");
		if (Input.GetButtonDown("Fire5")) print("Button 6");
		if (Input.GetButtonDown("Fire6")) {	//print("Button 7");
			LeftWheel.AddRelativeTorque(jumpPower, 0, 0);
			RightWheel.AddRelativeTorque(jumpPower, 0, 0);
		}
		if (Input.GetButtonDown("Fire7")) 		//print("Button 8");
			cylinder.AddRelativeTorque(jumpPower, 0, 0);
		if (Input.GetButtonDown("Fire8")) print("Button 9");
		if (Input.GetButtonDown("Fire9"))	//print("Button 10");
			Application.LoadLevel(0);
		/*if (Input.GetButtonDown("Fire10")) print("Button 11");
		if (Input.GetButtonDown("Fire11")) print("Button 12");*/
		}
}

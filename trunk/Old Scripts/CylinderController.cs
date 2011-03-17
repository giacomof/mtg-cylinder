using UnityEngine;
using System.Collections;

public class CylinderController : MonoBehaviour {
	
	public Rigidbody cylinder, LeftWheel, RightWheel;
	public int rotSpeed=150;
	public float distanceToGround;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
		
		// Raycast for staying on the ground
		RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0F))
		{
			distanceToGround = hit.distance;
			print(distanceToGround);
		}
		
		if(distanceToGround > 1.5F)
			print("I'm flying");
			//transform.position.x = 1.5f;
	}
	
	void FixedUpdate (){
		
		if (Input.GetKey("q")){
			LeftWheel.AddRelativeTorque(0, rotSpeed, 0);
		}
		if (Input.GetKey("a")){
			LeftWheel.AddRelativeTorque(0, -rotSpeed, 0);
		}
		
		
		if (Input.GetKey("e")){
			RightWheel.AddRelativeTorque(0, rotSpeed, 0);
		}
		if (Input.GetKey("d")){
			RightWheel.AddRelativeTorque(0, -rotSpeed, 0);
		}

		
	}
}

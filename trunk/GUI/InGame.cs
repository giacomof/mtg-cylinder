using UnityEngine;
using System.Collections;

public class InGame : MonoBehaviour {
	public Rigidbody cylinder, LeftWheel, RightWheel;
	float maxCylinder = 0, maxLeft = 0, maxRight = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	
	void OnGUI () {
		GUIStyle guiStyle = new GUIStyle();
		//guiStyle.alignment = UpperRight;
		GUI.Box(new Rect(0, 0, 200, 100), "Current velocity / Max"+
			"\n\tCylinder: "+ CalculateVelocity(cylinder) + "\t / " 	+ Mathf.RoundToInt(maxCylinder)+
			"\n\tLeft: "		+ CalculateVelocity(LeftWheel) + "\t / "	+ Mathf.RoundToInt(maxLeft)+
			"\n\tRight: "	+ CalculateVelocity(RightWheel) + "\t / "	+ Mathf.RoundToInt(maxRight));
		GUI.Box(new Rect (Screen.width - 150,0,150,150), "Input"	+
			"\nControl Scheme: " +
			"\nVertical"		+
			"\n\tLeft: "		+ Mathf.RoundToInt(Input.GetAxis("Vertical") * 100) + " %" +
			"\n\tRight: "	+ Mathf.RoundToInt(Input.GetAxis("VerticalRight") * 100) + " %" +
			"\nHorizontal"	+ 
			"\n\tLeft: "		+ Mathf.RoundToInt(Input.GetAxis("Horizontal") * 100) + " %" +
			"\n\tRight: "	+ Mathf.RoundToInt(Input.GetAxis("HorizontalRight") * 100) + " %");		
		GUI.Box(new Rect (0,Screen.height - 50,100,50), "Bottom-left", guiStyle);
		GUI.Box(new Rect (Screen.width - 100,Screen.height - 50,100,50), "Bottom-right", guiStyle);
	}
	
	float CalculateHeight() {
		/*RaycastHit hit;
		if (Physics.Raycast(transform.position, -Vector3.up, out hit, 100.0F)) {
			distanceToGround = hit.distance;
			print (distanceToGround);
		}*/
		return 0;
	}
	
	//	Calculates |velocity| of a rigidbody, ie. cylinder or wheels
	int CalculateVelocity(Rigidbody rigid) {
		float temp = rigid.velocity.magnitude * 3.6F;
		switch (rigid.ToString()) {
			case "Cylinder (UnityEngine.Rigidbody)":
				if (temp > maxCylinder)
					maxCylinder = temp;
				break;
			case "LeftWheel (UnityEngine.Rigidbody)":
				if (temp > maxLeft)
					maxLeft = temp;
				break;
			case "RightWheel (UnityEngine.Rigidbody)":
				if (temp > maxRight)
					maxRight = temp;
				break;
			default:
				break;
		}
		/*if (temp > maxVelocity)
			maxVelocity = temp;*/
		return Mathf.RoundToInt(temp);
	}
}

using UnityEngine;
using System.Collections;

public class InGame : MonoBehaviour {
	public Rigidbody cylinder, LeftWheel, RightWheel;
	//public ControlScheme controlScheme;
	float maxCylinder = 0;

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
			"\n\tLeft: "		+ CalculateVelocity(LeftWheel) + 
			"\n\tRight: "	+ CalculateVelocity(RightWheel));
		GUI.Box(new Rect(Screen.width / 2 -50, 0, 100, 25), "CYLINDREAM");
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
	
	//	Calculates |velocity| of a rigidbody, ie. cylinder or wheels
	int CalculateVelocity(Rigidbody rigid) {
		float temp = rigid.velocity.magnitude * 3.6F;
		if (rigid == cylinder)
			if (temp > maxCylinder)
				maxCylinder = temp;
		return Mathf.RoundToInt(temp);
	}
}

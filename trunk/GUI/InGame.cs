using UnityEngine;
using System.Collections;

public class InGame : MonoBehaviour {
	public Rigidbody cylinder, LeftWheel, RightWheel;
	float maxCylinder = 0;
	
	//Test
	bool first = true;
	bool countdownStarted = false;
	float offset = 0;
	bool showCounter = false;
	float displayMinutes, displaySeconds, restSeconds, startTime, timeLeft;
	public Font cdFont;
	int countdownSeconds = 120, roundedRestSeconds;
	string timetext;
	//TODO: (JRH) Change to use GUI skin instead
	GUIStyle cdText = new GUIStyle(), guiStyle = new GUIStyle();		

	// Use this for initialization
	void Start () {
		//Setup for countdown clock
		cdText.font = cdFont;
		cdText.fontStyle = FontStyle.Bold;
		startTime = Time.deltaTime;
	}
	
	// Update is called once per frame
	//void Update () {
	void FixedUpdate() {
		if (Input.GetKeyDown("t"))
			showCounter = !showCounter;
	}
	
	void OnGUI () {
		//Setting count down
		/*if (first) {
			countdownSeconds = 120;
			first = false;
		}*/
		if (countdownStarted) {
			if (first) {
				first = false;
				offset = Time.time;
			}
			timeLeft = Time.time - startTime - offset;
		}
		restSeconds = countdownSeconds - (timeLeft);
		
		roundedRestSeconds = Mathf.CeilToInt(restSeconds);
		if (roundedRestSeconds < 0) {
			roundedRestSeconds = 0;
			//TODO: (JRH) EXPLODE!!!
		}
		displaySeconds = roundedRestSeconds % 60;
		displayMinutes = (roundedRestSeconds / 60) %60;
		timetext = (displayMinutes.ToString() + ":");
		if (displaySeconds > 9)
			timetext = timetext + displaySeconds.ToString();
		else
			timetext = timetext + "0" + displaySeconds.ToString();
		
		GUI.Box(new Rect(0, 0, 100, 25), "CYLINDREAM");
		if (showCounter)
			GUI.Box(new Rect(Screen.width / 2 -50, 0, 100, 50), timetext, cdText);
		GUI.Box(new Rect (Screen.width - 150,0,150,150), "Input"	+
			"\nControl Scheme: " +
			"\nVertical"		+
			"\n\tLeft: "		+ Mathf.RoundToInt(Input.GetAxis("Vertical") * 100) + " %" +
			"\n\tRight: "	+ Mathf.RoundToInt(Input.GetAxis("VerticalRight") * 100) + " %" +
			"\nHorizontal"	+ 
			"\n\tLeft: "		+ Mathf.RoundToInt(Input.GetAxis("Horizontal") * 100) + " %" +
			"\n\tRight: "	+ Mathf.RoundToInt(Input.GetAxis("HorizontalRight") * 100) + " %");		
		GUI.Box(new Rect (0,Screen.height - 100, 200, 100), "Current velocity / Max"+
			"\n\tCylinder: "+ CalculateVelocity(cylinder) + "\t / " 	+ Mathf.RoundToInt(maxCylinder)+
			"\n\tLeft: "		+ CalculateVelocity(LeftWheel) + 
			"\n\tRight: "	+ CalculateVelocity(RightWheel), guiStyle);
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
	
	public void ShowTimer(bool isShowing) {
		showCounter = isShowing;
	}
	
	public void StartCountdown(bool start) {
		countdownStarted = start;
	}	
}

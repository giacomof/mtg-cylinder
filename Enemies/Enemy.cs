using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	Transform player;
	bool onGround;
	float distance;
	float proximityDistance;
	
	//Testing
	//Vector3 collisonNormal;
	bool isDead = false;
	float enemySpeed = 8.0f;
	float killVelocity = 50.0f;

	void Awake() {
		//collisonNormal = Vector3.up;
		onGround = true;
		proximityDistance = 50.0f;
	}
	
	// Use this for initialization
	void Start () {
		if (!player)
			player = GameObject.FindWithTag("Player").transform;
	}
	
	// Update is called once per frame
	void Update () {	
		//Death check
		if (isDead)
			Destroy(gameObject);
		//Control check aka tumbling
		if (onGround) {
			//Get in right position
			//Orientate();
			//Rotate
			//Rotate();
			//Proximity check
			if (WithinDistance(player)) {
				//Movement
				Move();
			}
		}
	}
	
	void OnCollisionEnter(Collision collision) {
		switch (collision.gameObject.tag) {
			case "KillPlane":
				isDead = true;
				break;
			case "Player":
				Debug.Log(collision.relativeVelocity.magnitude);
				if (collision.relativeVelocity.magnitude > killVelocity)
					isDead = true;
				break;
			default:
				break;
		}
	}
	
	void OnCollisionStay(Collision collision) {
		switch (collision.gameObject.tag) {
			case "Ground":
				onGround = true;
				break;
			default:
				break;
		}
		//collisionNormal = collison.contacts[0].normal;
	}
	
	void OnCollisionExit(Collision collision) {
		switch (collision.gameObject.tag) {
			case "Ground":
				onGround = false;
				break;
			case "Player":
				break;
			default:
				break;
		}
	}
	
	void Move() {
		Vector3 delta = player.transform.position - transform.position;
		delta.Normalize();
		
		float moveSpeed = enemySpeed * Time.deltaTime;
		
		transform.position = transform.position + (delta * moveSpeed);
	}
	
	bool WithinDistance(Transform target) {
		distance = Vector3.Distance(target.position, transform.position);
		if (distance < proximityDistance) {
			this.renderer.material.color = Color.red + Color.yellow;
			return true;
		} else {
			this.renderer.material.color = Color.yellow;
			return false;
		}
	}
}

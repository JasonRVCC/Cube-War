using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Just a basic bit of code to launch a cube
 * (pulled some code from Mission Demolition)
 */

//Enum for the current play state
public enum PlayState{
	Idle, //The player is idle
	Aiming, //The player has selected a cude and is currently aiming
	Launch}; //The player has launched the cube, and the cube is in motion

public class Cube : MonoBehaviour {

	private PlayState playState; //The current play state
	private Vector3 velocity; //The cube's velocity
	//private Rigidbody cubeBody;

	//private Vector3 mousePos2D;
	//private Vector3 mousePos3D;
	private Vector3 launcherPos;
	private Vector3 cubePos;
	private float maxMagnitude;
	private GameObject tempLauncher;
	private GameObject ground;
	private RaycastHit hit;
	private Ray ray;
	private Quaternion cubeRot;

	public GameObject launcher;
	public float velocityMulti = 4f; //Multiplyer for velocity
	public float rotDecreaseRate = 0.2f;


	// Use this for initialization
	void Start () {
		playState = PlayState.Idle; 
		//cubePos = Camera.main.ScreenToWorldPoint (this.gameObject.transform.position);
		//cubePos = this.gameObject.GetComponent<Collider>().bounds.center;
		//mousePos2D = Input.mousePosition;
		//mousePos3D = Vector3.zero;
		//maxMagnitude = this.gameObject.GetComponent<Collider> ().bounds.size.x * 3f;
		velocity = Vector3.zero; //The player only controls X and Z velocity
		cubeRot = this.gameObject.transform.rotation;
		//cubeBody = this.gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
			print ("cur" + this.gameObject.transform.position);

		switch (playState) {
		case PlayState.Idle:
			break;
		case PlayState.Aiming:
			UpdateLaunchVelocity ();
			if (Input.GetMouseButtonDown(0)) {
				this.gameObject.GetComponent<Rigidbody> ().AddForce (-velocity * velocityMulti);
				playState = PlayState.Launch;
			}
			break;
		case PlayState.Launch:
			GameObject.Destroy (tempLauncher);
			print ("launched 1" + this.gameObject.GetComponent<Rigidbody> ().velocity.magnitude);
			if (cubeRot.eulerAngles.y > 0.0f) {
				cubeRot.eulerAngles -= cubeRot.eulerAngles * rotDecreaseRate;
				if (cubeRot.eulerAngles.y <= 0.0f) {
					cubeRot = Quaternion.Euler (0, 0, 0);
				}
			}
			this.gameObject.transform.rotation = cubeRot;
			if (this.gameObject.GetComponent<Rigidbody> ().velocity.magnitude <= 0.01) {
				this.gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;
				print ("launched 2" + this.gameObject.GetComponent<Rigidbody> ().velocity.magnitude);
				//cubePos = Camera.main.ScreenToWorldPoint (this.gameObject.transform.position);
				print ("velo" + this.gameObject.GetComponent<Rigidbody> ().velocity);
				this.gameObject.transform.rotation = Quaternion.Euler (0, 0, 0);
				velocity = Vector3.zero;
				cubePos = this.gameObject.transform.position;
				print ("pos at endLnch"  + cubePos + "......" + this.gameObject.transform.position);
				playState = PlayState.Idle;
			}
			break;
		}
	}



	void OnMouseDown(){
		if (playState == PlayState.Idle) {
			//TEMP
			//this.gameObject.GetComponent<Rigidbody> ().velocity = new Vector3(30f,4f,30f);
			playState = PlayState.Aiming;
		}
	}

	private void UpdateLaunchVelocity(){
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if ( Physics.Raycast (ray,out hit,1000.0f)) {
			launcherPos = new Vector3 (hit.point.x, this.gameObject.transform.position.y, 
				hit.point.z);
		}
		/*
		mousePos2D = Input.mousePosition;
		mousePos2D.z = Camera.main.transform.position.y;
		//mousePos2D.z = cubePos.y;
		//mousePos2D.z = 1f;
		mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D);
		*/
		GameObject.Destroy (tempLauncher);
		//mousePos3D.y = 0f;
		tempLauncher = (GameObject)Instantiate (launcher, launcherPos, Quaternion.Euler (0, 0, 0));
		cubeRot.eulerAngles += new Vector3(0,Mathf.Atan2(
			(this.gameObject.transform.position.z - launcherPos.z ),
			this.gameObject.transform.position.x) * Mathf.Rad2Deg,0);
		this.gameObject.transform.rotation = cubeRot;
		//cubePos = Camera.main.ScreenToWorldPoint (this.gameObject.transform.position);
		velocity = launcherPos - this.gameObject.transform.position;
		velocity.y = 0.4f;
		//do something about the y later
		//velocity.y = 0f; 
		//GameObject.Destroy (tempLauncher);
		//print ("" + mousePos3D + ", " + velocity);
		//since the camera has been rotated, we need to change the vector values
		/*float z = velocity.z;
		velocity.z = velocity.y;
		velocity.y = z;*/
	}





}

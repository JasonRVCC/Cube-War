using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Just a basic bit of code to launch a cube
 * (pulled some code from Mission Demolition)
 */

//Enum for the current play state

public enum PlayState{
	idle, //The player is idle
	aiming, //The player has selected a cude and is currently aiming
	launch,
	placing,
	placed}; //The player has launched the cube, and the cube is in motion

public class Cube : MonoBehaviour {

	public PlayState playState;  //The current play state
	private Vector3 velocity; //The cube's velocity
	//private Rigidbody cubeBody;

	//private Vector3 mousePos2D;
	//private Vector3 mousePos3D;
	private UnitClass unit;
	private Vector3 launcherPos;
	private float launcherY = 0.0f;
	private Vector3 cubePos;
	private GameObject templauncher;
	private GameObject tempHitMarker;
	private GameObject ground;
	private RaycastHit hit;
	private Ray ray;
	private Vector3 hitPos;
	private float maxHitPosY;
	private float minHitPosY;
	private float maxlauncherY;
	private float minlauncherY;
	private Collider[] colliders;
	//private GameObject lineRendererObject;
	//private LineRenderer lineRenderer;


	private bool flick;
	private bool stun;



	public GameObject launcher;
	public GameObject hitPosMarker;
	public float velocityMulti = 4f; //Multiplyer for velocity
	public float hitPosMod = 0.02f;
	public float maxMagnitude = 5.0f;

	// Use this for initialization
	void Start () {
		//playState = StateMachine.playState;  BRING BACK LATER
		unit = this.gameObject.GetComponent<UnitClass>();
		flick = false;
		stun = false; 
		//cubePos = Camera.main.ScreenToWorldPoint (this.gameObject.transform.position);
		cubePos = this.gameObject.transform.position;
		//mousePos2D = Input.mousePosition;
		//mousePos3D = Vector3.zero;
		//maxMagnitude = this.gameObject.GetComponent<Collider> ().bounds.size.x * 3f;
		velocity = Vector3.zero; //The player only controls X and Z velocity
		unit.startDefense ();
		//cubeBody = this.gameObject.GetComponent<Rigidbody>();
		//StateMachine.battlePhase(); //For turning batttle on/off
	}
	
	// Update is called once per frame
	void FixedUpdate () {
			
		//StateMachine.playState = playState; //TODO: have the state machine called later
		//TODO: change Kine different way


		switch (playState) {
		case PlayState.idle:
			break;
		case PlayState.aiming:
			if (Input.GetAxis ("Cancel") == 1) {
				isKinematic = false;
				//DestroyLaunchingObjects ();
				LaunchLine.launchLine.Enabled (false);
				playState = PlayState.idle;
				break;
			}
			//alter hit position
			if(Input.GetAxis("TargetUp") == 1){
				if (hitPos.y < maxHitPosY) {
					hitPos.y += hitPosMod;
					launcherY += hitPosMod;
					if (hitPos.y > maxHitPosY) {
						hitPos.y = maxHitPosY;
					}
					if (hitPos.y > maxlauncherY) {
						launcherY = maxlauncherY;
					}
					//tempHitMarker.transform.position = hitPos;
				}
			}
			if(Input.GetAxis("TargetDown") == 1){
				if (hitPos.y > minHitPosY) {
					hitPos.y -= hitPosMod;
					launcherY -= hitPosMod;
					if (hitPos.y < minHitPosY) {
						hitPos.y = minHitPosY;
					}
					if (hitPos.y < minlauncherY) {
						launcherY = minlauncherY;
					}
					//tempHitMarker.transform.position = hitPos;
				}
			}

			//alter launcher pos
			if(Input.GetAxis("ForceUp") == 1){
				if (launcherY < maxlauncherY) {
					launcherY += hitPosMod;
					if (hitPos.y > maxlauncherY) {
						launcherY = maxlauncherY;
					}
				}
			}
			if(Input.GetAxis("ForceDown") == 1){
				if (hitPos.y > minlauncherY) {
					launcherY -= hitPosMod;
					if (hitPos.y < minlauncherY) {
						launcherY = minlauncherY;
					}
				}
			}
			UpdatelaunchVelocity ();

			//launch 
			if (Input.GetMouseButtonDown(0)) {
				isKinematic = false;
				flick = true;
				unit.startAttack ();
				this.gameObject.GetComponent<Rigidbody> ().AddForceAtPosition (-velocity * 
					this.gameObject.GetComponent<Rigidbody>().mass * velocityMulti,hitPos);
				//DestroyLaunchingObjects ();
				LaunchLine.launchLine.Enabled(false);
				playState = PlayState.launch;
			}
			break;
		case PlayState.launch:

			if (this.gameObject.GetComponent<Rigidbody> ().IsSleeping()) {

				//this.gameObject.transform.rotation = Quaternion.Euler (0, 0, 0);
				velocity = Vector3.zero;
				//cubePos = this.gameObject.transform.position;
				hitPos = this.gameObject.transform.position;
				launcherY = this.gameObject.transform.position.y;
				unit.startDefense ();
				playState = PlayState.idle;
				GameDriver.checkCubeMovement();
			}
			break;
		case PlayState.placing:
			if (Input.GetAxis ("Cancel") == 1) {
				GameDriver.cancelPlaceCube ();
			}
			if (Input.GetAxis("TargetUp") == 1) {
				cubePos.y += hitPosMod;
			}
			if (Input.GetAxis("TargetDown") == 1) {
				cubePos.y -= hitPosMod;
			}
			UpdatePlacingPosition ();
		

			if (Input.GetMouseButtonDown(0)) {
				colliders = Physics.OverlapBox(cubePos,this.gameObject.GetComponent<Renderer>().bounds.extents,Quaternion.identity);
				if (colliders.Length > 1) {
					print ("You cannot place a cube there!");
				}
				else{
					isKinematic = false;
					this.gameObject.layer = LayerMask.NameToLayer ("cube");
					playState = PlayState.placed;
				}
			}
			break;
		case PlayState.placed:
			if (this.gameObject.GetComponent<Rigidbody> ().IsSleeping ()) {
				playState = PlayState.idle;
				GameDriver.placedCube ();
			}
			break;
		}
	}


	//TODO: Add messages for each condition.
	void OnMouseDown(){
		if ((playState == PlayState.idle) && (StateMachine.currentTurn() == unit.owner) && (flick==false) && (stun==false) && 
			(StateMachine.turnState != Turn.pause) && (StateMachine.getPhase() == GamePhase.battle)) {
			//TEMP
			//this.gameObject.GetComponent<Rigidbody> ().velocity = new Vector3(30f,4f,30f);
			hitPos = this.gameObject.transform.position;
			launcherY = this.gameObject.transform.position.y;
			maxHitPosY = hitPos.y + this.gameObject.GetComponent<Renderer> ().bounds.extents.y;
			minHitPosY = hitPos.y - this.gameObject.GetComponent<Renderer> ().bounds.extents.y;
			maxlauncherY = maxHitPosY + this.gameObject.GetComponent<Renderer> ().bounds.extents.y;
			minlauncherY = minHitPosY - this.gameObject.GetComponent<Renderer> ().bounds.extents.y;

			//TESTING
			//tempHitMarker = (GameObject)Instantiate(hitPosMarker,hitPos,Quaternion.identity);

			isKinematic = true;
			UpdatelaunchVelocity ();
			LaunchLine.launchLine.Enabled (true);
			playState = PlayState.aiming;

			//might remove this later
		}
	}


	public bool isKinematic{
		get{
			return this.gameObject.GetComponent<Rigidbody> ().isKinematic;
		} 
		set{ this.gameObject.GetComponent<Rigidbody> ().isKinematic = value;
		}
	}	

	public bool flicked{
		get{ return flick;}
		set{ flick = value; }
	}

	public bool stunned{
		get{ return stun;}
		set{ stun = value; }
	}

	private void UpdatelaunchVelocity(){
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if ( Physics.Raycast (ray,out hit,1000.0f)) {
			launcherPos = new Vector3 (hit.point.x, launcherY, 
				hit.point.z);
		}
		/*
		mousePos2D = Input.mousePosition;
		mousePos2D.z = Camera.main.transform.position.y;
		//mousePos2D.z = cubePos.y;
		//mousePos2D.z = 1f;
		mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D);
		*/
		//GameObject.Destroy (templauncher);
		//mousePos3D.y = 0f;
		//templauncher = (GameObject)Instantiate (launcher, launcherPos, Quaternion.Euler (0, 0, 0));
		//cubePos = Camera.main.ScreenToWorldPoint (this.gameObject.transform.position);
		velocity = launcherPos - hitPos;
		if (velocity.magnitude > maxMagnitude) {
			velocity = Vector3.ClampMagnitude(velocity, maxMagnitude);
			launcherPos = velocity + hitPos;
			//templauncher.transform.position = launcherPos;
		}
		LaunchLine.launchLine.SetLinePosition(0,hitPos);
		LaunchLine.launchLine.SetLinePosition(1,launcherPos);
		LaunchLine.launchLine.SetPointPosition (launcherPos);
		//LaunchLine.launchLine.endColor = Color.HSVToRGB ((1 - (velocity.magnitude / maxMagnitude)) * 120.0f, 1.0f, 1.0f);
		LaunchLine.launchLine.endColor = Color.Lerp (Color.green, Color.red, velocity.magnitude / maxMagnitude);
		//do something about the y later
		//velocity.y = 0f; 
		//GameObject.Destroy (templauncher);
		//print ("" + mousePos3D + ", " + velocity);
		//since the camera has been rotated, we need to change the vector values
		/*float z = velocity.z;
		velocity.z = velocity.y;
		velocity.y = z;*/
	}

	private void DestroyLaunchingObjects(){
		GameObject.Destroy (templauncher);
		GameObject.Destroy (tempHitMarker);
	}

	public void SetToPlacing(){
		isKinematic = true;
		playState = PlayState.placing;
		this.gameObject.layer = LayerMask.NameToLayer("placing");
	}

	private void UpdatePlacingPosition(){
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if ( Physics.Raycast (ray,out hit,1000.0f, (LayerMask)4096)) { //Only casts the ray against the RaycastGround layer.
			cubePos = new Vector3 (hit.point.x, cubePos.y, 
				hit.point.z);
		}

		this.gameObject.transform.position = cubePos;
	}





}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Just a basic bit of code to launch a cube
 * (pulled some code from Mission Demolition)
 */

//Enum for the current play state
/*
public enum CubeState{
	idle, //The player is idle
	aiming, //The player has selected a cude and is currently aiming
	launch}; //The player has launched the cube, and the cube is in motion
	*/
public enum PlayState{
	idle, //The player is idle
	aiming, //The player has selected a cude and is currently aiming
	launch,
	placing};

public class Cube : MonoBehaviour {

	private PlayState playState;  //The current play state
	private Vector3 velocity; //The cube's velocity
	//private Rigidbody cubeBody;

	//private Vector3 mousePos2D;
	//private Vector3 mousePos3D;
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

	public GameObject launcher;
	public GameObject hitPosMarker;
	public float velocityMulti = 4f; //Multiplyer for velocity
	public float hitPosMod = 0.02f;
	public float maxMagnitude = 5.0f;

	// Use this for initialization
	void Start () {
		playState = StateMachine.playState; 
		//cubePos = Camera.main.ScreenToWorldPoint (this.gameObject.transform.position);
		//cubePos = this.gameObject.GetComponent<Collider>().bounds.center;
		//mousePos2D = Input.mousePosition;
		//mousePos3D = Vector3.zero;
		//maxMagnitude = this.gameObject.GetComponent<Collider> ().bounds.size.x * 3f;
		velocity = Vector3.zero; //The player only controls X and Z velocity
		//cubeBody = this.gameObject.GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
			
		StateMachine.playState = playState;
		switch (playState) {
		case PlayState.idle:
			break;
		case PlayState.aiming:
			UpdatelaunchVelocity ();
			//alter hit position
			if(Input.GetKey(KeyCode.Z)){
				if (hitPos.y < maxHitPosY) {
					hitPos.y += hitPosMod;
					launcherY += hitPosMod;
					if (hitPos.y > maxHitPosY) {
						hitPos.y = maxHitPosY;
					}
					if (hitPos.y > maxlauncherY) {
						launcherY = maxlauncherY;
					}
					tempHitMarker.transform.position = hitPos;
				}
			}
			if(Input.GetKey(KeyCode.X)){
				if (hitPos.y > minHitPosY) {
					hitPos.y -= hitPosMod;
					launcherY -= hitPosMod;
					if (hitPos.y < minHitPosY) {
						hitPos.y = minHitPosY;
					}
					if (hitPos.y < minlauncherY) {
						launcherY = minlauncherY;
					}
					tempHitMarker.transform.position = hitPos;
				}
			}

			//alter launcher pos
			if(Input.GetKey(KeyCode.C)){
				if (launcherY < maxlauncherY) {
					launcherY += hitPosMod;
					if (hitPos.y > maxlauncherY) {
						launcherY = maxlauncherY;
					}
				}
			}
			if(Input.GetKey(KeyCode.V)){
				if (hitPos.y > minlauncherY) {
					launcherY -= hitPosMod;
					if (hitPos.y < minlauncherY) {
						launcherY = minlauncherY;
					}
				}
			}

			//launch 
			if (Input.GetMouseButtonDown(0)) {
				isKinematic = false;
				GameObject.Destroy (tempHitMarker);
				GameObject.Destroy(templauncher);
				this.gameObject.GetComponent<Rigidbody> ().AddForceAtPosition (-velocity * velocityMulti,hitPos);
				playState = PlayState.launch;
			}
			break;
		case PlayState.launch:
			
			if (this.gameObject.GetComponent<Rigidbody> ().velocity.magnitude <= 0.01) {
				this.gameObject.GetComponent<Rigidbody> ().velocity = Vector3.zero;

				this.gameObject.transform.rotation = Quaternion.Euler (0, 0, 0);
				velocity = Vector3.zero;
				//cubePos = this.gameObject.transform.position;
				hitPos = this.gameObject.transform.position;
				launcherY = this.gameObject.transform.position.y;
				playState = PlayState.idle;
			}
			break;
		}
	}



	void OnMouseDown(){
		if (playState == PlayState.idle) {
			//TEMP
			//this.gameObject.GetComponent<Rigidbody> ().velocity = new Vector3(30f,4f,30f);
			hitPos = this.gameObject.transform.position;
			launcherY = this.gameObject.transform.position.y;
			maxHitPosY = hitPos.y + this.gameObject.GetComponent<Renderer> ().bounds.extents.y;
			minHitPosY = hitPos.y - this.gameObject.GetComponent<Renderer> ().bounds.extents.y;
			maxlauncherY = maxHitPosY + this.gameObject.GetComponent<Renderer> ().bounds.extents.y;
			minlauncherY = minHitPosY - this.gameObject.GetComponent<Renderer> ().bounds.extents.y;

			//TESTING
			tempHitMarker = (GameObject)Instantiate(hitPosMarker,hitPos,Quaternion.identity);

			isKinematic = true;
			playState = PlayState.aiming;
			//StateMachine.playState = playState;

			//might remove this later
		}
	}


	public PlayState cubePlayState{
		get{ 
			return playState;
		}
		set{
			playState = value;
		}
	}

	public bool isKinematic{
		get{
			return this.gameObject.GetComponent<Rigidbody> ().isKinematic;
		} 
		set{ this.gameObject.GetComponent<Rigidbody> ().isKinematic = value;
		}
	}	

	private void UpdatelaunchVelocity(){
		//set launch position
		ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		if ( Physics.Raycast (ray,out hit,1000.0f)) {
			launcherPos = new Vector3 (hit.point.x, launcherY, 
				hit.point.z);
			//make sure the launcher is outside the cube
			/*
			if(this.gameObject.GetComponent<Renderer>().bounds.Contains(launcherPos)){
				
				if (launcherPos.x > this.gameObject.transform.position.x - this.gameObject.GetComponent<Renderer> ().bounds.extents.x &&
				    launcherPos.x < this.gameObject.transform.position.x) {
						launcherPos.x = this.gameObject.transform.position.x - this.gameObject.GetComponent<Renderer> ().bounds.extents.x;
				}
				else if (launcherPos.x < this.gameObject.transform.position.x + this.gameObject.GetComponent<Renderer> ().bounds.extents.x &&
					launcherPos.x > this.gameObject.transform.position.x) {
						launcherPos.x = this.gameObject.transform.position.x + this.gameObject.GetComponent<Renderer> ().bounds.extents.x;
				}

				if (launcherPos.z > this.gameObject.transform.position.z - this.gameObject.GetComponent<Renderer> ().bounds.extents.z &&
					launcherPos.z < this.gameObject.transform.position.z) {
						launcherPos.z = this.gameObject.transform.position.z - this.gameObject.GetComponent<Renderer> ().bounds.extents.z;
				}
				else if (launcherPos.z < this.gameObject.transform.position.z + this.gameObject.GetComponent<Renderer> ().bounds.extents.z &&
					launcherPos.z > this.gameObject.transform.position.z) {
					launcherPos.z = this.gameObject.transform.position.z + this.gameObject.GetComponent<Renderer> ().bounds.extents.z;
				}
			}
			*/
		}
		//
		//set hit position
		/*
		ray = new Ray (new Vector3 (launcherPos.x, hitPos.y, launcherPos.z), 
			new Vector3 (this.gameObject.transform.position.x, hitPos.y, this.gameObject.transform.position.z));
		if (Physics.Raycast (ray, out hit, 10000.0f, LayerMask.GetMask("cube"))) {
			hitPos = hit.point;
		}
		print (hitPos);
*/
		/*
		mousePos2D = Input.mousePosition;
		mousePos2D.z = Camera.main.transform.position.y;
		//mousePos2D.z = cubePos.y;
		//mousePos2D.z = 1f;
		mousePos3D = Camera.main.ScreenToWorldPoint (mousePos2D);
		*/
		GameObject.Destroy (templauncher);
		//mousePos3D.y = 0f;
		templauncher = (GameObject)Instantiate (launcher, launcherPos, Quaternion.Euler (0, 0, 0));
		//cubePos = Camera.main.ScreenToWorldPoint (this.gameObject.transform.position);
		velocity = launcherPos - hitPos;
		if (velocity.magnitude > maxMagnitude) {
			velocity = launcherPos.normalized * maxMagnitude;
			launcherPos = velocity + hitPos;
			templauncher.transform.position = launcherPos;
		}
		//do something about the y later
		//velocity.y = 0f; 
		//GameObject.Destroy (templauncher);
		//print ("" + mousePos3D + ", " + velocity);
		//since the camera has been rotated, we need to change the vector values
		/*float z = velocity.z;
		velocity.z = velocity.y;
		velocity.y = z;*/
	}





}

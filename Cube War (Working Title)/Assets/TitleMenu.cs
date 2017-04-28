using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class TitleMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	public void StartGame(){
		SceneManager.LoadScene ("Playable", LoadSceneMode.Single);
	}
}

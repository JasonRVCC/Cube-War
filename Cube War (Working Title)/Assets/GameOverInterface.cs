using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverInterface : MonoBehaviour {

	public void reset(){
		SceneManager.LoadScene (SceneManager.GetActiveScene ().buildIndex);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeKillBox : MonoBehaviour {
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag.Equals("Cube"))
        {
			if (collision.gameObject.GetComponent<Cube> ().playState == PlayState.launch && collision.gameObject.GetComponent<UnitClass>().unitClass != className.King) {
				GameDriver.checkCubeMovement();
			} 
            GameDriver.removeCubeFromPlay(collision.gameObject);
        }
    }
}

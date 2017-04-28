using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DicePhysics : MonoBehaviour {
    public Vector3 incomingForce;
    public bool activeDie;

    private void OnCollisionEnter(Collision collision)
    {
        if (!activeDie && collision.gameObject.CompareTag("Cube"))
        {


            float incDown = collision.gameObject.GetComponent<Rigidbody>().velocity.y;
            incomingForce = collision.impulse;
            incomingForce.x += -collision.gameObject.GetComponent<Rigidbody>().velocity.x;
            incomingForce.z += -collision.gameObject.GetComponent<Rigidbody>().velocity.z;
            incomingForce *= Mathf.Pow(2, collision.gameObject.GetComponent<Rigidbody>().mass);
            incomingForce.x *= Mathf.Max(1, -incDown);
            incomingForce.z *= Mathf.Max(1, -incDown);
            incomingForce.y /= 50;
            GetComponent<Rigidbody>().AddForce(-incomingForce * GetComponent<Rigidbody>().mass * 50);


            //print(name + " - Hit, adding force of: " + incomingForce * GetComponent<Rigidbody>().mass + " from: " + GetComponent<Rigidbody>().mass);
        }
        else if (activeDie && collision.gameObject.CompareTag("Cube"))
        {
            float incDown = collision.gameObject.GetComponent<Rigidbody>().velocity.y;
            incomingForce = collision.impulse;
            incomingForce.x += -GetComponent<Rigidbody>().velocity.x;
            incomingForce.z += -GetComponent<Rigidbody>().velocity.z;
            incomingForce.x *= Mathf.Max(1, -incDown);
            incomingForce.z *= Mathf.Max(1, -incDown);
            incomingForce.y /= 50;
            GetComponent<Rigidbody>().AddForce(incomingForce * GetComponent<Rigidbody>().mass *50);

            print(GetComponent<Rigidbody>().mass);
            print(name + " - Hit, adding force of: " + incomingForce * Mathf.Pow(2, GetComponent<Rigidbody>().mass) * 100);
        }
    }
}

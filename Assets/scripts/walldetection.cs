using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walldetection : MonoBehaviour {
    public float front, left, right, frontRange,rightRange,leftRange;
    
	
	// Update is called once per frame
	void Update () {

        //raycast straight forwards
        RaycastHit2D hitFront = Physics2D.Raycast(this.gameObject.transform.position, this.gameObject.transform.up,frontRange);
        if (hitFront.collider != null)//if it hit something
        {
            
            //Debug.Log("FRONT");
            Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.up*frontRange, Color.red); //draw it red
            front = hitFront.distance/frontRange; //change front to 0-1 depending on how far it is away
            
        }
        else //if it aint hit nothing
        {
            Debug.DrawRay(this.gameObject.transform.position, this.gameObject.transform.up * frontRange, Color.green); //draw it green
            front = 1; //1 is far away
         }

        //same but for 45 degree right/left
        Vector2 rightVector = (this.gameObject.transform.up + this.gameObject.transform.right).normalized;
        RaycastHit2D hitRight= Physics2D.Raycast(this.gameObject.transform.position, rightVector, rightRange);
        if (hitRight.collider != null)
        {
            //Debug.Log("FRONT");
            Debug.DrawRay(this.gameObject.transform.position, rightVector * rightRange, Color.red);
            right = hitRight.distance / rightRange;
        }
        else
        {
            Debug.DrawRay(this.gameObject.transform.position, rightVector * rightRange, Color.green);
            right = 1;
        }

        Vector2 leftVector = (this.gameObject.transform.up - this.gameObject.transform.right).normalized;
        RaycastHit2D hitLeft = Physics2D.Raycast(this.gameObject.transform.position, leftVector, leftRange);
        if (hitLeft.collider != null)
        {
            //Debug.Log("FRONT");
            Debug.DrawRay(this.gameObject.transform.position, leftVector * leftRange, Color.red);
            left = hitLeft.distance /leftRange;
        }
        else
        {
            Debug.DrawRay(this.gameObject.transform.position, leftVector * leftRange, Color.green);
            left = 1;
        }


    }
    
}

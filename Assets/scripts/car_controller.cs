using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class car_controller : MonoBehaviour {
    

    public float score = 0f;


    private bool initilized = false;
    float accelForce = 10f;//how fast you go
    float turnForce = 10f;//how fast you turn
    Vector2 lastPosition,startPosition;

    private brain net;
    // Use this for initialization
    void Start() {
        lastPosition = transform.position;//for fitness function
    }

    //happens before update()
    void FixedUpdate() {


        if (initilized == true)
        {
            GetComponent<Rigidbody2D>().velocity = ForwardVelocity();

            float[] inputs = new float[4];//number of inputs (front, right, left and speed)
            inputs[0] = GetComponent<walldetection>().front;//the raycast from walldirection
            if (inputs[0] < 0.1f)
            {
                //Debug.Log(inputs[0]);testing code
            }
            inputs[1] = GetComponent<walldetection>().right;//right
            inputs[2] = GetComponent<walldetection>().left;//left
            inputs[3] = ForwardVelocity().magnitude;//speed

            /*LEGACY TESTING, use for manual driving
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            {
                Forward();
            }
            if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                Reverse();
            }
            if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            {
                Right();
            }
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            {
                Left();
            }
            
            score += ForwardVelocity().magnitude;
            */
            float[] output = net.FeedForward(inputs);//put the array of inputs into the array. Feedforward is where all the work is done, the rest of the shit is for creating/copying it.
            //Debug.Log(output[0]);
            //these can be in any order
            Forward(output[0]);
            Reverse(output[1]);
            Right(output[2]);

            int trackPos =  getTrackPart();//the asshole of a fitness function... tried doing distance travelled at first but the cars just kept doing burnouts on the starting line
            //the fitness function divides the track into 4 parts, the first 2 work and thats good enough for me. top part go left, right part go down and etc.
            //change to winding number once track editor is in place
            float fitness = 0;
            switch (trackPos)
            {
                case 1: fitness = transform.position.x-lastPosition.x;
                    break;
                case 2:
                    fitness = -(transform.position.y - lastPosition.y);
                    break;
                    
                case 3:
                    fitness = -(transform.position.x - lastPosition.x);
                    break;
                case 4:
                    fitness = transform.position.y - lastPosition.y;
                    break;
            }
           

            net.AddFitness(fitness);//add the fitness function to the brain
            score += fitness;
            lastPosition = transform.position;//reset last position
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //If it hits a wall stop
        accelForce = 0f;
        turnForce = 0f;
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        //Debug.Log("FAIL");
    }
    void Forward(float f)
    {
        //Debug.Log("FORWARD");
        if (f > 0f)
        {
            GetComponent<Rigidbody2D>().AddForce(transform.up * accelForce * f);
        }
    }
    void Reverse(float f)
    {
        if (f >0) {
            GetComponent<Rigidbody2D>().AddForce(-transform.up * accelForce * f);

        }
    }

    void Right(float f)
    {
        //Debug.Log("right");
        //originally was 2 functions for left and right at a specific rate. car kept going straight by using both at same time so changed to be negative goes left positive goes right
        GetComponent<Rigidbody2D>().transform.Rotate(0f, 0f, turnForce*f);
    }

    Vector2 ForwardVelocity()
    {
        return transform.up * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.up);
        //makes it so the car goes where it points, not just up
    }
    Vector2 RightVelocity()
    {
        //not used, can be used for drifting in another build
        //TODO: Dejavu simulator
        return transform.right * Vector2.Dot(GetComponent<Rigidbody2D>().velocity, transform.right);
    }

    public void Init(brain net)
    {
        this.net = net;
        //pointer to its brain in the manager script.
        initilized = true;
    }
    
    private int getTrackPart()
    {
        //fitness function splitting the track
        //change to winding number later
        if (transform.position.y >= 2.5)
        {
            return 1;
        }
        else if (transform.position.x > 15.25)
        {
            return 2;
        }
        else if (transform.position.y < -1 && transform.position.x >-16)
        {
            return 3;
        }
        else return 4;
    }
}

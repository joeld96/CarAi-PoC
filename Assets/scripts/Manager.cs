using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour {
   
    public Sprite[] red;
    public float genTime;
    public GameObject carPrefab;
    public Text genText, best, timeT;



    private bool isTraining = false;
    private int populationSize = 200; //number of cars
    private int generationNumber = 0; //generation number
    private int[] layers = new int[] { 4, 10, 10, 3 }; //4 input, 2 hidden layers, 3 output, set number of neurons here
    private List<brain> nets;//
    private List<car_controller> carList = null;
    
    void Timer()
    {
        isTraining = false;
    }         
	
	// Update is called once per frame
	void Update () {

        //restart function
        if (isTraining == false)
        {
            if (generationNumber == 0)
            {
                InitCarNeuralNetworks(); //start list of neural networks
            }
            else
            {
                nets.Sort();//uses the compare function to sort the list of neural networks
                nets.Reverse();//reverses list so the best scoring is at the start
               
                best.text ="Best score: " + nets[0].GetFitness();
                Debug.Log(nets[0].GetFitness());//for best ftness

                for (int i = 1; i < populationSize/4; i++)
                {
                    nets[i] = new brain(nets[0]);//first 25% excluding the best are mutated
                    nets[i].Mutate();

                }
                
                for (int i = populationSize/4; i < populationSize*0.9f; i++)
                {
                    nets[i] = new brain(nets[0]);//next 65% are mutated twice
                    nets[i].Mutate();
                    nets[i].Mutate();
                }
                for (int i = (int)(populationSize*0.9f); i < populationSize; i++)
                {
                    nets[i] = new brain(layers);//last 10% are completely random
                    nets[i].Mutate();

                }
                
                for (int i = 0; i < populationSize; i++)
                {
                    nets[i].SetFitness(0f);//reset list of nets to 0 fitness
                }
                
            }

            generationNumber++;
            genText.text = "Generation: " + generationNumber;
            isTraining = true;
            Invoke("Timer", genTime);//timer for generations
            CreateCars();
        }
	}
    private void CreateCars()
    {
        if (carList != null)
        {
            for (int i = 0; i < carList.Count; i++)
            {
                
                GameObject.Destroy(carList[i].gameObject);//destroy all current cars
                
            }
        }

        carList = new List<car_controller>();//recreate list as empty

        for (int i = 0; i < populationSize; i++)
        {
            //create new car
            car_controller car = ((GameObject)Instantiate(carPrefab, this.gameObject.transform.position, carPrefab.transform.rotation)).GetComponent<car_controller>();
            if (i == 0)
            {
                car.GetComponent<SpriteRenderer>().sprite = red[0];
                car.transform.position = car.transform.position - Vector3.forward;//best car is blue
            }
            else if (i < populationSize / 4)
            {
                car.GetComponent<SpriteRenderer>().sprite = red[1];//slightly mutant is this colour
            }
            else if (i < populationSize * 0.9f)
            {
                car.GetComponent<SpriteRenderer>().sprite = red[2];//double mutate is this sprite
                
            }
            else{
                car.GetComponent<SpriteRenderer>().sprite = red[3];//new brains is this sprite
            }

            car.Init(nets[i]);//initialize the car
            carList.Add(car);//add car to list
        }
    }

    private void InitCarNeuralNetworks()
    {
        
        nets = new List<brain>();//initialize list of brains

        for (int i = 0; i < populationSize; i++)
        {
            brain net = new brain(layers);//create new list
            net.Mutate();//mutate the generated brain
            nets.Add(net);//add it to the list
        }
    }

    //stuff for changing timer
    public void SetTime(float time)
    {
        genTime = time;
        timeT.text = "Generation time: " + time;
    }
}

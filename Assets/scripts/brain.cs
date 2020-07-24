using System;
using System.Collections.Generic;
using UnityEngine;

public class brain : IComparable<brain>{
    
    //The brain of the cars
    //Deep learning 
    private int[] layers;//array of how many neurons in each layer i.e {4,10,10,3}
    private float[][] neurons;
    private float[][][] weights;
    private float fitness;//fitness of how well the car is driving

    //Deep copy of the brain between generations
    public brain(brain copyBrain)
    {
        this.layers = new int[copyBrain.layers.Length];
        for (int i = 0; i < copyBrain.layers.Length; i++)
        {
            this.layers[i] = copyBrain.layers[i];
        }
        InitNeurons(); // all cars have the same neuron setup, no augmenting topology

        InitWeights();

        CopyWeights(copyBrain.weights);
    }
    //the first gen and pink cars (completely random) use this
    public  brain(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        InitNeurons();
        InitWeights();

        
    }

    private void CopyWeights(float[][][] copyWeights) 
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = copyWeights[i][j][k] ;
                }
            }
        }
    }

    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();//a list of float arrays

        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]); //adds a float array of size 4 then 10 then 10 then 3
        }

        neurons = neuronsList.ToArray();//changes the list of float arrays into a 2d array
    }

    //function to create the initial weights of the neural connections
    //randomly assigns a number between -0.5 and 0.5 to the connection
    private void InitWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();

        for (int i = 1; i <layers.Length; i++)//starts at 1, very important due to input layer
        {
            List<float[]> layerWeightList = new List<float[]>();

            int neuronsInPreviousLayer = layers[i - 1];

            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];

                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    neuronWeights[k] =UnityEngine.Random.Range(-0.5f,0.5f);//random number for the neuron weight
                }

                layerWeightList.Add(neuronWeights);//adds the array to the layerweightlist
            }

            weightsList.Add(layerWeightList.ToArray());//adds the list of arrays to the weightlist after converting it to a 2d array
        }

        weights = weightsList.ToArray();//converts the list of 2d arrays into a 3d array
    }


    public float[] FeedForward(float[] inputs)
    {
        for(int i =0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];// converts the first column of neurons into the inputted data (speed, distance to walls) Can add more later
        }

        for (int i = 1; i < layers.Length; i++)//skip the input neurons
        {
            for (int j = 0; j < neurons[i].Length; j++)//for every neuron in neurons
            {

                float value = 0.25f;//bias so that the value doesnt lock at 0
                for (int k = 0; k < neurons[i-1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];//multiplying the weights by the previous neurons that it is connected to
                }

                neurons[i][j] = (float)Math.Tanh(value);//changes value of hyperbolic tangent activation function, so that small values are bigger https://theclevermachine.wordpress.com/tag/tanh-function/
            }
        }

        return neurons[neurons.Length - 1] ;//return output neurons
    }

    //mutate the brain with the best fitness onto the next generation
    //red cars mutated once, green cares mutated twice
    public void Mutate()
    {
        
        for (int i = 0; i < weights.Length; i++)//go through every neuron and weight
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];//pointer

                    float randomNumber = UnityEngine.Random.Range(0,1000f);//random number between 0-1000
                    //small numbers are needed here due to the large number of neurons, only small changes will be needed
                    if(randomNumber <= 2f)
                    {
                        weight *= -1f;//0.2% chance of reversing the sign
                    }
                    else if(randomNumber<=4f)
                    {
                        weight = UnityEngine.Random.Range(-0.5f, 0.5f);//0.2% chance of changing it to a random number between -0.5 - 0.5
                    }
                    else if (randomNumber <= 6f)
                    {
                        float factor = UnityEngine.Random.Range(0f, 1f) + 1f; //increases the weight by 100-200%
                        weight *= factor;
                    }
                    else if (randomNumber <= 8f)
                    {
                        float factor = UnityEngine.Random.Range(0f, 1f);//decreases the weight by 0-100%
                        weight *= factor;
                    }


                    weights[i][j][k] = weight;//assign weight
                }
            }
        }
        
    }

    //fitness stuff
    public void SetFitness(float fit)
    {
        fitness = fit;
    }
    public float GetFitness()
    {
        return fitness;
    }
    public void AddFitness(float fit)
    {
        fitness += fit;
    }

    //compare function to determine biggest brain
    public int CompareTo(brain other)
    {
        if( other == null) return 1;

        if (fitness > other.fitness)
            return 1;
        else if (fitness < other.fitness)
            return -1;
        else return 0;
    }
}

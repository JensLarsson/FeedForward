using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrainTest : MonoBehaviour
{
    float[,] trainingData =  {
        {0,0,0 },
        {1,1,0 },
        {0,1,1 },
        {1,0,1 },

        };




    NeuralNetwork neuralNetwork;
    // Start is called before the first frame update
    void Awake()
    {
        neuralNetwork = new NeuralNetwork(2, 2, 1);
        neuralNetwork.SetLeanringRate(0.2f);
        neuralNetwork.SetMomentum(true, 0.9f);
        TrainBrain();
    }

    void TrainBrain()
    {
        float error = 1;
        int saftey = 5000000;
        Debug.Log(trainingData.GetLength(0));
        neuralNetwork.PrintData();
        while (error > 0.05f && saftey > 0)
        {
            error = 0;
            saftey--;
            for (int i = 0; i < trainingData.GetLength(0); i++)
            {
                neuralNetwork.SetInput(0, trainingData[i, 0]);
                neuralNetwork.SetInput(1, trainingData[i, 1]);

                neuralNetwork.SetDesiredOutput(0, trainingData[i, 2]);

                neuralNetwork.FeedForward();
                error += neuralNetwork.CalculateError();
                neuralNetwork.BackPropagate();
            }

            error = error / trainingData.GetLength(0);
        }
        Debug.Log(saftey);
        Debug.Log(error);
        Debug.Log("-");
        Debug.Log("-");
        Debug.Log("-");

        neuralNetwork.PrintData();
    }
}

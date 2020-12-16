using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestScript : MonoBehaviour
{


    TrainingData[] trainingData =
    {
        new TrainingData{
            input=new float[] {0,0 },
            targetResult = new float[]{0}
        },
        new TrainingData{
            input=new float[] {1,1 },
            targetResult = new float[]{0}
        },
        new TrainingData{
            input=new float[] {0.25f,0.25f },
            targetResult = new float[]{0}
        },
        new TrainingData{
            input=new float[] {0.5f,0.5f },
            targetResult = new float[]{0}
        },
        new TrainingData{
            input=new float[] {0.75f,0.75f },
            targetResult = new float[]{0}
        },
        new TrainingData{
            input=new float[] {1,0 },
            targetResult = new float[]{1}
        },
        new TrainingData{
            input=new float[] {0,1 },
            targetResult = new float[]{1}
        },
    };

    public TrainedNetworkSO trainedNetworkA;
    public TrainedNetworkSO trainedNetworkB;

    NeuralNetwork_Matrix neuralNetwork;
    const int SIZE = 100;
    float[,] array = new float[SIZE, SIZE];

    // Start is called before the first frame update
    void Start()
    {
        neuralNetwork = new NeuralNetwork_Matrix(2, 4, 1);
        Debug.Log(trainingData.Length);
        for (int i = 0; i < 100_000; i++)
        {
            int random = Random.Range(0, trainingData.Length);
            neuralNetwork.Train(trainingData[random].input, trainingData[random].targetResult);
        }
        trainedNetworkA.SetNetworkVariables(neuralNetwork);

        neuralNetwork = new NeuralNetwork_Matrix(trainedNetworkA);

        trainedNetworkB.SetNetworkVariables(neuralNetwork);


    }
}

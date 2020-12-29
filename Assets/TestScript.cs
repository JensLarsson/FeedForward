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

        if (trainedNetworkA.networkSet)
        {
            neuralNetwork = new NeuralNetwork_Matrix(trainedNetworkA);
        }
        else
        {
            neuralNetwork = new NeuralNetwork_Matrix(2, 4, 1);

            neuralNetwork.TrainNeuralNetwork(trainingData, 100_000, true);


            trainedNetworkA.SetNetworkVariables(neuralNetwork);

            neuralNetwork = new NeuralNetwork_Matrix(trainedNetworkA);

            trainedNetworkB.SetNetworkVariables(neuralNetwork);
        }
        TestNeuralNetwork(10_000);




    }


    public void TestNeuralNetwork(int itterations)
    {
        int failsTotal = 0;
        for (int i = 0; i < itterations; i++)
        {
            int random = UnityEngine.Random.Range(0, trainingData.Length);
            float[] result = neuralNetwork.Predict(trainingData[random].input);
            if (!IsResultCorrect(result, trainingData[random].targetResult))
            {
                failsTotal++;
            }
        }
        Debug.Log($"{failsTotal} fails:");
        Debug.Log((1 - (float)failsTotal / (float)(itterations)) * 100 + " % accurate");
    }

    bool IsResultCorrect(float[] guess, float[] answer)
    {
        int guessIndex = 0;
        int answerIndex = 0;
        for (int i = 1; i < guess.Length; i++)
        {
            if (guess[i] > guess[guessIndex])
            {
                guessIndex = i;
            }
            if (answer[i] > answer[guessIndex])
            {
                answerIndex = i;
            }
        }
        return guessIndex == answerIndex;
    }

}

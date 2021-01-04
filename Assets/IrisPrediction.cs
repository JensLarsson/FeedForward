using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IrisPrediction : MonoBehaviour
{



    // Start is called before the first frame update
    void Start()
    {
        TrainingData[] trainingData = new TrainingData[dataSet.Length];
        for (int i = 0; i < trainingData.Length; i++)
        {
            trainingData[i] = new TrainingData
            {
                input = new float[] {
                    dataSet[i].sepal_length,
                    dataSet[i].sepal_width,
                    dataSet[i].petal_length,
                    dataSet[i].sepal_width
                },
                targetResult = dataSet[i].species,
            };
        }

        //Normalize(trainingData);

        NeuralNetwork_Matrix neuralNetwork = new NeuralNetwork_Matrix(
                trainingData[0].input.Length, 7, trainingData[0].targetResult.Length);

        neuralNetwork.TrainNeuralNetwork(trainingData, 10_000, true);

        int itterations = 100;
        int fails = 0;
        for (int i = 0; i < itterations; i++)
        {
            int random = Random.Range(0, trainingData.Length);
            float[] result = neuralNetwork.Guess(trainingData[random].input);

            Debug.Log($"{result[0]} {result[1]} {result[2]} |||| {trainingData[random].targetResult[0]} {trainingData[random].targetResult[1]} {trainingData[random].targetResult[2]}");

            if (!IsResultCorrect(result, trainingData[random].targetResult))
            {
                fails++;
            }

        }
        Debug.Log((1 - (float)fails / (float)itterations) * 100 + " % accurate");
    }

    bool IsResultCorrect(float[] guess, float[] answer)
    {
        int guessIndex = 0;
        int answerIndex = 0;
        for (int i = 1; i < guess.Length; i++)
        {
            if (guess[guessIndex] < guess[i])
            {
                guessIndex = i;
            }
            if (answer[answerIndex] < answer[i])
            {
                answerIndex = i;
            }
        }
        return guessIndex == answerIndex;
    }

    void Normalize(TrainingData[] dataset)
    {
        Debug.Log(dataset.Length);
        for (int colum = 0; colum < dataset[0].input.Length; colum++)
        {
            float sum = 0;
            for (int i = 0; i < dataset.Length; i++)
            {
                sum += dataset[i].input[colum];
            }
            float average = sum / (float)dataset.Length;
            sum = 0;
            for (int i = 0; i < dataset.Length; i++)
            {
                sum += (dataset[i].input[colum] - average) * (dataset[i].input[colum] - average);
            }
            float sd = Mathf.Sqrt(sum / (float)dataset.Length - 1);
            if (float.IsNaN(sd))
            {
                Debug.Log(sum);

            }
            for (int i = 0; i < dataset.Length; i++)
            {
                dataset[i].input[colum] = (dataset[i].input[colum] - average) / sd;
            }
        }
    }















    struct Iris
    {
        public float sepal_length, sepal_width, petal_length, petal_width;
        public float[] species;

    }

    Iris[] dataSet = {
    new Iris(){ sepal_length=5.1f, sepal_width=3.5f, petal_length=1.4f, petal_width=0.2f, species=new float[] {1f,0f,0f } },
    new Iris(){ sepal_length=4.9f,sepal_width=3f,petal_length=1.4f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.7f,sepal_width=3.2f,petal_length=1.3f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.6f,sepal_width=3.1f,petal_length=1.5f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5f,sepal_width=3.6f,petal_length=1.4f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.4f,sepal_width=3.9f,petal_length=1.7f,petal_width=0.4f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.6f,sepal_width=3.4f,petal_length=1.4f,petal_width=0.3f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5f,sepal_width=3.4f,petal_length=1.5f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.4f,sepal_width=2.9f,petal_length=1.4f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.9f,sepal_width=3.1f,petal_length=1.5f,petal_width=0.1f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.4f,sepal_width=3.7f,petal_length=1.5f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.8f,sepal_width=3.4f,petal_length=1.6f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.8f,sepal_width=3f,petal_length=1.4f,petal_width=0.1f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.3f,sepal_width=3f,petal_length=1.1f,petal_width=0.1f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.8f,sepal_width=4f,petal_length=1.2f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.7f,sepal_width=4.4f,petal_length=1.5f,petal_width=0.4f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.4f,sepal_width=3.9f,petal_length=1.3f,petal_width=0.4f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.1f,sepal_width=3.5f,petal_length=1.4f,petal_width=0.3f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.7f,sepal_width=3.8f,petal_length=1.7f,petal_width=0.3f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.1f,sepal_width=3.8f,petal_length=1.5f,petal_width=0.3f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.4f,sepal_width=3.4f,petal_length=1.7f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.1f,sepal_width=3.7f,petal_length=1.5f,petal_width=0.4f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.6f,sepal_width=3.6f,petal_length=1f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.1f,sepal_width=3.3f,petal_length=1.7f,petal_width=0.5f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.8f,sepal_width=3.4f,petal_length=1.9f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5f,sepal_width=3f,petal_length=1.6f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5f,sepal_width=3.4f,petal_length=1.6f,petal_width=0.4f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.2f,sepal_width=3.5f,petal_length=1.5f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.2f,sepal_width=3.4f,petal_length=1.4f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.7f,sepal_width=3.2f,petal_length=1.6f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.8f,sepal_width=3.1f,petal_length=1.6f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.4f,sepal_width=3.4f,petal_length=1.5f,petal_width=0.4f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.2f,sepal_width=4.1f,petal_length=1.5f,petal_width=0.1f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.5f,sepal_width=4.2f,petal_length=1.4f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.9f,sepal_width=3.1f,petal_length=1.5f,petal_width=0.1f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5f,sepal_width=3.2f,petal_length=1.2f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.5f,sepal_width=3.5f,petal_length=1.3f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.9f,sepal_width=3.1f,petal_length=1.5f,petal_width=0.1f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.4f,sepal_width=3f,petal_length=1.3f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.1f,sepal_width=3.4f,petal_length=1.5f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5f,sepal_width=3.5f,petal_length=1.3f,petal_width=0.3f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.5f,sepal_width=2.3f,petal_length=1.3f,petal_width=0.3f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.4f,sepal_width=3.2f,petal_length=1.3f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5f,sepal_width=3.5f,petal_length=1.6f,petal_width=0.6f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.1f,sepal_width=3.8f,petal_length=1.9f,petal_width=0.4f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.8f,sepal_width=3f,petal_length=1.4f,petal_width=0.3f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.1f,sepal_width=3.8f,petal_length=1.6f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=4.6f,sepal_width=3.2f,petal_length=1.4f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5.3f,sepal_width=3.7f,petal_length=1.5f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=5f,sepal_width=3.3f,petal_length=1.4f,petal_width=0.2f,species=new float[] {1f,0f,0f }},
    new Iris(){ sepal_length=7f,sepal_width=3.2f,petal_length=4.7f,petal_width=1.4f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.4f,sepal_width=3.2f,petal_length=4.5f,petal_width=1.5f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.9f,sepal_width=3.1f,petal_length=4.9f,petal_width=1.5f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.5f,sepal_width=2.3f,petal_length=4f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.5f,sepal_width=2.8f,petal_length=4.6f,petal_width=1.5f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.7f,sepal_width=2.8f,petal_length=4.5f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.3f,sepal_width=3.3f,petal_length=4.7f,petal_width=1.6f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=4.9f,sepal_width=2.4f,petal_length=3.3f,petal_width=1f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.6f,sepal_width=2.9f,petal_length=4.6f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.2f,sepal_width=2.7f,petal_length=3.9f,petal_width=1.4f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5f,sepal_width=2f,petal_length=3.5f,petal_width=1f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.9f,sepal_width=3f,petal_length=4.2f,petal_width=1.5f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6f,sepal_width=2.2f,petal_length=4f,petal_width=1f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.1f,sepal_width=2.9f,petal_length=4.7f,petal_width=1.4f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.6f,sepal_width=2.9f,petal_length=3.6f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.7f,sepal_width=3.1f,petal_length=4.4f,petal_width=1.4f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.6f,sepal_width=3f,petal_length=4.5f,petal_width=1.5f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.8f,sepal_width=2.7f,petal_length=4.1f,petal_width=1f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.2f,sepal_width=2.2f,petal_length=4.5f,petal_width=1.5f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.6f,sepal_width=2.5f,petal_length=3.9f,petal_width=1.1f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.9f,sepal_width=3.2f,petal_length=4.8f,petal_width=1.8f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.1f,sepal_width=2.8f,petal_length=4f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.3f,sepal_width=2.5f,petal_length=4.9f,petal_width=1.5f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.1f,sepal_width=2.8f,petal_length=4.7f,petal_width=1.2f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.4f,sepal_width=2.9f,petal_length=4.3f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.6f,sepal_width=3f,petal_length=4.4f,petal_width=1.4f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.8f,sepal_width=2.8f,petal_length=4.8f,petal_width=1.4f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.7f,sepal_width=3f,petal_length=5f,petal_width=1.7f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6f,sepal_width=2.9f,petal_length=4.5f,petal_width=1.5f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.7f,sepal_width=2.6f,petal_length=3.5f,petal_width=1f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.5f,sepal_width=2.4f,petal_length=3.8f,petal_width=1.1f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.5f,sepal_width=2.4f,petal_length=3.7f,petal_width=1f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.8f,sepal_width=2.7f,petal_length=3.9f,petal_width=1.2f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6f,sepal_width=2.7f,petal_length=5.1f,petal_width=1.6f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.4f,sepal_width=3f,petal_length=4.5f,petal_width=1.5f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6f,sepal_width=3.4f,petal_length=4.5f,petal_width=1.6f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.7f,sepal_width=3.1f,petal_length=4.7f,petal_width=1.5f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.3f,sepal_width=2.3f,petal_length=4.4f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.6f,sepal_width=3f,petal_length=4.1f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.5f,sepal_width=2.5f,petal_length=4f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.5f,sepal_width=2.6f,petal_length=4.4f,petal_width=1.2f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.1f,sepal_width=3f,petal_length=4.6f,petal_width=1.4f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.8f,sepal_width=2.6f,petal_length=4f,petal_width=1.2f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5f,sepal_width=2.3f,petal_length=3.3f,petal_width=1f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.6f,sepal_width=2.7f,petal_length=4.2f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.7f,sepal_width=3f,petal_length=4.2f,petal_width=1.2f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.7f,sepal_width=2.9f,petal_length=4.2f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.2f,sepal_width=2.9f,petal_length=4.3f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.1f,sepal_width=2.5f,petal_length=3f,petal_width=1.1f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=5.7f,sepal_width=2.8f,petal_length=4.1f,petal_width=1.3f,species=new float[] {0f,1f,0f }},
    new Iris(){ sepal_length=6.3f,sepal_width=3.3f,petal_length=6f,petal_width=2.5f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=5.8f,sepal_width=2.7f,petal_length=5.1f,petal_width=1.9f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=7.1f,sepal_width=3f,petal_length=5.9f,petal_width=2.1f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.3f,sepal_width=2.9f,petal_length=5.6f,petal_width=1.8f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.5f,sepal_width=3f,petal_length=5.8f,petal_width=2.2f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=7.6f,sepal_width=3f,petal_length=6.6f,petal_width=2.1f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=4.9f,sepal_width=2.5f,petal_length=4.5f,petal_width=1.7f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=7.3f,sepal_width=2.9f,petal_length=6.3f,petal_width=1.8f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.7f,sepal_width=2.5f,petal_length=5.8f,petal_width=1.8f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=7.2f,sepal_width=3.6f,petal_length=6.1f,petal_width=2.5f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.5f,sepal_width=3.2f,petal_length=5.1f,petal_width=2f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.4f,sepal_width=2.7f,petal_length=5.3f,petal_width=1.9f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.8f,sepal_width=3f,petal_length=5.5f,petal_width=2.1f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=5.7f,sepal_width=2.5f,petal_length=5f,petal_width=2f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=5.8f,sepal_width=2.8f,petal_length=5.1f,petal_width=2.4f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.4f,sepal_width=3.2f,petal_length=5.3f,petal_width=2.3f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.5f,sepal_width=3f,petal_length=5.5f,petal_width=1.8f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=7.7f,sepal_width=3.8f,petal_length=6.7f,petal_width=2.2f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=7.7f,sepal_width=2.6f,petal_length=6.9f,petal_width=2.3f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6f,sepal_width=2.2f,petal_length=5f,petal_width=1.5f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.9f,sepal_width=3.2f,petal_length=5.7f,petal_width=2.3f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=5.6f,sepal_width=2.8f,petal_length=4.9f,petal_width=2f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=7.7f,sepal_width=2.8f,petal_length=6.7f,petal_width=2f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.3f,sepal_width=2.7f,petal_length=4.9f,petal_width=1.8f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.7f,sepal_width=3.3f,petal_length=5.7f,petal_width=2.1f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=7.2f,sepal_width=3.2f,petal_length=6f,petal_width=1.8f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.2f,sepal_width=2.8f,petal_length=4.8f,petal_width=1.8f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.1f,sepal_width=3f,petal_length=4.9f,petal_width=1.8f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.4f,sepal_width=2.8f,petal_length=5.6f,petal_width=2.1f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=7.2f,sepal_width=3f,petal_length=5.8f,petal_width=1.6f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=7.4f,sepal_width=2.8f,petal_length=6.1f,petal_width=1.9f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=7.9f,sepal_width=3.8f,petal_length=6.4f,petal_width=2f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.4f,sepal_width=2.8f,petal_length=5.6f,petal_width=2.2f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.3f,sepal_width=2.8f,petal_length=5.1f,petal_width=1.5f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.1f,sepal_width=2.6f,petal_length=5.6f,petal_width=1.4f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=7.7f,sepal_width=3f,petal_length=6.1f,petal_width=2.3f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.3f,sepal_width=3.4f,petal_length=5.6f,petal_width=2.4f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.4f,sepal_width=3.1f,petal_length=5.5f,petal_width=1.8f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6f,sepal_width=3f,petal_length=4.8f,petal_width=1.8f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.9f,sepal_width=3.1f,petal_length=5.4f,petal_width=2.1f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.7f,sepal_width=3.1f,petal_length=5.6f,petal_width=2.4f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.9f,sepal_width=3.1f,petal_length=5.1f,petal_width=2.3f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=5.8f,sepal_width=2.7f,petal_length=5.1f,petal_width=1.9f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.8f,sepal_width=3.2f,petal_length=5.9f,petal_width=2.3f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.7f,sepal_width=3.3f,petal_length=5.7f,petal_width=2.5f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.7f,sepal_width=3f,petal_length=5.2f,petal_width=2.3f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.3f,sepal_width=2.5f,petal_length=5f,petal_width=1.9f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.5f,sepal_width=3f,petal_length=5.2f,petal_width=2f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=6.2f,sepal_width=3.4f,petal_length=5.4f,petal_width=2.3f,species=new float[] {0f,0f,1f }},
    new Iris(){ sepal_length=5.9f,sepal_width=3f,petal_length=5.1f,petal_width=1.8f,species=new float[] {0f,0f,1f }}
};


}

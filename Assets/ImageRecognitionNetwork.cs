﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

//ScriptableObjects keep loosing their values, consider replacing
public class ImageRecognitionNetwork : MonoBehaviour
{
    const int NOISE_TRAINING_AMOUNT = 6000;

    public bool testNetwork = true;
    [SerializeField] Image image;
    [SerializeField] TMP_Text guessText;
    [SerializeField] TMP_Text certaintyText;

    [SerializeField] ImageArray trainingImages;
    [SerializeField] ImageArray testImages;

    [SerializeField] TrainedNetworkSO trainedNetwork;

    NeuralNetwork_Matrix neuralNetwork;
    TrainingData[] trainingData;
    // Start is called before the first frame update
    void Start()
    {
        guessText.text = "A";
        if (trainedNetwork == null || (trainedNetwork != null && !trainedNetwork.networkSet))
        {
            trainingImages.LoadBytesFromPath();
            guessText.text = "B";
            //Create training data array
            trainingData = new TrainingData[trainingImages.imageCount + NOISE_TRAINING_AMOUNT];
            uint pixelCount = trainingImages.pixelWidth * trainingImages.pixelHeight;

            guessText.text = "C";
            for (int i = 0; i < trainingImages.imageCount; i++)
            {
                trainingData[i].input = new float[pixelCount];  //input nodes
                trainingData[i].targetResult = new float[11];   //target nodes

                //Set data
                trainingData[i].targetResult[trainingImages.labelValueArray[i]] = 1f;
                for (int j = 0; j < pixelCount; j++)
                {
                    trainingData[i].input[j] = trainingImages.pixelValueArray[i * trainingImages.pixelValueArrayOffset + j];
                }
            }

            for (int i = 0; i < NOISE_TRAINING_AMOUNT; i++)
            {
                trainingData[trainingImages.imageCount + i].input = new float[pixelCount];  //input nodes
                trainingData[trainingImages.imageCount + i].targetResult = new float[11];   //target nodes
                trainingData[trainingImages.imageCount + i].targetResult[10] = 1f;

                for (int j = 0; j < pixelCount; j++)
                {
                    trainingData[trainingImages.imageCount + i].input[j] = Random.Range(0f, 1f);
                }
            }



            guessText.text = "D";
            // (inputNodeCount, hiddenNodeCount, outputNodeCount)
            neuralNetwork = new NeuralNetwork_Matrix(
                   trainingData[0].input.Length,
                  (trainingData[0].input.Length + trainingData[0].targetResult.Length) / 2,
                   trainingData[0].targetResult.Length);


            guessText.text = "E";
            neuralNetwork.TrainNeuralNetwork(trainingData, 70000, true);


            guessText.text = "F";
            if (trainedNetwork != null)
            {
                trainedNetwork.SetNetworkVariables(neuralNetwork);
            }

            guessText.text = "G";
        }
        else
        {
            neuralNetwork = new NeuralNetwork_Matrix(trainedNetwork);
        }
        guessText.text = "H";
#if UNITY_EDITOR
        if (testImages.pixelValueArray.Length == 0)
        {
            testImages.LoadBytesFromPath();
        }
#endif
        if (testNetwork)
        {
            TestNeuralNetwork();
        }
        guessText.text = "I";
        TestRandomImage();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestRandomImage();
        }
    }
    public void TestNeuralNetwork()
    {
        int[] fails = new int[10];
        int failsTotal = 0;
        for (int i = 0; i < testImages.imageCount; i++)
        {
            float[] input = new float[testImages.pixelValueArrayOffset];
            for (int j = 0; j < testImages.pixelValueArrayOffset; j++)
            {
                input[j] =
                    testImages.pixelValueArray[i * testImages.pixelValueArrayOffset + j];
            }
            float[] result = neuralNetwork.Guess(input);
            if (!IsResultCorrect(result, testImages.labelValueArray[i]))
            {
                fails[testImages.labelValueArray[i]]++;
                failsTotal++;
            }
        }
        Debug.Log($"{failsTotal} fails:");
        Debug.Log((1 - (float)failsTotal / (float)(testImages.imageCount)) * 100 + " % accurate");
    }
    public void TestRandomImage()
    {
        float certainty = 0;
        int random = Random.Range(0, (int)testImages.imageCount);
        float[] input = new float[testImages.pixelValueArrayOffset];
        for (int j = 0; j < testImages.pixelValueArrayOffset; j++)
        {
            input[j] = testImages.pixelValueArray[random * testImages.pixelValueArrayOffset + j];
        }
        float[] result = neuralNetwork.Guess(input);
        image.sprite = Sprite.Create(testImages.GetImage(random), new Rect(0, 0, 28, 28), Vector2.zero);
        guessText.text = GetGuessedValue(result, ref certainty).ToString();
        certaintyText.text = (certainty * 100f).ToString("F2") + '%';
    }
    public void TestImage(float[] input)
    {
        float certainty = 0;
        float[] result = neuralNetwork.Guess(input);
        int guessedValue = GetGuessedValue(result, ref certainty);
        guessText.text = guessedValue == 10 ? ((char)8).ToString() : guessedValue.ToString();
        certaintyText.text = (certainty * 100f).ToString("F2") + '%';
    }

    int GetGuessedValue(float[] values)
    {
        int guess = 0;
        for (int i = 1; i < values.Length; i++)
        {
            if (values[i] > values[guess])
            {
                guess = i;
            }
        }
        return guess;
    }
    int GetGuessedValue(float[] values, ref float certainty)
    {
        int guess = 0;
        for (int i = 1; i < values.Length; i++)
        {
            if (values[i] > values[guess])
            {
                guess = i;
            }
        }
        certainty = values[guess];
        return guess;
    }

    bool IsResultCorrect(float[] guess, byte answer)
    {
        int guessIndex = 0;
        for (int i = 1; i < guess.Length; i++)
        {
            if (guess[i] > guess[guessIndex])
            {
                guessIndex = i;
            }

        }
        return guessIndex == answer;
    }

}
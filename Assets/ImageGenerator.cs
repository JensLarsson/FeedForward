
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class ImageGenerator : MonoBehaviour
{
    public bool testNetwork = true;
    [SerializeField] Image image;
    [SerializeField] TMP_Text GeneratedNumber;
    [SerializeField] Slider slider;

    [SerializeField] ImageArray trainingImages;
    [SerializeField] ImageArray testImages;

    [SerializeField] TrainedNetworkSO trainedNetwork;

    NeuralNetwork_Matrix neuralNetwork;
    TrainingData[] trainingData;
    // Start is called before the first frame update
    void Start()
    {
        if (trainedNetwork == null || (trainedNetwork != null && !trainedNetwork.networkSet))
        {
            //load data
            trainingImages.LoadBytesFromPath();

            //Create training data array
            trainingData = new TrainingData[trainingImages.imageCount];
            uint pixelCount = trainingImages.pixelWidth * trainingImages.pixelHeight;

            for (int i = 0; i < trainingImages.imageCount; i++)
            {
                trainingData[i].input = new float[11];                  //input nodes
                trainingData[i].targetResult = new float[pixelCount];   //target nodes
                trainingData[i].input[10] = RandomGaussian();           //Noise

                //Set data
                trainingData[i].input[trainingImages.LabelValueArray[i]] = 1f;
                for (int j = 0; j < pixelCount; j++)
                {
                    trainingData[i].targetResult[j] = trainingImages.PixelValueArray[i][j];
                }
            }

            // (inputNodeCount, hiddenNodeCount, outputNodeCount)
            neuralNetwork = new NeuralNetwork_Matrix(
                   trainingData[0].input.Length,
                  (trainingData[0].input.Length + trainingData[0].targetResult.Length) / 2,
                   trainingData[0].targetResult.Length);


            neuralNetwork.TrainNeuralNetwork(trainingData);


            if (trainedNetwork != null)
            {
                trainedNetwork.SetNetworkVariables(neuralNetwork);
            }
        }
        else
        {
            neuralNetwork = new NeuralNetwork_Matrix(trainedNetwork);
        }
        Texture2D tempTex = new Texture2D(28, 28);
        image.sprite = Sprite.Create(tempTex, new Rect(0, 0, 28, 28), Vector2.zero);
        CreateImange();
    }

    KeyCode[] keyCodes = {
         KeyCode.Alpha0,
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };

    private void Update()
    {
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                targetNumber = i;
                CreateImange();
            }
        }
    }

    int targetNumber = 0;
    public void CreateImange()
    {
        GeneratedNumber.text = targetNumber.ToString("F3");

        float[] input = new float[11];
        input[targetNumber] = 1;
        input[10] = slider.value;
        float[] pixelValues = neuralNetwork.Predict(input);
        for (int i = 0; i < 28; i++)
        {
            for (int j = 0; j < 28; j++)
            {

                image.sprite.texture.SetPixel(i, j, new Color(pixelValues[i + j * 28], 0, 0));
            }
        }
        image.sprite.texture.Apply();
    }

    public void TestNeuralNetwork()
    {
        int[] fails = new int[10];
        int failsTotal = 0;
        for (int i = 0; i < testImages.imageCount; i++)
        {
            float[] result = neuralNetwork.Predict(testImages.PixelValueArray[i]);
            if (!IsResultCorrect(result, testImages.LabelValueArray[i]))
            {
                fails[testImages.LabelValueArray[i]]++;
                failsTotal++;
            }
        }
        Debug.Log($"{failsTotal} fails:");
        Debug.Log((1 - (float)failsTotal / (float)(testImages.imageCount)) * 100 + " % accurate");
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

    public float RandomGaussian(float minValue = 0.0f, float maxValue = 1.0f)
    {
        float u, v, S;

        do
        {
            u = 2.0f * UnityEngine.Random.value - 1.0f;
            v = 2.0f * UnityEngine.Random.value - 1.0f;
            S = u * u + v * v;
        }
        while (S >= 1.0f);

        // Standard Normal Distribution
        float std = u * Mathf.Sqrt(-2.0f * Mathf.Log(S) / S);

        // Normal Distribution centered between the min and max value
        // and clamped following the "three-sigma rule"
        float mean = (minValue + maxValue) / 2.0f;
        float sigma = (maxValue - mean) / 3.0f;
        return Mathf.Clamp(std * sigma + mean, minValue, maxValue);
    }

}
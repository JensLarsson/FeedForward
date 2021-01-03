
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
                trainingData[i].input[trainingImages.labelValueArray[i]] = 1f;
                for (int j = 0; j < pixelCount; j++)
                {
                    trainingData[i].targetResult[j] = trainingImages.pixelValueArray[i * trainingImages.pixelValueArrayOffset + j];
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
        GeneratedNumber.text = targetNumber.ToString();

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

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
                trainingData[i].input = new float[10];  //input nodes
                trainingData[i].targetResult = new float[pixelCount];   //target nodes

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
        //testImages.LoadBytesFromPath();
        //if (testNetwork)
        //{
        //    TestNeuralNetwork();
        //}
        CreateImange(0);
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
                CreateImange(i);
            }
        }
    }

    void CreateImange(int number)
    {
        if (number < 0 || number > 9)
        {
            return;
        }
        else
        {
            float[] input = new float[10];
            input[number] = 1;
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

}
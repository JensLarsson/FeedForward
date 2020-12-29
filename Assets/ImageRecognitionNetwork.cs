using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class ImageRecognitionNetwork : MonoBehaviour
{
    [SerializeField] Image image;
    [SerializeField] TMP_Text textMeshPro;

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

            for (int i = 0; i < trainingImages.imageCount ; i++)
            {
                trainingData[i].input = new float[pixelCount];  //input nodes
                trainingData[i].targetResult = new float[10];   //target nodes

                //Set data
                trainingData[i].targetResult[trainingImages.LabelValueArray[i]] = 1f;
                for (int j = 0; j < pixelCount; j++)
                {
                    trainingData[i].input[j] = trainingImages.PixelValueArray[i][j];
                }
            }

            // (inputNodeCount, hiddenNodeCount, outputNodeCount)
            neuralNetwork = new NeuralNetwork_Matrix(
                   trainingData[0].input.Length,
                  (trainingData[0].input.Length + trainingData[0].targetResult.Length) / 2,
                   trainingData[0].targetResult.Length);

            neuralNetwork.TrainNeuralNetwork(trainingData, trainingData.Length / 1000);

            if (trainedNetwork != null)
            {
                trainedNetwork.SetNetworkVariables(neuralNetwork);
            }
        }
        else
        {
            neuralNetwork = new NeuralNetwork_Matrix(trainedNetwork);
        }

        testImages.LoadBytesFromPath();
        TestNeuralNetwork();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestRandomImage();
        }
    }




    //public void TrainNeuralNetwork()
    //{
    //    for (int i = 0; i < trainingImages.imageCount/1000; i++)
    //    {
    //        neuralNetwork.Train(trainingData[i].input, trainingData[i].targetResult);
    //    }
    //}

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
    public void TestRandomImage()
    {
        int random = Random.Range(0, (int)testImages.imageCount);
        float[] result = neuralNetwork.Predict(testImages.PixelValueArray[random]);
        image.sprite = Sprite.Create(testImages.GetImage(random), new Rect(0, 0, 28, 28), Vector2.zero);
        textMeshPro.text = GetGuessedValue(result).ToString();
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
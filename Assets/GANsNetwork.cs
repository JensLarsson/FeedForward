using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class GANsNetwork : MonoBehaviour
{
    public bool testNetwork = true;
    [SerializeField] Image image;
    [SerializeField] TMP_Text guessText;
    [SerializeField] TMP_Text certaintyText;

    [SerializeField] ImageArray trainingImages;
    [SerializeField] ImageArray testImages;

    [SerializeField] TrainedNetworkSO trainedNetwork;

    NeuralNetwork_Matrix generator;
    NeuralNetwork_Matrix discriminator;
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
                trainingData[i].input = new float[pixelCount];  //input nodes
                trainingData[i].targetResult = new float[11];   //target nodes

                //Set data
                trainingData[i].targetResult[trainingImages.LabelValueArray[i]] = 1f;
                trainingData[i].targetResult[10] = 1f;
                for (int j = 0; j < pixelCount; j++)
                {
                    trainingData[i].input[j] = trainingImages.PixelValueArray[i][j];
                }

            }

            // (inputNodeCount, hiddenNodeCount, outputNodeCount)
            discriminator = new NeuralNetwork_Matrix(
                   trainingData[0].input.Length,
                  (trainingData[0].input.Length + trainingData[0].targetResult.Length) / 2,
                   trainingData[0].targetResult.Length);

            generator = new NeuralNetwork_Matrix(11, (11 + 28 * 28) / 2, 28 * 28);


            //if (trainedNetwork != null)
            //{
            //    trainedNetwork.SetNetworkVariables(discriminator);
            //}
        }
        //else
        //{
        //    discriminator = new NeuralNetwork_Matrix(trainedNetwork);
        //}

        testImages.LoadBytesFromPath();
        TrainNetwork(1);
    }

    void TrainNetwork(int itterations)
    {
        for (int i = 0; i < itterations; i++)
        {
            int random = Random.Range(0, trainingData.Length);
            float[] generatorInput =
            {
                 trainingData[random].targetResult[0],
                 trainingData[random].targetResult[1],
                 trainingData[random].targetResult[2],
                 trainingData[random].targetResult[3],
                 trainingData[random].targetResult[4],
                 trainingData[random].targetResult[5],
                 trainingData[random].targetResult[6],
                 trainingData[random].targetResult[7],
                 trainingData[random].targetResult[8],
                 trainingData[random].targetResult[9],
                 RandomGaussian()
        };
            float[] generatedImage = generator.Predict(generatorInput);
            float[] realOutput = discriminator.Predict(trainingData[random].input);
            float[] fakeOutput = discriminator.Predict(generatedImage);

            float discriminatorLoss = CalculateLoss(realOutput);
            float generatorLoss = CalculateLoss(fakeOutput);


        }
    }
    float CalculateLoss(float[] input)
    {
        float sum = 0;
        for (int i = 0; i < input.Length; i++)
        {
            sum += Mathf.Log(input[i], 2);
        }
        return -sum;
    }

    //float CrossEntropy(float[] p, float[] q)
    //{
    //    float sum = 0;
    //    for (int i = 0; i < p.Length; i++)
    //    {
    //        sum += p[i] * Mathf.Log(q[i], 2);
    //    }
    //    return -sum;
    //}
    //float CrossEntropyAllOnes(float[] q)
    //{
    //    float sum = 0;
    //    for (int i = 0; i < p.Length; i++)
    //    {
    //        sum += 1 * Mathf.Log(q[i], 2);
    //    }
    //    return -sum;
    //}

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
            float[] result = discriminator.Predict(testImages.PixelValueArray[i]);
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
        float certainty = 0;
        int random = Random.Range(0, (int)testImages.imageCount);
        float[] result = discriminator.Predict(testImages.PixelValueArray[random]);
        image.sprite = Sprite.Create(testImages.GetImage(random), new Rect(0, 0, 28, 28), Vector2.zero);
        guessText.text = GetGuessedValue(result, ref certainty).ToString();
        certaintyText.text = (certainty * 100f).ToString("F2") + '%';
    }
    public void TestImage(float[] input)
    {
        float certainty = 0;
        float[] result = discriminator.Predict(input);
        guessText.text = GetGuessedValue(result, ref certainty).ToString();
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
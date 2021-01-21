using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Barracuda;
using UnityEngine.UI;
using TMPro;

public class BarracudaTest : MonoBehaviour
{
    public NNModel model;

    Model runtimeModel;
    IWorker worker;


    public bool testNetwork = true;

    [SerializeField] Image image;
    [SerializeField] TMP_Text guessText;
    [SerializeField] TMP_Text certaintyText;

    [SerializeField] ImageArray testImages;
    TrainingData[] trainingData;

    void Start()
    {
        runtimeModel = ModelLoader.Load(model);
        worker = WorkerFactory.CreateWorker(runtimeModel);

#if UNITY_EDITOR
        if (testImages.pixelValueArray.Length == 0)
        {
            testImages.LoadBytesFromPath();
        }
        if (testNetwork)
        {
            int fails = 0;
            Tensor input = new Tensor(1, 0, 28 * 28, 1);
            for (int i = 0; i < testImages.imageCount; i++)
            {
                for (int j = 0; j < testImages.pixelValueArrayOffset; j++)
                {
                    input[0, 0, j, 0] =
                        testImages.pixelValueArray[i * testImages.pixelValueArrayOffset + j];

                }
                Tensor output = worker.Execute(input).PeekOutput("Y");
                int value = GetResult(output);
                bool correctGuess = value == testImages.labelValueArray[i];
                fails += correctGuess ? 0 : 1;
            }
            Debug.Log(fails + " fails of " + testImages.imageCount + " images");
            Debug.Log((1 - ((float)fails / testImages.imageCount)) * 100 + "% accuracy");
        }
#endif
        TestRandomImage(true);
    }

    int GetResult(Tensor tensor)
    {
        int best = 0;
        for (int i = 0; i < tensor.length; i++)
        {
            if (tensor[best] < tensor[i])
            {
                best = i;
            }
        }
        return best;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TestRandomImage(true);
        }
    }

    public void TestImage(float[] inputArray)
    {
        Tensor input = new Tensor(1, 0, 28 * 28, 1);
        for (int j = 0; j < inputArray.Length; j++)
        {
            input[0, 0, j, 0] =
                inputArray[j];
        }
        Tensor output = worker.Execute(input).PeekOutput("Y");
        int value = GetResult(output);
        guessText.text = value.ToString();
    }

    bool TestRandomImage(bool generateImage = false)
    {
        int random = Random.Range(0, (int)testImages.imageCount);
        Tensor input = new Tensor(1, 0, 28 * 28, 1);
        for (int j = 0; j < testImages.pixelValueArrayOffset; j++)
        {
            input[0, 0, j, 0] =
                testImages.pixelValueArray[random * testImages.pixelValueArrayOffset + j];

            //if (j % 28 == 0)
            //{
            //    s += '\n';
            //}
            //s += testImages.pixelValueArray[random * testImages.pixelValueArrayOffset + j] < 0.01 ? '_' : '#' ;

        }
        if (generateImage)
        {
            image.sprite = Sprite.Create(testImages.GetImage(random), new Rect(0, 0, 28, 28), Vector2.zero);
        }
        Tensor output = worker.Execute(input).PeekOutput("Y");
        int value = GetResult(output);
        guessText.text = value.ToString();
        return value == testImages.labelValueArray[random];
    }
}
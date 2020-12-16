using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "learnedNetwork")]
[System.Serializable]
public class TrainedNetworkSO : ScriptableObject
{
    int[] nodeCounts;
    public FlatMatrix weightsInputToHidden;
    public FlatMatrix weightsHiddenToOutput;
    public float[] biasHidden;
    public float[] biasOutput;
    float learningRate;
    ActivationFunction activationFunc;

    public bool networkSet = false;

    public void SetNetworkVariables(NeuralNetwork_Matrix neuralNetwork)
    {
        nodeCounts = new int[]
        {
            neuralNetwork.getInputNodeCount,
            neuralNetwork.getHiddenNodeCount,
            neuralNetwork.getOutputNodeCount,
        };

        weightsInputToHidden = new FlatMatrix(neuralNetwork.getWeightsInputToHidden);
        weightsHiddenToOutput = new FlatMatrix(neuralNetwork.getWeightsHiddenToOutput);
        biasHidden = neuralNetwork.getBiasHidden;
        biasOutput = neuralNetwork.getBiasOutput;

        learningRate = neuralNetwork.getLearningRate;
        activationFunc = neuralNetwork.getActivationFunction;

        networkSet = true;
    }


    public int InputNodeCount => nodeCounts[0];
    public int HiddenNodeCount => nodeCounts[1];
    public int OutputNodeCount => nodeCounts[2];

    public Matrix WeightsInputToHidden() => new Matrix(weightsInputToHidden);
    public Matrix WeightsHiddenToOutput => new Matrix(weightsHiddenToOutput);
    public float[] BiasHidden => biasHidden;
    public float[] BiasOutput => biasOutput;

    public float LearningRate => learningRate;
    public ActivationFunction ativationFunction => activationFunc;
}

[System.Serializable]
public class FlatMatrix
{
    public float[] matrix;
    public int colums;
    public int rows;

    public FlatMatrix(Matrix mat)
    {
        matrix = new float[mat.Colums * mat.Rows];
        colums = mat.Colums;
        rows = mat.Rows;
        Debug.Log(colums);
        Debug.Log(rows);
        Debug.Log(matrix.Length);
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < colums; x++)
            {
                matrix[x + y * colums] = mat.GetMatrix()[y][x];
            }
        }
    }
}
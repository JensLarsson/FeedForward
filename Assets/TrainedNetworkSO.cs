using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "learnedNetwork")]
[System.Serializable]
public class TrainedNetworkSO : ScriptableObject
{
    public int[] nodeCounts;
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

    public float[,] WeightsInputToHidden => weightsInputToHidden.GetMatrix();
    public float[,] WeightsHiddenToOutput => weightsHiddenToOutput.GetMatrix();
    public float[] BiasHidden => biasHidden;
    public float[] BiasOutput => biasOutput;

    public float LearningRate => learningRate;
    public ActivationFunctions ActivationFunction => activationFunc.savedFunctionIndex;
}

[System.Serializable]
public class FlatMatrix
{
    public float[] matrix;
    public int colums;
    public int rows;

    public FlatMatrix(float[,] mat)
    {
        Debug.Log(mat == null);
        colums = mat.GetLength(0);
        rows = mat.GetLength(1);
        matrix = new float[colums * rows];
        for (int i = 0; i < colums; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                matrix[j + i * rows] = mat[i, j];
            }
        }
    }

    public float[,] GetMatrix()
    {
        float[,] newMatrix = new float[colums, rows];
        for (int i = 0; i < colums; i++)
        {
            for (int j = 0; j < rows; j++)
            {
                newMatrix[i, j] = matrix[j + i * rows];
            }
        }
        return newMatrix;
    }
}
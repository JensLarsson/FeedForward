using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NeuralNetwork_Matrix
{
    int inputNodeCount;
    int hiddenNodeCount;
    int outputNodeCount;

    float[,] weightsInputToHidden;
    float[,] weightsHiddenToOutput;

    float[] biasHidden;
    float[] biasOutput;

    float learningRate = 0.1f;

    ActivationFunction activationFunction;
    public NeuralNetwork_Matrix(int inputCount, int hiddenCount, int outputCount, ActivationFunctions activation = ActivationFunctions.Sigmoid)
    {
        inputNodeCount = inputCount;
        hiddenNodeCount = hiddenCount;
        outputNodeCount = outputCount;
        weightsInputToHidden = new float[hiddenCount, inputCount];
        weightsHiddenToOutput = new float[outputCount, hiddenCount];
        RandomizeValues(weightsInputToHidden);
        RandomizeValues(weightsHiddenToOutput);

        biasHidden = new float[hiddenCount];
        biasOutput = new float[outputCount];
        for (int i = 0; i < hiddenCount; i++)
        {
            biasHidden[i] = UnityEngine.Random.Range(0f, 1f);
        }
        for (int i = 0; i < outputCount; i++)
        {
            biasOutput[i] = UnityEngine.Random.Range(0f, 1f);
        }

        activationFunction = new ActivationFunction(activation);

        Debug.Log("input layers: " + inputCount);
        Debug.Log("hidden layers: " + hiddenCount);
        Debug.Log("output layers: " + outputCount);
    }

    public NeuralNetwork_Matrix(TrainedNetworkSO trainedNetwork)
    {
        inputNodeCount = trainedNetwork.InputNodeCount;
        hiddenNodeCount = trainedNetwork.HiddenNodeCount;
        outputNodeCount = trainedNetwork.OutputNodeCount;

        weightsInputToHidden = trainedNetwork.WeightsInputToHidden;
        weightsHiddenToOutput = trainedNetwork.WeightsHiddenToOutput;

        biasHidden = trainedNetwork.BiasHidden;
        biasOutput = trainedNetwork.BiasOutput;

        activationFunction = new ActivationFunction(trainedNetwork.ActivationFunction);

        Debug.Log("input layers: " + inputNodeCount);
        Debug.Log("hidden layers: " + hiddenNodeCount);
        Debug.Log("output layers: " + outputNodeCount);
    }

    public void TrainNeuralNetwork(TrainingData[] trainingData, int itterations = -1, bool randomizeTrainingData = false)
    {
        if (randomizeTrainingData)
        {
            for (int i = 0; i < itterations; i++)
            {
                int random = UnityEngine.Random.Range(0, trainingData.Length);
                Train(trainingData[random].input, trainingData[random].targetResult);
            }
        }
        else
        {
            if (itterations == -1)
            {
                itterations = trainingData.Length;
            }
            for (int i = 0; i < itterations; i++)
            {
                Train(trainingData[i].input, trainingData[i].targetResult);
            }
        }
    }

    //Guesses the output value based on network matrixes
    public float[] Predict(float[] inputNodes)
    {
        float[] hidden = MultiplyValuesWithWeights(weightsInputToHidden, inputNodes);
        RunActivationFunction(ref hidden, activationFunction.Activation);

        float[] output = MultiplyValuesWithWeights(weightsHiddenToOutput, hidden);
        AddBias(ref output, biasOutput);
        RunActivationFunction(ref output, activationFunction.Activation);
        return output;
    }

    void Train(float[] inputArray, float[] targetArray)
    {
        // Generating the Hidden Outputs
        float[] hidden = MultiplyValuesWithWeights(weightsInputToHidden, inputArray);
        RunActivationFunction(ref hidden, activationFunction.Activation);

        float[] outputs = MultiplyValuesWithWeights(weightsHiddenToOutput, hidden);
        AddBias(ref outputs, biasOutput);
        //// activation function!
        RunActivationFunction(ref outputs, activationFunction.Activation);

        // Calculate the error
        // ERROR = TARGETS - OUTPUTS
        float[] outputErrors = CalculateDifference(targetArray, outputs);

        // Calculate gradient
        float[] gradience = (float[])outputs.Clone();
        RunActivationFunction(ref gradience, activationFunction.Activation);
        MultiplyHadamardProduct(ref gradience, outputErrors);
        for (int i = 0; i < gradience.Length; i++)
        {
            gradience[i] *= learningRate;
        }

        // Calculate deltas
        float[,] weightHiddenToOutputDelta = ArrayMatrixMultiplication(gradience, hidden);


        // Adjust the weights by deltas
        AdjustWeights(ref weightsHiddenToOutput, weightHiddenToOutputDelta);

        // Adjust the bias by its deltas (which is just the gradients)
        AddBias(ref biasOutput, gradience);

        // Calculate the hidden layer errors
        float[] hiddenErrors = MultiplyValuesWithWeightsTransposed(weightsHiddenToOutput, outputErrors);

        // Calculate hidden gradient
        float[] hiddenGradience = (float[])hidden.Clone();
        RunActivationFunction(ref hiddenGradience, activationFunction.Deactivation);
        MultiplyHadamardProduct(ref hiddenGradience, hiddenErrors);
        for (int i = 0; i < hiddenGradience.Length; i++)
        {
            hiddenGradience[i] *= learningRate;
        }

        // Calcuate input->hidden deltas
        float[,] weightsInputToHiddenDelta = ArrayMatrixMultiplication(hiddenGradience, inputArray);
        AdjustWeights(ref weightsInputToHidden, weightsInputToHiddenDelta);

        // Adjust the bias by its deltas (which is just the gradients)
        AddBias(ref biasHidden, hiddenGradience);
    }

    void RandomizeValues(float[,] matrix)
    {
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                matrix[i, j] = UnityEngine.Random.Range(-1f, 1f);
            }
        }
    }

    float[,] ArrayMatrixMultiplication(float[] a, float[] b)
    {
        float[,] newMatrix = new float[a.Length, b.Length];
        for (int i = 0; i < a.Length; i++)
        {
            for (int j = 0; j < b.Length; j++)
            {
                newMatrix[i, j] = a[i] * b[j];
            }
        }
        return newMatrix;
    }
    float[,] ArrayMatrixMultiplicationTransposed(float[] a, float[] b)
    {
        float[,] newMatrix = new float[a.Length, b.Length];
        for (int i = 0; i < a.Length; i++)
        {
            for (int j = 0; j < b.Length; j++)
            {
                newMatrix[i, j] = a[i] * b[j];
            }
        }
        return newMatrix;
    }


    //Multiplying each node with all weights
    float[] MultiplyValuesWithWeights(float[,] weightMatrix, float[] value)
    {
        if (weightMatrix.GetLength(1) != value.Length)
        {
            throw new Exception("matrix dimmention missmatch");
            return null;
        }
        float[] newMatrix = new float[weightMatrix.GetLength(0)];
        for (int node = 0; node < newMatrix.Length; node++)
        {
            float sum = 0;
            for (int weight = 0; weight < weightMatrix.GetLength(1); weight++)
            {
                sum += weightMatrix[node, weight] * value[weight];
            }
            newMatrix[node] = sum;
        }
        return newMatrix;
    }

    float[] MultiplyValuesWithWeightsTransposed(float[,] weightMatrix, float[] value)
    {
        if (weightMatrix.GetLength(0) != value.Length)
        {
            throw new Exception("matrix dimmention missmatch");
            return null;
        }
        float[] newMatrix = new float[weightMatrix.GetLength(1)];
        for (int node = 0; node < newMatrix.Length; node++)
        {
            float sum = 0;
            for (int weight = 0; weight < weightMatrix.GetLength(0); weight++)
            {
                sum += weightMatrix[weight, node] * value[weight];
            }
            newMatrix[node] = sum;
        }
        return newMatrix;
    }

    void AddBias(ref float[] target, float[] bias)
    {
        if (target.Length != bias.Length)
        {
            throw new Exception("Arrays need to be same lenght");
        }
        else
        {
            for (int i = 0; i < target.Length; i++)
            {
                target[i] += bias[i];
            }
        }
    }
    void AdjustWeights(ref float[,] target, float[,] values)
    {
        if ((target.GetLength(0) != values.GetLength(0)) || (target.GetLength(1) != values.GetLength(1)))
        {
            throw new Exception("Arrays need to be matching size");
        }
        else
        {
            for (int i = 0; i < target.GetLength(0); i++)
            {
                for (int j = 0; j < target.GetLength(1); j++)
                {
                    target[i, j] += values[i, j];
                }
            }
        }
    }

    float[] CalculateDifference(float[] target, float[] result)
    {
        if (target.Length != result.Length)
        {
            throw new Exception("Arrays need to be same lenght");
        }
        else
        {
            float[] difference = new float[target.Length];
            for (int i = 0; i < target.Length; i++)
            {
                difference[i] = target[i] - result[i];
            }
            return difference;
        }
    }

    void RunActivationFunction(ref float[] target, System.Func<float, float> function)
    {
        for (int i = 0; i < target.Length; i++)
        {
            target[i] = function(target[i]);
        }
    }
    void MultiplyHadamardProduct(ref float[] target, float[] values)
    {
        if (target.Length != values.Length)
        {
            throw new Exception("Arrays need to be same lenght");
        }
        else
        {
            for (int i = 0; i < target.Length; i++)
            {
                target[i] *= values[i];
            }
        }
    }


    public void SetLearningRate(float rate) => learningRate = rate;
    public void SetActivationFunction(ActivationFunctions function) =>
        activationFunction = new ActivationFunction(function);


    public int getInputNodeCount => inputNodeCount;
    public int getHiddenNodeCount => hiddenNodeCount;
    public int getOutputNodeCount => outputNodeCount;

    public float[,] getWeightsInputToHidden => weightsInputToHidden;
    public float[,] getWeightsHiddenToOutput => weightsHiddenToOutput;
    public float[] getBiasHidden => biasHidden;
    public float[] getBiasOutput => biasOutput;

    public float getLearningRate => learningRate;

    public ActivationFunction getActivationFunction => activationFunction;
}

public enum ActivationFunctions { Sigmoid = 0, tanh = 2 };
[Serializable]
public class ActivationFunction
{
    Func<float, float> activation;
    Func<float, float> deactivation;

    public Func<float, float> Activation => activation;
    public Func<float, float> Deactivation => deactivation;

    public ActivationFunctions savedFunctionIndex;

    public ActivationFunction(ActivationFunctions functionIndex)
    {
        savedFunctionIndex = functionIndex;
        activation = functionArray[(int)functionIndex];
        deactivation = functionArray[(int)functionIndex + 1];
    }

    Func<float, float>[] functionArray =
    {
        (float x)=>{return 1f/ (1f+Mathf.Exp(-x)); },
        (float y)=>{return y*(1f-y); },

        (float x)=>{return (float)Math.Tanh(x); },
        (float y)=>{return 1-(y*y); }
    };
}

public struct TrainingData
{
    public float[] input;
    public float[] targetResult;
}

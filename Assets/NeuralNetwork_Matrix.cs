using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    float[] hiddenNodes;

    float[] outputNodes;

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

    public NeuralNetwork_Matrix(TrainedNetworkSO trainedNetwork, bool reversed = false)
    {
        if (reversed == false)
        {
            inputNodeCount = trainedNetwork.InputNodeCount;
            hiddenNodeCount = trainedNetwork.HiddenNodeCount;
            outputNodeCount = trainedNetwork.OutputNodeCount;

            weightsInputToHidden = trainedNetwork.WeightsInputToHidden;
            weightsHiddenToOutput = trainedNetwork.WeightsHiddenToOutput;

            biasHidden = trainedNetwork.BiasHidden;
            biasOutput = trainedNetwork.BiasOutput;

            activationFunction = new ActivationFunction(trainedNetwork.ActivationFunction);
        }
        else
        {

        }
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
        hiddenNodes = MultiplyValuesWithWeights(weightsInputToHidden, inputNodes);
        RunActivationFunction(ref hiddenNodes, activationFunction.Activation);

        outputNodes = MultiplyValuesWithWeights(weightsHiddenToOutput, hiddenNodes);
        AddBias(ref outputNodes, biasOutput);
        RunActivationFunction(ref outputNodes, activationFunction.Activation);

        return outputNodes;
    }

    public float[] PredictReverse(float[] inputNodes)
    {
        float[,] hiddenToOutput = GetTransposedCopy(weightsInputToHidden);
        float[,] inputToHidden = GetTransposedCopy(weightsHiddenToOutput);

        hiddenNodes = MultiplyValuesWithWeights(inputToHidden, inputNodes);
        RunActivationFunction(ref hiddenNodes, activationFunction.Activation);

        outputNodes = MultiplyValuesWithWeights(hiddenToOutput, hiddenNodes);
        AddBias(ref outputNodes, biasOutput);
        RunActivationFunction(ref outputNodes, activationFunction.Activation);

        return outputNodes;
    }


    public void Train(float[] inputArray, float[] targetArray)
    {
        // Generating the Hidden Outputs
        hiddenNodes = MultiplyValuesWithWeights(weightsInputToHidden, inputArray);
        RunActivationFunction(ref hiddenNodes, activationFunction.Activation);

        outputNodes = MultiplyValuesWithWeights(weightsHiddenToOutput, hiddenNodes);
        AddBias(ref outputNodes, biasOutput);
        RunActivationFunction(ref outputNodes, activationFunction.Activation);

        // Calculate the error
        // ERROR = TARGETS - OUTPUTS
        float[] outputErrors = CalculateDifference(targetArray, outputNodes);

        // Calculate gradient
        float[] gradience = (float[])outputNodes.Clone();
        RunActivationFunction(ref gradience, activationFunction.Activation);
        MultiplyHadamardProduct(ref gradience, outputErrors);
        for (int i = 0; i < gradience.Length; i++)
        {
            gradience[i] *= learningRate;
        }

        // Calculate deltas
        float[,] weightHiddenToOutputDelta = ArrayMatrixMultiplication(gradience, hiddenNodes);

        // Adjust the weights by deltas
        MatrixAddition(ref weightsHiddenToOutput, weightHiddenToOutputDelta);

        // Adjust the bias by its deltas (which is just the gradients)
        AddBias(ref biasOutput, gradience);

        // Calculate the hidden layer errors
        float[] hiddenErrors = MultiplyValuesWithWeightsTransposed(weightsHiddenToOutput, outputErrors);

        // Calculate hidden gradient
        float[] hiddenGradience = (float[])hiddenNodes.Clone();
        RunActivationFunction(ref hiddenGradience, activationFunction.Deactivation);
        MultiplyHadamardProduct(ref hiddenGradience, hiddenErrors);
        for (int i = 0; i < hiddenGradience.Length; i++)
        {
            hiddenGradience[i] *= learningRate;
        }

        // Calcuate input->hidden deltas
        float[,] weightsInputToHiddenDelta = ArrayMatrixMultiplication(hiddenGradience, inputArray);
        MatrixAddition(ref weightsInputToHidden, weightsInputToHiddenDelta);

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
        //Parallel.For(0, a.Length, (int i) =>
        {
            for (int j = 0; j < b.Length; j++)
            //Parallel.For(0, b.Length, (int j) =>
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
        //for (int node = 0; node < newMatrix.Length; node++)
        Parallel.For(0, newMatrix.Length, (int node) =>
        {
            float sum = 0;
            for (int weight = 0; weight < weightMatrix.GetLength(1); weight++)
            {
                sum += weightMatrix[node, weight] * value[weight];
            }
            newMatrix[node] = sum;
        });
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
        //for (int node = 0; node < newMatrix.Length; node++)
        Parallel.For(0, newMatrix.Length, (int node) =>
        {
            float sum = 0;
            for (int weight = 0; weight < weightMatrix.GetLength(0); weight++)
            {
                sum += weightMatrix[weight, node] * value[weight];
            }
            newMatrix[node] = sum;
        });
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
    void MatrixAddition(ref float[,] target, float[,] values)
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
            // for (int i = 0; i < target.Length; i++)
            Parallel.For(0, target.Length, (int i) =>
            {
                difference[i] = target[i] - result[i];
            });
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
    float[,] GetTransposedCopy(float[,] input)
    {
        float[,] copy = new float[input.GetLength(1), input.GetLength(0)];
        for (int i = 0; i < input.GetLength(0); i++)
        {
            for (int j = 0; j < input.GetLength(1); j++)
            {
                copy[j, i] = input[i, j];
            }
        }
        return copy;
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

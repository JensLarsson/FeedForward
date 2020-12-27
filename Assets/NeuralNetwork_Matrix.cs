using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NeuralNetwork_Matrix
{
    int inputNodeCount;
    int hiddenNodeCount;
    int outputNodeCount;

    Matrix weightsInputToHidden;
    Matrix weightsHiddenToOutput;
    float[] biasHidden;
    float[] biasOutput;

    float learningRate = 0.1f;

    ActivationFunction activationFunction;
    public NeuralNetwork_Matrix(int inputCount, int hiddenCount, int outputCount, ActivationFunctions activation = ActivationFunctions.Sigmoid)
    {
        inputNodeCount = inputCount;
        hiddenNodeCount = hiddenCount;
        outputNodeCount = outputCount;

        weightsInputToHidden = new Matrix(hiddenCount, inputCount);
        weightsHiddenToOutput = new Matrix(outputCount, hiddenCount);
        weightsInputToHidden.RandomizeValues();
        weightsHiddenToOutput.RandomizeValues();

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
        //biasHidden = new Matrix(hiddenCount, 1);
        //biasOutput = new Matrix(outputCount, 1);
        //biasHidden.RandomizeValues();
        //biasOutput.RandomizeValues();

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

        weightsInputToHidden = trainedNetwork.WeightsInputToHidden();
        weightsHiddenToOutput = trainedNetwork.WeightsHiddenToOutput;

        biasHidden = trainedNetwork.BiasHidden;
        biasOutput = trainedNetwork.BiasOutput;

        activationFunction = trainedNetwork.ativationFunction;

        Debug.Log("input layers: " + inputNodeCount);
        Debug.Log("hidden layers: " + hiddenNodeCount);
        Debug.Log("output layers: " + outputNodeCount);
    }

    //Guesses the output value based on network matrixes
    public float[] Predict(float[] inputNodes)
    {
        Matrix hidden = Matrix.MultiplyDotProduct(weightsInputToHidden, inputNodes);
        hidden.Add(biasHidden);
        hidden.Map(activationFunction.Activation);

        Matrix output = Matrix.MatrixMultiplication(weightsHiddenToOutput, hidden);
        output.Add(biasOutput);
        output.Map(activationFunction.Activation);

        return output.ToArray();
    }

    public void Train(float[] inputArray, float[] targetArray)
    {
        // Generating the Hidden Outputs
        Matrix inputs = new Matrix(inputArray);
        Matrix hidden = new Matrix(Matrix.MatrixMultiplicationColumMatch(weightsInputToHidden, inputArray));
        hidden.Add(biasHidden);
        // activation function!
        hidden.Map(activationFunction.Activation);

        // Generating the output's output!
        Matrix outputs = Matrix.MatrixMultiplication(weightsHiddenToOutput, hidden);
        outputs.Add(biasOutput);
        outputs.Map(activationFunction.Activation);

        // Convert array to matrix object
        Matrix targets = new Matrix(targetArray);

        // Calculate the error
        // ERROR = TARGETS - OUTPUTS
        Matrix output_errors = Matrix.Subtract(targets, outputs);

        // let gradient = outputs * (1 - outputs);
        // Calculate gradient
        Matrix gradients = Matrix.MapClone(outputs, activationFunction.Activation);
        gradients.MultiplyHadamardProduct(output_errors);
        gradients.MultiplyScalarProduct(learningRate);
        //Debug.Log("Gradient " + gradients.Colums + ": " + gradients.Rows);

        // Calculate deltas
        Matrix hidden_T = hidden.GetTransposedClone();
        Matrix weight_ho_deltas = Matrix.MatrixMultiplication(gradients, hidden_T);
        //Matrix weight_ho_deltas = Matrix.MatrixMultiplication(gradients.ToArray(), hidden_T);

        // Adjust the weights by deltas
        this.weightsHiddenToOutput.Add(weight_ho_deltas);
        // Adjust the bias by its deltas (which is just the gradients)
        Matrix.Add(ref this.biasOutput, gradients.ToArray());

        // Calculate the hidden layer errors
        Matrix who_t = weightsHiddenToOutput.GetTransposedClone();
        Matrix hidden_errors = Matrix.MatrixMultiplication(who_t, output_errors);

        // Calculate hidden gradient
        Matrix hidden_gradient = Matrix.MapClone(hidden, activationFunction.Deactivation);
        hidden_gradient.MultiplyHadamardProduct(hidden_errors);
        hidden_gradient.MultiplyScalarProduct(learningRate);
        //Debug.Log("Gradient " + hidden_gradient.Colums + ": " + hidden_gradient.Rows);

        // Calcuate input->hidden deltas
        Matrix inputs_T = inputs.GetTransposedClone();
        //float[] weight_ih_deltas = Matrix.MatrixMultiplicationRowMatch(hidden_gradient, inputArray);
        Matrix weight_ih_deltas = Matrix.MatrixMultiplication(hidden_gradient, inputs_T);

        this.weightsInputToHidden.Add(weight_ih_deltas);
        // Adjust the bias by its deltas (which is just the gradients)
        Matrix.Add(ref this.biasHidden, hidden_gradient.ToArray());
    }

    public void SetLearningRate(float rate) => learningRate = rate;
    public void SetActivationFunction(ActivationFunctions function) =>
        activationFunction = new ActivationFunction(function);


    public int getInputNodeCount => inputNodeCount;
    public int getHiddenNodeCount => hiddenNodeCount;
    public int getOutputNodeCount => outputNodeCount;

    public Matrix getWeightsInputToHidden => weightsInputToHidden;
    public Matrix getWeightsHiddenToOutput => weightsHiddenToOutput;
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

    public ActivationFunction(ActivationFunctions functionIndex)
    {
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

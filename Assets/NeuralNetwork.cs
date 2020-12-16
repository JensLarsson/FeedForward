using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetwork
{
    public NeuralNetworkLayer inputLayer;
    public NeuralNetworkLayer hiddenLayer;
    public NeuralNetworkLayer outputLayer;


    public NeuralNetwork(int inputNodes, int hiddenNodes, int outputNodes)
    {
        outputLayer = new NeuralNetworkLayer(outputNodes, 0, hiddenNodes,
            hiddenLayer, null);

        hiddenLayer = new NeuralNetworkLayer(hiddenNodes, outputNodes, inputNodes,
    inputLayer, outputLayer);
        hiddenLayer.RandomizeWeights();

        inputLayer = new NeuralNetworkLayer(inputNodes, hiddenNodes, 0,
            null, hiddenLayer);
        inputLayer.RandomizeWeights();

    }


    public void Cleanup()
    {
        inputLayer.CleanUp();
        hiddenLayer.CleanUp();
        outputLayer.CleanUp();
    }
    public void SetInput(int index, float value) =>
        inputLayer.SetNeuronValue(index, value);

    public float GetOutput(int index) =>
         outputLayer.GetNeuronValue(index);

    public void SetDesiredOutput(int index, float value) =>
        outputLayer.SetDesiredValue(index, value);

    public void FeedForward()
    {
        inputLayer.CalculateNeuronValues();
        hiddenLayer.CalculateNeuronValues();
        outputLayer.CalculateNeuronValues();
    }
    public void BackPropagate()
    {
        outputLayer.CalculateErrors();

        hiddenLayer.CalculateErrors();

        hiddenLayer.AdjustWeights();

        inputLayer.AdjustWeights();

    }
    public int GetMaxOutputID() =>
        outputLayer.GetMaxNeuronIndex();


    public float CalculateError() => outputLayer.CalculateError();

    public void SetLeanringRate(float value)
    {
        inputLayer.setLearningRate(value);
        hiddenLayer.setLearningRate(value);
        outputLayer.setLearningRate(value);
    }
    public void SetLinearOutput(bool useLinear)
    {
        inputLayer.SetLinearOutput(useLinear);
        hiddenLayer.SetLinearOutput(useLinear);
        outputLayer.SetLinearOutput(useLinear);
    }

    public void SetMomentum(bool useMomentum, float factor = 0.0f)
    {
        inputLayer.SetMomentum(useMomentum, factor);
        hiddenLayer.SetMomentum(useMomentum, factor);
        outputLayer.SetMomentum(useMomentum, factor);
    }

    public void PrintData()
    {
        Debug.Log("---------------------------------------------\n");

        Debug.Log("Input Layer\n");

        Debug.Log("---------------------------------------------\n");

        Debug.Log("Node Values:\n");

        for (int i = 0; i < inputLayer.numberOfNodes; i++)

            Debug.Log($"({i}) = {inputLayer.GetNeuronValue(i)} \n");

        Debug.Log("Weights:\n");

        for (int i = 0; i < inputLayer.numberOfNodes; i++)

            for (int j = 0; j < inputLayer.numberOfChildNodes; j++)

                Debug.Log($"({i}, {i}) = { inputLayer.weights[i, j]} \n");

        Debug.Log("Bias Weights:\n");

        for (int j = 0; j < inputLayer.numberOfChildNodes; j++)

            Debug.Log($"{j} = {inputLayer.biasWeights[j]} \n");

        Debug.Log("\n");

        Debug.Log("---------------------------------------------\n");

        Debug.Log("---------------------------------------------\n");

        Debug.Log("Weights:\n");

        for (int i = 0; i < hiddenLayer.numberOfNodes; i++)

            for (int j = 0; j < hiddenLayer.numberOfChildNodes; j++)

                Debug.Log($"({i}, {i}) = { hiddenLayer.weights[i, j]} \n");

        Debug.Log("\n");

        Debug.Log("Bias Weights:\n");

        for (int j = 0; j < hiddenLayer.numberOfChildNodes; j++)

            Debug.Log($"({j}) = hiddenLayer.biasWeights[j] \n");

        Debug.Log("---------------------------------------------\n");

        Debug.Log("---------------------------------------------\n");

        Debug.Log("Node Values:\n");

        for (int i = 0; i < outputLayer.numberOfNodes; i++)

            Debug.Log($"({i}) = {outputLayer.GetNeuronValue(i)}\n");

        Debug.Log("\n");

    }
}

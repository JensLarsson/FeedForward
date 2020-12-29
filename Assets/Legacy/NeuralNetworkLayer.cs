using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkLayer
{
    public int numberOfNodes;
    public int numberOfChildNodes;
    public int numberOfParentNodes;

    public float[,] weights;
    public float[,] weightsChanges;
    float[] neuronValues;
    float[] desiredValues;
    public float[] errors;
    public float[] biasWeights;
    public float[] biasValues;
    public float learningRate;
    bool linearOutput;
    bool useMomentum;
    float momentumFactor;
    NeuralNetworkLayer parentLayer;
    NeuralNetworkLayer childLayer;

    public NeuralNetworkLayer(int nodeCount, int childNodeCount, int parentNodeCount,
        NeuralNetworkLayer parent, NeuralNetworkLayer child)
    {
        numberOfNodes = nodeCount;
        numberOfChildNodes = childNodeCount;
        numberOfParentNodes = parentNodeCount;
        neuronValues = new float[nodeCount];
        desiredValues = new float[nodeCount];
        errors = new float[nodeCount];
        parentLayer = parent;

        if (child != null)
        {
            childLayer = child;
            weights = new float[nodeCount, numberOfChildNodes];
            weightsChanges = new float[nodeCount, numberOfChildNodes];
            biasValues = new float[numberOfChildNodes];
            biasWeights = new float[numberOfChildNodes];
            for (int i = 0; i < numberOfChildNodes; i++)
            {
                biasValues[i] = -1;
            }
        } //eles null
    }

    public void CleanUp()
    {
        neuronValues = null;
        desiredValues = null;
        errors = null;
        weights = null;
        weightsChanges = null;
        biasValues = null;
        biasWeights = null;
    }
    public void RandomizeWeights()
    {

        for (int i = 0; i < numberOfNodes; i++)
        {
            for (int j = 0; j < numberOfChildNodes; j++)
            {
                weights[i, j] = Random.Range(-1f, 1f);
            }
        }
        for (int j = 0; j < numberOfChildNodes; j++)
        {
            biasWeights[j] = Random.Range(-1f, 1f);
        }
    }
    public void CalculateErrors()
    {
        if (childLayer == null)         //output layer
        {
            for (int i = 0; i < numberOfNodes; i++)
            {
                errors[i] = (desiredValues[i] - neuronValues[i])
                    * neuronValues[i] * (1.0f - neuronValues[i]);
            }
        }
        else if (parentLayer == null)   //input layer
        {
            for (int i = 0; i < numberOfNodes; i++)
            {
                errors[i] = 0.0f;
            }
        }
        else                            // hidden layer
        {
            for (int i = 0; i < numberOfNodes; i++)
            {
                float sum = 0;
                for (int j = 0; j < numberOfChildNodes; j++)
                {
                    sum += childLayer.errors[j] * weights[i, j];
                }
                errors[i] = sum * neuronValues[i] * (1.0f - neuronValues[i]);
            }
        }
    }
    public void AdjustWeights()
    {
        float dw = 0;
        if (childLayer != null)
        {
            for (int i = 0; i < numberOfNodes; i++)
            {
                for (int j = 0; j < numberOfChildNodes; j++)
                {
                    dw = learningRate * childLayer.errors[j] * neuronValues[i];
                    if (useMomentum)
                    {
                        weights[i, j] += dw + momentumFactor * weightsChanges[i, j];
                        weightsChanges[i, j] = dw;
                    }
                    else
                    {
                        weights[i, j] += dw;
                    }
                }
            }
            for (int j = 0; j < numberOfChildNodes; j++)
            {
                biasWeights[j] += learningRate * childLayer.errors[j] * biasValues[j];
            }
        }
    }
    public void CalculateNeuronValues()
    {
        if (parentLayer != null)
        {
            Debug.Log(numberOfParentNodes);
            Debug.Log(parentLayer);
            for (int j = 0; j < numberOfNodes; j++)
            {
                float x = 0;
                for (int i = 0; i < numberOfParentNodes; i++)
                {
                    x += parentLayer.neuronValues[i] * parentLayer.weights[i, j];
                }
                x += parentLayer.biasValues[j] * parentLayer.biasWeights[j];
                if (childLayer == null && linearOutput)
                {
                    neuronValues[j] = x;
                }
                else
                {
                    neuronValues[j] = 1.0f / (1 + Mathf.Exp(-x));
                }
            }
        }
    }

    public float CalculateError()
    {
        float error = 0;
        for (int i = 0; i < numberOfNodes; i++)
        {
            //This was described as neuronValues[i] -- desiredValues[i] in the literature
            error += Mathf.Pow(neuronValues[i] - desiredValues[i], 2);
        }
        return error / numberOfNodes;

    }


    public int GetMaxNeuronIndex()
    {
        int id = 0;
        float maxValue = neuronValues[0];
        for (int i = 1; i < numberOfNodes; i++)
        {
            if (maxValue < neuronValues[i])
            {
                id = i;
                maxValue = neuronValues[i];
            }
        }
        return id;
    }


    public float GetNeuronValue(int index) =>
         (index >= 0 && index < numberOfNodes) ?
            neuronValues[index] :
            float.MaxValue;

    public void SetNeuronValue(int index, float value)
    {
        if (index >= 0 && index < numberOfNodes)
            neuronValues[index] = value;
    }
    public void SetDesiredValue(int index, float value)
    {
        if (index >= 0 && index < numberOfNodes)
            desiredValues[index] = value;
    }
    public void setLearningRate(float value) => learningRate = value;
    public void SetLinearOutput(bool useLinear) => linearOutput = useLinear;
    public void SetMomentum(bool momentum, float factor)
    {
        useMomentum = momentum;
        momentumFactor = factor;
    }
}

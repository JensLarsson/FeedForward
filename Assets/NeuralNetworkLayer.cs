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
    public float[] neuronValues;
    public float[] desiredValues;
    public float[] errors;
    public float[] biasWeight;
    public float[] biasValues;
    public float learningRate;
    bool linearOutput;
    bool useMomentum;
    float momentumFactor;
    NeuralNetworkLayer parentLayer;
    NeuralNetworkLayer childLayer;

    public NeuralNetworkLayer(int nodeCount, NeuralNetworkLayer parent, NeuralNetworkLayer child)
    {
        numberOfNodes = nodeCount;
        neuronValues = new float[nodeCount];
        desiredValues = new float[nodeCount];
        errors = new float[nodeCount];
        parentLayer = parent;

        if (child != null)
        {
            childLayer = child;
            weights = new float[nodeCount, nodeCount];
            weightsChanges = new float[nodeCount, nodeCount];
            biasValues = new float[nodeCount];
            biasWeight = new float[nodeCount];
            for(int i = 0; i < nodeCount; i++)
            {
                biasValues[i] = -1;
            }
        } //eles null
    }

    public void CleanUp()
    {

    }
    public void RandomizeWeights()
    {

    }
    public void CalcuateErrors()
    {

    }
    public void AdjustWeights()
    {

    }
    public void CalculateNeuronValues()
    {

    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class FileByteTranslator : ScriptableObject
{
    [SerializeField] string imageDataPath;
    [SerializeField] string labelDataPath;
    [HideInInspector] public byte[] imageByteArray;
    [HideInInspector] public byte[] labelByteArray;

    public virtual void LoadBytesFromPath()
    {
        imageByteArray = System.IO.File.ReadAllBytes(imageDataPath);
        labelByteArray = System.IO.File.ReadAllBytes(labelDataPath);
        //Debug.Log(imageByteArray.Length + " bytes loaded");
    }
    public virtual void CheckBytes()
    {
        Debug.Log(imageByteArray.Length);
    }
}

[CustomEditor(typeof(ImageArray))]
public class CustomInspectorButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Load Bytes"))
        {
            (target as ImageArray).LoadBytesFromPath();
        }
        if (GUILayout.Button("Check Bytes"))
        {
            (target as ImageArray).CheckBytes();
        }
    }
}
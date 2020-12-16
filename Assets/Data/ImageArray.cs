using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ImageArrayContainer")]
public class ImageArray : FileByteTranslator
{
    const int IMAGE_COUNT_OFFSET = 4;
    const int IMAGE_HEIGHT_OFFSET = 8;
    const int IMAGE_WIDTH_OFFSET = 12;
    const int IMAGE_IMAGE_START_OFFSET = 16;


    public UInt32 imageCount;
    public UInt32 pixelWidth;
    public UInt32 pixelHeight;
    float[][] pixelValueArray;
    byte[] labelValueArray;

    public float[][] PixelValueArray => pixelValueArray;
    public byte[] LabelValueArray => labelValueArray;


    public override void LoadBytesFromPath()
    {
        base.LoadBytesFromPath();

        imageCount = ReverseBytes(BitConverter.ToUInt32(imageByteArray, IMAGE_COUNT_OFFSET));
        pixelHeight = ReverseBytes(BitConverter.ToUInt32(imageByteArray, IMAGE_HEIGHT_OFFSET));
        pixelWidth = ReverseBytes(BitConverter.ToUInt32(imageByteArray, IMAGE_WIDTH_OFFSET));

        //Debug.Log(imageCount + " images in array");
        //Debug.Log(pixelHeight + " pixels wide");
        //Debug.Log(pixelWidth + " pixels high");

        pixelValueArray = new float[imageCount][];
        Debug.Log(pixelValueArray.Length);
        labelValueArray = new byte[imageCount];
        UInt32 arraySize = pixelHeight * pixelWidth;
        for (int i = 0; i < imageCount; i++)
        {
            pixelValueArray[i] = new float[arraySize];
            labelValueArray[i] = labelByteArray[8 + i];
            for (int j = 0; j < arraySize; j++)
            {
                byte temp = imageByteArray[
                    IMAGE_IMAGE_START_OFFSET + i * (int)pixelHeight * (int)pixelWidth + j];

                pixelValueArray[i][j] = (float)temp / 255;
            }
        }
        //System.IO.File.WriteAllBytes("C:/Users/larss/Documents/GitHub/FeedForward/Assets/Data/Test.png", testTexture.EncodeToPNG());


    }

    public Texture2D GetImage(int index)
    {
        if (index >= pixelValueArray.Length)
        {
            Debug.LogError("Index out of range");
            return null;
        }
        Texture2D texture = new Texture2D(28, 28);
        texture.filterMode = FilterMode.Point;
        for (int y = 0; y < 28; y++)
        {
            for (int x = 0; x < 28; x++)
            {
                texture.SetPixel(x, y, new Color(pixelValueArray[index][x + y * 28], 0, 0));
            }
        }
        Debug.Log("Generated texture of value '" + labelValueArray[index].ToString() + "' at index '" + index + "'");
        texture.Apply();
        return texture;
    }

    public override void CheckBytes()
    {
        Debug.Log(pixelValueArray[0].Length);
        Debug.Log(pixelValueArray[imageCount - 1].Length);
    }

    static UInt32 ReverseBytes(UInt32 value)
    {
        return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
            (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
    }

}




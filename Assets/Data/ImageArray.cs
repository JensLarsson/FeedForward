using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ImageArrayContainer")]
[System.Serializable]
public class ImageArray : FileByteTranslator
{
    const int IMAGE_COUNT_OFFSET = 4;
    const int IMAGE_HEIGHT_OFFSET = 8;
    const int IMAGE_WIDTH_OFFSET = 12;
    const int IMAGE_IMAGE_START_OFFSET = 16;


    public UInt32 imageCount;
    public UInt32 pixelWidth;
    public UInt32 pixelHeight;
    public UInt32 pixelValueArrayOffset;
    public float[] pixelValueArray;
    public byte[] labelValueArray;

    //public float[] PixelValueArray => pixelValueArray;
    //public byte[] LabelValueArray => labelValueArray;


    public override void LoadBytesFromPath()
    {
        base.LoadBytesFromPath();

        imageCount = ReverseBytes(BitConverter.ToUInt32(imageByteArray, IMAGE_COUNT_OFFSET));
        pixelHeight = ReverseBytes(BitConverter.ToUInt32(imageByteArray, IMAGE_HEIGHT_OFFSET));
        pixelWidth = ReverseBytes(BitConverter.ToUInt32(imageByteArray, IMAGE_WIDTH_OFFSET));
        pixelValueArrayOffset = pixelHeight * pixelWidth;

        //Debug.Log(imageCount + " images in array");
        //Debug.Log(pixelHeight + " pixels wide");
        //Debug.Log(pixelWidth + " pixels high");

        pixelValueArray = new float[imageCount * pixelValueArrayOffset];
        Debug.Log(pixelValueArray.Length);
        labelValueArray = new byte[imageCount];
        for (int i = 0; i < imageCount; i++)
        {
            labelValueArray[i] = labelByteArray[8 + i];
            for (int j = 0; j < pixelValueArrayOffset; j++)
            {
                byte temp = imageByteArray[
                    IMAGE_IMAGE_START_OFFSET + i * (int)pixelHeight * (int)pixelWidth + j];

                pixelValueArray[i * pixelValueArrayOffset + j] = (float)temp / 255;
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
        Texture2D texture = new Texture2D(28, 28, TextureFormat.R8, false);
        texture.filterMode = FilterMode.Point;
        for (int i = 0; i < 28; i++)
        {
            for (int j = 0; j < 28; j++)
            {
                texture.SetPixel(j, i, new Color(pixelValueArray[index * pixelValueArrayOffset + j + i * 28], 0, 0));
            }
        }
        Debug.Log("Generated texture of value '" + labelValueArray[index].ToString() + "' at index '" + index + "'");
        texture.Apply();
        return texture;
    }

    public Texture2D GetImageFlipped(int index)
    {
        if (index >= pixelValueArray.Length)
        {
            Debug.LogError("Index out of range");
            return null;
        }
        Texture2D texture = new Texture2D(28, 28, TextureFormat.R8, false);
        texture.filterMode = FilterMode.Point;
        for (int i = 27; i >= 0; i--)
        {
            for (int j = 0; j < 28; j++)
            {
                texture.SetPixel(j, i, new Color(pixelValueArray[index * pixelValueArrayOffset + j + i * 28], 0, 0));
            }
        }
        Debug.Log("Generated texture of value '" + labelValueArray[index].ToString() + "' at index '" + index + "'");
        texture.Apply();
        return texture;
    }

    public override void CheckBytes()
    {
        Debug.Log(pixelValueArray.Length / pixelValueArrayOffset);
    }

    static UInt32 ReverseBytes(UInt32 value)
    {
        return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
            (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
    }

}




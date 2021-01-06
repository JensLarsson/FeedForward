using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drawing : MonoBehaviour, IPointerDownHandler
{
    const int IMAGE_SIZE = 28;

    [SerializeField] ImageRecognitionNetwork network;

    float[,] drawValues =
    {
        {0.5f,1.0f,0.5f },
        {1.0f,1.0f,1.0f },
        {0.5f,1.0f,0.5f },
    };

    Image image;
    Vector2 imageRealPixelSize;
    float[] pixelArray = new float[28 * 28];

    bool usDrawing = false;

    private void Start()
    {
        if (image == null)
        {
            image = GetComponent<Image>();
        }
    }

    private void Update()
    {
        if (usDrawing && Input.GetMouseButton(0))
        {
            Vector2 mouseInImagePos = Input.mousePosition - image.rectTransform.position;
            mouseInImagePos /= imageRealPixelSize;              //MousePos to -0.5 - 0.5
            mouseInImagePos.x += 0.5f;                          //MousePos to 0.0 - 1.0
            mouseInImagePos.y += 0.5f;
            mouseInImagePos.y = 1 - mouseInImagePos.y;          //Flipping to match dataset


            if (mouseInImagePos.x > 0f && mouseInImagePos.x < 1f && mouseInImagePos.y > 0f && mouseInImagePos.y < 1f)
            {
                int x = (int)(mouseInImagePos.x * 28);
                int y = (int)(mouseInImagePos.y * 28);

                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (x + i >= 0 && x + i < IMAGE_SIZE && y + j >= 0 && y + j < IMAGE_SIZE)
                        {
                            float value = pixelArray[x + i + (y + j) * 28];
                            value = value > drawValues[i + 1, j + 1] ? value : drawValues[i + 1, j + 1];
                            image.sprite.texture.SetPixel(i + x, j + y, new Color(value, 0, 0));
                            pixelArray[x + i + (y + j) * 28] = value;
                        }
                    }
                }

                image.sprite.texture.Apply();
                network.TestImage(GetCenteredArrayByWeight(pixelArray));
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            usDrawing = false;
        }
    }

    float[] GetCenteredArrayByWeight(float[] values)
    {
        float x = 0;
        float y = 0;
        float mass = 0;

        //read values
        for (int i = 0; i < IMAGE_SIZE; i++)
        {
            for (int j = 0; j < IMAGE_SIZE; j++)
            {
                x += i * values[i * IMAGE_SIZE + j];
                y += j * values[i * IMAGE_SIZE + j];
                mass += values[i * IMAGE_SIZE + j];
            }
        }
        x /= mass;
        y /= mass;

        //offset
        int xOffset = -((int)x - (IMAGE_SIZE / 2));
        int yOffset = -((int)y - (IMAGE_SIZE / 2));

        //itteration Max
        int xMaxIndex = xOffset < 0 ? IMAGE_SIZE + xOffset : IMAGE_SIZE;
        int yMaxIndex = yOffset < 0 ? IMAGE_SIZE + yOffset : IMAGE_SIZE;//<<<

        //Create new array with values moved
        float[] newArray = new float[values.Length];
        for (int i = xOffset > 0 ? xOffset : 0; i < xMaxIndex; i++)
        {
            for (int j = yOffset > 0 ? yOffset : 0; j < yMaxIndex; j++)
            {
                newArray[(i) * IMAGE_SIZE + (j)] = values[(i - xOffset) * IMAGE_SIZE + (j - yOffset)];
            }
        }
        return newArray;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!usDrawing)
        {
            //This should be moved to a check of resolution change 
            Vector3[] pos = new Vector3[4];
            image.rectTransform.GetWorldCorners(pos);
            imageRealPixelSize = pos[3] - pos[1];

            //Reset array and make sprite black
            pixelArray = new float[28 * 28];

            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    image.sprite.texture.SetPixel(i, j, Color.black);
                }
            }
            usDrawing = true;
        }
    }
}

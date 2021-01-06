using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drawing : MonoBehaviour, IPointerDownHandler
{
    const int IMAGE_SIZE = 28;
    const int PADDING = 2;

    [SerializeField] ImageRecognitionNetwork network;

    float[,] drawValues =
    {
        {0.5f,1.0f,0.5f },
        {1.0f,1.0f,1.0f },
        {0.5f,1.0f,0.5f },
    };


    Image image;
    Camera camera;
    Canvas canvas;
    Vector2 size;
    float[] pixelArray = new float[28 * 28];

    bool drawing = false;

    private void Start()
    {
        camera = Camera.main;
        if (image == null)
        {
            image = GetComponent<Image>();
        }
        canvas = transform.parent.GetComponent<Canvas>();
        Vector3[] pos = new Vector3[4];
        image.rectTransform.GetWorldCorners(pos);
        size = pos[3] - pos[1];
    }

    private void Update()
    {
        if (drawing && Input.GetMouseButton(0))
        {
            Vector2 mouseInImagePos = Input.mousePosition - image.rectTransform.position;
            mouseInImagePos /= size;
            mouseInImagePos.x += 0.5f;
            mouseInImagePos.y += 0.5f;
            mouseInImagePos.y = 1 - mouseInImagePos.y;

            //Sloppy code to restrict drawing to the middle
            //This should be replaced either with more training or normalizing the position of the drawn number
            if (mouseInImagePos.x > 0f && mouseInImagePos.x < 1f && mouseInImagePos.y > 0f && mouseInImagePos.y < 1f)
            {
                int x = (int)(mouseInImagePos.x * 28);
                int y = (int)(mouseInImagePos.y * 28);
                //Idiotic conditionals
                for (int i = -1; i <= 1; i++)
                {
                    for (int j = -1; j <= 1; j++)
                    {
                        if (x + i >= PADDING && x + i < IMAGE_SIZE - PADDING && y + j >= PADDING && y + j < IMAGE_SIZE - PADDING)
                        {
                            float value = pixelArray[x + i + (y + j) * 28];
                            value = value > drawValues[i + 1, j + 1] ? value : drawValues[i + 1, j + 1];
                            image.sprite.texture.SetPixel(i + x, j + y, new Color(value, 0, 0));
                            pixelArray[x + i + (y + j) * 28] = value;
                        }
                    }
                }
                //for (int i = x > PADDING ? x - 1 : PADDING; i <= (x >= IMAGE_SIZE - 1 - PADDING ? IMAGE_SIZE - 1 - PADDING : x + 1); i++)
                //{
                //    for (int j = y > PADDING ? y - 1 : PADDING; j <= (y >= IMAGE_SIZE - 1 - PADDING ? IMAGE_SIZE - 1 - PADDING : y + 1); j++)
                //    {
                //        if (pixelArray[i + j * 28] < 0.1)
                //        {
                //            image.sprite.texture.SetPixel(i, j, new Color(1f, 0, 0));
                //            pixelArray[i + j * 28] = drawValues[i, j];
                //        }
                //    }
                //}

                image.sprite.texture.Apply();
                network.TestImage(GetCenteredArrayByWeight(pixelArray));
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            drawing = false;
        }
    }

    float[] GetCenteredArrayByWeight(float[] values)
    {
        float x = 0;
        float y = 0;
        float mass = 0;

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
        if (!drawing)
        {
            //This should be moved to a check of resolution change 
            Vector3[] pos = new Vector3[4];
            image.rectTransform.GetWorldCorners(pos);
            size = pos[3] - pos[1];

            //Reset array and make sprite black
            pixelArray = new float[28 * 28];

            for (int i = 0; i < 28; i++)
            {
                for (int j = 0; j < 28; j++)
                {
                    image.sprite.texture.SetPixel(i, j, Color.black);
                }
            }
            drawing = true;
        }
    }
}

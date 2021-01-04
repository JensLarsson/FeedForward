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
                if (x >= PADDING && x < IMAGE_SIZE - PADDING && y >= PADDING && y < IMAGE_SIZE - PADDING)
                {
                                                                        //Idiotic conditionals
                    for (int i = x > PADDING ? x - 1 : PADDING; i <= (x >= IMAGE_SIZE - 1 - PADDING ? IMAGE_SIZE - 1 - PADDING : x + 1); i++)
                    {
                        for (int j = y > PADDING ? y - 1 : PADDING; j <= (y >= IMAGE_SIZE - 1 - PADDING ? IMAGE_SIZE - 1 - PADDING : y + 1); j++)
                        {
                            if (pixelArray[i + j * 28] < 0.1)
                            {
                                image.sprite.texture.SetPixel(i, j, new Color(1f, 0, 0));
                                pixelArray[i + j * 28] = 1f;
                            }
                        }
                    }
                    image.sprite.texture.SetPixel(x, y, Color.red);
                    image.sprite.texture.Apply();
                    pixelArray[x + y * 28] = 1f;

                    network.TestImage(pixelArray);
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            drawing = false;
        }
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

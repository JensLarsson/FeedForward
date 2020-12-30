using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Drawing : MonoBehaviour, IPointerDownHandler
{
    Image image;
    Camera camera;
    Canvas canvas;
    Vector2 size;
    float[,] pixelArray = new float[28, 28];

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
        if (Input.GetMouseButtonUp(0))
        {
            drawing = false;
        }
        if (drawing)
        {
            Vector2 mouseInImagePos = Input.mousePosition - image.rectTransform.position;
            mouseInImagePos /= size;
            mouseInImagePos.x += 0.5f;
            mouseInImagePos.y += 0.5f;
            mouseInImagePos.y = 1 - mouseInImagePos.y;


            if (mouseInImagePos.x > 0f || mouseInImagePos.x < 1f || mouseInImagePos.y > 0f || mouseInImagePos.y < 1f)
            {
                int x = (int)(mouseInImagePos.x * 28);
                int y = (int)(mouseInImagePos.y * 28);
                image.sprite.texture.SetPixel(x, y, Color.white);
                image.sprite.texture.Apply();


            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        drawing = true;
    }
}

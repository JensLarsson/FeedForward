using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class slökdjgölskdj : MonoBehaviour
{
    Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        camera = Camera.main;
    }

    bool b;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            b = !b;
            camera.backgroundColor = b ? Color.red : Color.blue;
        }
    }
}

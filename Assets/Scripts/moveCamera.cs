using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveCamera : MonoBehaviour
{
    public float speed = 10.0f;
    private Vector3 target;
    public CubeGrid cubeGrid;
    Camera cam;

    private void Start()
    {
        target = new Vector3(cubeGrid.getWidth() / 2f, cubeGrid.getHeight() / 2f, cubeGrid.getWidth() / 2f);
        cam = GetComponent<Camera>();
        transform.LookAt(target);
    }

    void Update()
    {
        //transform.LookAt(target);
        
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
            transform.RotateAround(target, new Vector3(0.0f, 1.0f, 0.0f),  Time.deltaTime * speed * 10);
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
            transform.RotateAround(target, new Vector3(0.0f, -1.0f, 0.0f), Time.deltaTime * speed * 10);
        //if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        //    transform.Translate(Vector3.forward * Time.deltaTime * speed);
        //if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        //    transform.Translate(Vector3.back * Time.deltaTime * speed);
        if (Input.mouseScrollDelta.y != 0)
        {
            cam.fieldOfView -= Input.mouseScrollDelta.y;
            if (cam.fieldOfView < 25) cam.fieldOfView = 25;
            if (cam.fieldOfView > 100) cam.fieldOfView = 100;
        }

    }

}

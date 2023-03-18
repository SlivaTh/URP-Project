using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    [SerializeField] private float xSensivity;
    [SerializeField] private float ySensivity;

    [SerializeField] private Transform orientation;

    private float _xRotation;
    private float _yRotation;
    
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * xSensivity;
        float mousey = Input.GetAxis("Mouse Y") * Time.deltaTime * ySensivity;

        _yRotation += mouseX;
        _xRotation -= mousey;

        _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);
        
        transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, _yRotation, 0);
    }
}

using System;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    public Transform cameraPosition;

    private Transform _thisTransform;

    private void Awake()
    {
        _thisTransform = GetComponent<Transform>();
    }

    void Update()
    {
        _thisTransform.position = cameraPosition.position;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Scrolling : MonoBehaviour
{
    public float backgroundSize;
     
    private Transform cameraTransform;
    private Transform[] layers;
    private float viewZone = 3;
    private int rightIndex;
    private int leftIndex;

    private void Start()
    {
        if (Camera.main != null) 
            cameraTransform = Camera.main.transform;
        layers = new Transform[transform.childCount];
        for (var i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
        }
        leftIndex = 0;
        rightIndex = layers.Length - 1;
    }

    private void Update()
    {
        if (cameraTransform.position.x < (layers[leftIndex].position.x + viewZone))
        {
            ScrollLeft();
        }

        if (cameraTransform.position.x > (layers[rightIndex].position.x - viewZone))
        {
            ScrollRight();
        }
    }

    private void ScrollLeft()
    {
        layers[rightIndex].position = Vector3.right * (layers[leftIndex].position.x - backgroundSize);
        leftIndex = rightIndex;
        rightIndex--;

        if (rightIndex < 0) 
        {
            rightIndex = layers.Length - 1;
        }
    }
    
    private void ScrollRight()
    {
        layers[leftIndex].position = Vector3.right * (layers[rightIndex].position.x + backgroundSize);
        rightIndex = leftIndex;
        leftIndex++;

        if (leftIndex == layers.Length)
        {
            leftIndex = 0;
        }
    }
}

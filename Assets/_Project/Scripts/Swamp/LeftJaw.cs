using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;

public class LeftJaw : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float degrees = 90f;
    [FormerlySerializedAs("rot")] public bool isRotating;
   
    private void Start()
    {
        isRotating = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            isRotating = true;
        }

        if (isRotating)
        {
            Rotate();
        }
    }

    private void Rotate()
    {
        Vector3 to = new Vector3(0, 0, degrees);
        transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, to, Time.deltaTime * speed);
    }
}

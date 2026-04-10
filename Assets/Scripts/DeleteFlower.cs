using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class DeleteFlower : MonoBehaviour
{
    [SerializeField] private GameObject flower;
    [SerializeField] private  float y;

    private void Update()
    {
        y = transform.position.y;
        if (y <= -16f)
        {
            Destroy(flower);
        }
        
    }
}

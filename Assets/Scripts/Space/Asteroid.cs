using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float speedX = -0.001f;
    [SerializeField] private float speedY = -0.001f;
    
    private void Update()
    {
        transform.Translate(speedY, speedX, 0f);
    }
}

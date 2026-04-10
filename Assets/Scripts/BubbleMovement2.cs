using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(Rigidbody2D))]
public class BubbleMovement2 : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float frequency;
    [SerializeField] private float magnitude;
    [SerializeField] private Vector2 direction;
    
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {

        direction.x = Mathf.Sin(Time.fixedTime * frequency) * magnitude;

        rb.AddForce(direction * speed);

    }
}

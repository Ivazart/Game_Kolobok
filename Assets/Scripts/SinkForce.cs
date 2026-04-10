using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SinkForce : MonoBehaviour
{
    public float speed;
    public bool isDrown = false;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (!isDrown) 
            return;
        
        rb.constraints = RigidbodyConstraints2D.None;
        rb.freezeRotation = true;
        rb.MovePosition(rb.position + Vector2.down * speed);

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isDrown = true;
        }
    }
}

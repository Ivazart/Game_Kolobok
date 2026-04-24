using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    public bool IsGrounded { get; private set; }

    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnTriggerStay2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("obstacle") || trig.gameObject.CompareTag("stopper"))
        {
            IsGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("obstacle") || trig.gameObject.CompareTag("stopper"))
        {
            IsGrounded = false;
        }
    }

}

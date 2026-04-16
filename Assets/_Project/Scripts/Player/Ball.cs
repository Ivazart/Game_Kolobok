using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    public event Action <bool> OnPushChanged ;
    public Vector3 Pos => transform.position;
    
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
            OnPushChanged?.Invoke(false);
        }
    }

    public void Push(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void ActivateRb()
    {
        OnPushChanged?.Invoke(true);
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void DeactivateRb()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

}

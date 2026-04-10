using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Stopper : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private float scale;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("stopper") & gameManager.isPushed == false)
        {
            BallStop();
        }

    }

    private void BallStop()
    {
        Vector2 speed = rb.linearVelocity;
        rb.linearVelocity = speed * scale;
    }
}
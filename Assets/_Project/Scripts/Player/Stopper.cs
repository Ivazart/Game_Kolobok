using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Stopper : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private Ball ball;
    [SerializeField] private float scale;

    private bool isPushed;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        ball.OnPushChanged += b => isPushed = b;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("stopper") && isPushed == false)
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
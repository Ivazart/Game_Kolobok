using System.Collections;
using System.Collections.Generic;
using _Project.Player;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Stopper : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float scale;
    [SerializeField] private MovementDetector movementDetector;
    private bool isPushed => movementDetector.IsMoving;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
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
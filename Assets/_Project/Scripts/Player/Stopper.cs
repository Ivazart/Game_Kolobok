using System.Collections;
using System.Collections.Generic;
using _Project.Player;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Stopper : MonoBehaviour
{
    [SerializeField] private float scale;
    [SerializeField] private MovementDetector movementDetector;
    
    private Rigidbody2D rb;
    private bool isPushed => movementDetector.CanMove;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        movementDetector.OnCanBeStopped += BallStop;
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
        Debug.Log("Ball stop");
        Vector2 speed = rb.linearVelocity;
        rb.linearVelocity = speed * scale;
    }
}
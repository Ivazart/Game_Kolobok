using System.Collections;
using System.Collections.Generic;
using _Project.Player;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class Stopper : MonoBehaviour
{
    [SerializeField] private float scale;
   
    public bool IsPushed { get; set; }
    
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("stopper") && IsPushed == false )
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
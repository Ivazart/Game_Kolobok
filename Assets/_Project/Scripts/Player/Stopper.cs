using System.Collections;
using System.Collections.Generic;
using _Project.Player;
using Unity.Burst.Intrinsics;
using UnityEngine;

[RequireComponent(typeof(CollisionLogic))]
[RequireComponent(typeof(Rigidbody2D))]
public class Stopper : MonoBehaviour
{
    [SerializeField] private float scale;
    public bool IsPushed { get; set; }
    
    private Rigidbody2D rb;
    private CollisionLogic collision;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        collision = GetComponent<CollisionLogic>();
        collision.OnEnterPlayerSolidEnemySolid += Collision_OnEnterPlayerSolidEnemySolid;
    }

    private void Collision_OnEnterPlayerSolidEnemySolid(CollisionEventData data)
    {
        if (data.OtherCollider.gameObject.CompareTag("stopper") && IsPushed == false )
            BallStop();
    }

    /*private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("stopper") && IsPushed == false )
        {
            BallStop();
        }

    }*/

    public void BallStop()
    {
        Debug.Log("Ball stop");
        Vector2 speed = rb.linearVelocity;
        rb.linearVelocity = speed * scale;
    }
}
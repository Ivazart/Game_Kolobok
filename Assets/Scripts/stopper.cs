using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public class stopper : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    //[HideInInspector] public CircleCollider2D col;
    public GameManager gameManager;
    public float scale;
   

    void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
        //col = GetComponent<CircleCollider2D>();

    }

  
    void Update()
    {  

        
    }
    void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.tag == "stopper" & gameManager.push == false)
        {
            BallStop();
        }

    }

    void BallStop()
    {
        Vector2 speed = rb.linearVelocity;
        rb.linearVelocity = speed * scale;
         
       

    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
   
    


    void Start()
    {
        
    }

    
    void Update()
    {
        Vector2 screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 m = Camera.main.ScreenToWorldPoint(screenPosition);
        Vector2 rbp = rb.position;

        if (Input.GetMouseButtonDown(0))
            rb.AddForce((m - rbp) * speed);
            //rb.AddForce (Vector2.up * speed);
           



    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buble_move : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + Vector2.down * speed);
    }
}

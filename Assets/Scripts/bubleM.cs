using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bubleM : MonoBehaviour
{

    public GameObject babble;
    public Rigidbody2D rb;
    public float speed;
    public float frequency;
    public float magnitude;
    public Vector2 direction;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {

        direction.x = Mathf.Sin(Time.fixedTime * frequency) * magnitude;

        rb.AddForce(direction * speed);

    }
}

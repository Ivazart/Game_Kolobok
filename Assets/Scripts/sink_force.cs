using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sink_force : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;
    public bool drown = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (drown == true)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.freezeRotation = true;


            rb.MovePosition(rb.position + Vector2.down * speed);

        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            drown = true;







        }
    }
}

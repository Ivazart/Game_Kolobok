using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public CircleCollider2D col;

    [HideInInspector] public Vector3 pos { get { return transform.position; }}

    public bool isGrounded;
    public GameManager gameManeager;
    

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
    }

  

    void OnTriggerStay2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "obstacle" || trig.gameObject.tag == "stopper")
        {
            isGrounded = true;
        }
    }
    void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.gameObject.tag == "obstacle" || trig.gameObject.tag == "stopper")
        {
            isGrounded = false;
            gameManeager.push = false;

        }
    }

    public void Push(Vector2 force)
    { 
       
        rb.AddForce(force, ForceMode2D.Impulse);

    }

    public void ActivateRb()
    {
        gameManeager.push = true;
        rb.isKinematic = false;
    }

    public void DesactivateRb()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.isKinematic = true;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      


    }
}

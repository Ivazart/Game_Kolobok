using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sink : MonoBehaviour
{
    [SerializeField] private float speed;
    private bool isDrown = false;

    private void FixedUpdate() 
    { 
        if (isDrown)
        {
            transform.Translate(Vector3.down * speed);
        }
        
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("sink");
            isDrown= true;
        }
    }
}

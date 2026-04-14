using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkTemp : MonoBehaviour
{
    [SerializeField] private float speed;
    private bool isDrown = false;

    private void FixedUpdate()
    {
        if (isDrown)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);
        }
    }
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isDrown = true;
        }
    }
    
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isDrown = false;
        }
    }
}

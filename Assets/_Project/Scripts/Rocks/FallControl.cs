using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallControl : MonoBehaviour
{
    public Fall[] Fall;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (var fall in Fall) 
            {
                fall.Drop();
            }
        }
    }
}

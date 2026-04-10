using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public float speed;

    private void Update()
    {
        var screenPosition = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        
        if (Camera.main != null)
        {
            Vector2 m = Camera.main.ScreenToWorldPoint(screenPosition);
            Vector2 rbp = rb.position;

            if (Input.GetMouseButtonDown(0))
                rb.AddForce((m - rbp) * speed);
        }
    }
}

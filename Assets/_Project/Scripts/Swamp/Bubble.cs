using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bubble : MonoBehaviour
{
    [SerializeField] private Vector2 direction;

    private Rigidbody2D rb;
    private float speed;
    private float frequency;
    private float magnitude;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        speed = Random.Range(0f, 1f);
        frequency = Random.Range(2f, 4f);
        magnitude = Random.Range(3f, 5f);
        float n = Random.Range(0.05f, 0.3f);
        rb.transform.localScale = new Vector3(n, n, n);
    }

    private void FixedUpdate()
    {
        direction.x = Mathf.Sin(Time.fixedTime * frequency) * magnitude;
        rb.AddForce(direction * speed);
    }
    
}

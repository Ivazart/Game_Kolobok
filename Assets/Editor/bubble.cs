using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D;
using UnityEngine;

public class bubble : MonoBehaviour
{
    public GameObject babble;
    public Rigidbody2D rb;
    public float speed;
    public float frequency;
    public float magnitude;
    public Vector2 direction;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void Awake()
    {
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

    private void Delete()
    {
        
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class Ball : MonoBehaviour
{
    [HideInInspector] public bool isGrounded;
    [HideInInspector] public Vector3 pos => transform.position;
    [SerializeField] private GameManager gameManager;
    
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    private void OnTriggerStay2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("obstacle") || trig.gameObject.CompareTag("stopper"))
        {
            isGrounded = true;
        }
    }

    private void OnTriggerExit2D(Collider2D trig)
    {
        if (trig.gameObject.CompareTag("obstacle") || trig.gameObject.CompareTag("stopper"))
        {
            isGrounded = false;
            gameManager.isPushed = false;
        }
    }

    public void Push(Vector2 force)
    {
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public void ActivateRb()
    {
        gameManager.isPushed = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    public void DeactivateRb()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = 0f;
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

}

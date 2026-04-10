using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour
{
    public Rigidbody2D rb;
    public ScenesManager scenesManager;
    public PlayerSwampAnimation anim;

    private void Restart()
    {
        scenesManager.RestartGame();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("poison") || collision.gameObject.CompareTag("faire") )
        {
            rb.linearVelocity = Vector3.zero;
            Invoke(nameof(Restart), 1f);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("swamp")) 
            return;
        
        anim.isSwamp = true;
        GetComponent<SpriteRenderer>().enabled = false;
        rb.linearVelocity = Vector3.zero;
        rb.bodyType = RigidbodyType2D.Kinematic;
        Invoke(nameof(Restart), 2f);
    }
}

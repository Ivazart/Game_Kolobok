using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poison : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameManager gameManeager;
    public ScenesManager scenesManager;
    public pl_swamp_anim anim;

    void Restart()
    {
        scenesManager.RestartGame();

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "poison" )
        {
            rb.linearVelocity = Vector3.zero;
            Invoke("Restart", 1f);


        }
        if (collision.gameObject.tag == "faire")
        {
            rb.linearVelocity = Vector3.zero;
            Invoke("Restart", 1f);


        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "swamp")
        {
            anim.swamp = true;
            GetComponent<SpriteRenderer>().enabled = false;
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
           
            //gameObject.GetComponent<RigidBody2D>().SetActive(false);
            Invoke("Restart", 2f);




        }

    }



}

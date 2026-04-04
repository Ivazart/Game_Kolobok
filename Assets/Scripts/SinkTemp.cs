using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SinkTemp : MonoBehaviour
{
    public float speed;
    bool drown = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (drown == true)
        {
            transform.Translate(Vector3.down * speed * Time.deltaTime);

        }

    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            drown = true;

        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            drown = false;

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallControl : MonoBehaviour
{
    public fall[] Fall;
    
    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (var fall in Fall) 
            {
                fall.drope();

            }

            
        }

    }


    


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChekPoints : MonoBehaviour
{
    public Transform player;
    public int index;
    void Awake()
    {
       if (DataContainer.checkpointIndex == index) 
       { 
          player.position = transform.position;
        
       }
        



    }

   
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            DataContainer.checkpointIndex= index;
        }
    }
}

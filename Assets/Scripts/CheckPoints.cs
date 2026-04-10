using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    public Transform player;
    public int index;

    private void Awake()
    {
       if (DataContainer.CheckpointIndex == index) 
       { 
          player.position = transform.position;
       }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            DataContainer.CheckpointIndex= index;
        }
    }
}

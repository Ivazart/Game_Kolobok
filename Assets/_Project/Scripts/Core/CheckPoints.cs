using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    public static event Action<CheckPoints> OnCheckpointEnter;
    public int index;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnCheckpointEnter?.Invoke(this);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    public static event Action<CheckPoints> OnCheckpointEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            OnCheckpointEnter?.Invoke(this);
        }
    }
}

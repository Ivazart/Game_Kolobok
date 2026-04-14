using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Trigger : MonoBehaviour
{
    [SerializeField] private SinkForce sinkForce;

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            sinkForce.isDrown = true;
        }
    }
}

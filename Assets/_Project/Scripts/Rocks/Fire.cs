using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Fire : MonoBehaviour
{
    private FireD fire;

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            fire = other.GetComponent<FireD>();
            fire.Burned();
        }
    }
}
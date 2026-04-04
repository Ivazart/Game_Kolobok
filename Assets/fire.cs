using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire : MonoBehaviour
{
    public fire_d fir;
    public ParticleSystem ps;

    void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            fir.Burned();

        }

    }
   

    
}
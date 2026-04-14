using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class FireD : MonoBehaviour
{
    public Rigidbody2D rb;
    public ScenesManager scenesManager;

    public void Burned()
    {
        rb.linearVelocity = Vector3.zero;
        Invoke(nameof(Restart), 1f);
    }

    private void Restart()
    {
        scenesManager.RestartGame();
    }

}

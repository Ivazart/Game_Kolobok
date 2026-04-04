using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire_d : MonoBehaviour
{
    public Rigidbody2D rb;
    public GameManager gameManeager;
    public ScenesManager scenesManager;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Burned()
    {
        rb.linearVelocity = Vector3.zero;
        Invoke("Restart", 1f);

    }
    void Restart()
    {
        scenesManager.RestartGame();

    }

}

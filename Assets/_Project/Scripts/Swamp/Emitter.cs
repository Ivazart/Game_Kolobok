using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Emitter : MonoBehaviour
{
    [SerializeField] private GameObject bubble;
    [SerializeField] private int n;

    private GameObject parent;
   
    private void Awake()
    {
        parent = GameObject.FindWithTag("Temporal");
    }

    private void Update()
    {
        Create();
    }

    private void Create()
    {
        int random = UnityEngine.Random.Range(1, n);
        if (random == 1)
        {
            var bubbleGO = Instantiate(bubble,parent.transform);
            bubbleGO.transform.position = transform.position;
            Destroy(bubbleGO, 17);
        }

    }
}   

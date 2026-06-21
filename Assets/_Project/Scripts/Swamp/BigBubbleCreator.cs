using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class BigBubbleCreator : MonoBehaviour
{
    [SerializeField] private GameObject bubblePrefab;
    private GameObject parent;
   
    private void Awake()
    {
        parent = GameObject.FindWithTag("Temporal");
    }

    public void CreateBubble()
    {
        var bubbleGO = Instantiate(bubblePrefab,parent.transform);
        bubbleGO.transform.position = transform.position;
    }
    
}

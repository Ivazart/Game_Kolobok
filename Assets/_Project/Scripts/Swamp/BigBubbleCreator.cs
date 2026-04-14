using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public class BigBubbleCreator : MonoBehaviour
{
    [SerializeField] private GameObject bubblePrefab;
    
    public void CreateBubble()
    {
        var bubbleGO = Instantiate(bubblePrefab);
        bubbleGO.transform.position = transform.position;
    }
    
}

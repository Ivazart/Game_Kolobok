using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class EmmiterSt : MonoBehaviour
{
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private float time;

    private void Start()
    {
        InvokeRepeating(nameof(Create), 2.0f, time);
    }

    private void Create()
    {
        var babbleC = Instantiate(bubblePrefab);
        babbleC.transform.position = transform.position;
        Destroy(babbleC, 17);
    }
}

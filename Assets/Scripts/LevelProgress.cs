using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgress : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private Transform endLineTransform;
    [SerializeField] private Transform startLineTransform;
    [SerializeField] private Slider slider;

    private Vector3 endLinePosition;
    private float fullDistance;


    private void Start()
    {
        endLinePosition = endLineTransform.position;
        fullDistance = Vector3.Distance(startLineTransform.position, endLinePosition);
    }

    private void Update()
    {
        float newDistance = GetDistance();
        slider.value = Mathf.InverseLerp( fullDistance, 0f, newDistance);
    }

    private float GetDistance ()
    {
        return Vector3.Distance(playerTransform.position, endLinePosition);
    }
}

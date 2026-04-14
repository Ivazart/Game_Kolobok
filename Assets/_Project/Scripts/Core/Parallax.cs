using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Transform followingTarget;
    [SerializeField, Range(0f, 1f)] private float ParallaxStrength = 0.1f;

    private Vector3 targetPreviousPosition;

    private void Start()
    {
        if (!followingTarget)
        {
            if (Camera.main != null) 
                followingTarget = Camera.main.transform;
        }
        targetPreviousPosition = followingTarget.position;
    }

    private void Update()
    {
        Vector3 delta;
        var position = followingTarget.position;
        delta = position - targetPreviousPosition;
        targetPreviousPosition = position;
        transform.position += delta * ParallaxStrength;
    }
}

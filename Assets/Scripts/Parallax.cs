using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] Transform followingTarget;
    [SerializeField, Range(0f, 1f)] float ParallaxStrength = 0.1f;
    Vector3 targetPreviousPosition;
    // Start is called before the first frame update
    void Start()
    {
        if (!followingTarget)
        {
            followingTarget = Camera.main.transform;
        }
        targetPreviousPosition = followingTarget.position;   
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 delta;
        delta = followingTarget.position - targetPreviousPosition;

        targetPreviousPosition = followingTarget.position;
        transform.position += delta * ParallaxStrength;            
        
    }
}

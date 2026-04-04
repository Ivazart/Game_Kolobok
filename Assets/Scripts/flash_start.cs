using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class flash_start : MonoBehaviour
{
    public SkeletonAnimation flash;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Flash()
    {
        flash.AnimationState.SetAnimation(0, "animation", false);

    }
}

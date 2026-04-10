using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation) )]
public class FlashStart : MonoBehaviour
{
    private SkeletonAnimation flash;

    private void Awake()
    {
        flash = GetComponent<SkeletonAnimation>();
    }

    private void Flash()
    {
        flash.AnimationState.SetAnimation(0, "animation", false);
    }
}

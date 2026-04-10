using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class ScreenAnimation : MonoBehaviour
{
    public SkeletonAnimation screen;

    private void Awake()
    {
        screen = GetComponent<SkeletonAnimation>();
    }

    public void alarm_anim()
    {
        screen.AnimationState.SetAnimation(0, "alarm", true);
    }

    public void ActiveState()
    {
        Invoke(nameof(SetActivate), 2f);
    }
    public void SetActivate()
    {
        screen.AnimationState.SetAnimation(0, "activate", true);
    }

}

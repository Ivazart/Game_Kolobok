using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class ManAnimation : MonoBehaviour
{
    private SkeletonAnimation man;

    private void Awake()
    {
        man = GetComponent<SkeletonAnimation>();
    }

    public void alarm_anim()
    {
        man.AnimationState.SetAnimation(0, "alarm", true);
    }
    public void activ_anim()
    {
        man.AnimationState.SetAnimation(0, "activate", false);
    }
}

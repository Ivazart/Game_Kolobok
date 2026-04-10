using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using UnityEngine.Serialization;

[RequireComponent(typeof(SkeletonAnimation))]
public class PlayerSwampAnimation : MonoBehaviour
{
    public bool isSwamp = false;
    private bool isAlive = true;
    private SkeletonAnimation skAnim;

    private void Awake()
    {
        skAnim = GetComponent<SkeletonAnimation>();
    }

    private void Update()
    {
        if (isSwamp && isAlive)
        {
            GetComponent<MeshRenderer>().enabled = true;
            isAlive = false;
            skAnim.AnimationState.SetAnimation(0, "pl_swamp", false);
        }
    }
}
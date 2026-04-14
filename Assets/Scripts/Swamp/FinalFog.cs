using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class FinalFog : MonoBehaviour
{
    private SkeletonAnimation skAnim;

    private void Awake()
    {
        skAnim = GetComponent<SkeletonAnimation>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            skAnim.AnimationState.SetAnimation(0, "animation", false);
        }
    }
}

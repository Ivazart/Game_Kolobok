using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class FlowerFog : MonoBehaviour
{
    private SkeletonAnimation skAnim;
    
    private void Awake()
    {
        skAnim = GetComponent<SkeletonAnimation>();
        skAnim.AnimationState.SetAnimation(0, "cycle", true);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Fade();
        }
    }

    private void Fade()
    {
        StartCoroutine(FadeFog());
    }

    private IEnumerator FadeFog()
    {
        var track = skAnim.state.SetAnimation(0, "disappearance", false);
        yield return new WaitForSpineAnimationComplete(track);
    }
    
}
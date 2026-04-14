using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;


public class SpineAnimationFader : MonoBehaviour
{
    private SkeletonAnimation spineAnim;
    private void Awake()
    {
        spineAnim = GetComponent<SkeletonAnimation>();
    }

    public async UniTask FadeOutSpine(float duration)
    {
        await DOTween.To(() => spineAnim.skeleton.A, x => spineAnim.skeleton.A = x, 0f, duration).ToUniTask();
    }
}
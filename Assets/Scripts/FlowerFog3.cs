using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(SkeletonAnimation))]
public class FlowerFog3 : MonoBehaviour
{
    private SkeletonAnimation skAnim;
    [SerializeField] private bool isActive;
    
    private void Awake()
    {
        skAnim = GetComponent<SkeletonAnimation>();
        skAnim.AnimationState.SetAnimation(0, "cycle", true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && isActive)
        {
            skAnim.state.SetAnimation(0, "disappearance", false);
            isActive = false;
        }
    }
}

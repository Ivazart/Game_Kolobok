using System;
using Spine.Unity;
using UnityEngine;

namespace StartingLab
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class StartingLabAnimation : MonoBehaviour
    {
        private const string IdleAnimationName = "idle";
        private const string AlarmAnimationName = "alarm";
        private const string ActiveAnimationName = "activate";

        private SkeletonAnimation skeletonAnimation;

        private void Awake()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
        }

        public virtual void SetState(StartingLabState state)
        {
            string animationName = state switch
            {
                StartingLabState.Idle => IdleAnimationName,
                StartingLabState.Alarm => AlarmAnimationName,
                StartingLabState.Active => ActiveAnimationName,
                _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
            };
            TryPlayAnimation(animationName);
        }

        private void TryPlayAnimation(string animationName)
        {
            Spine.Animation anim = skeletonAnimation.Skeleton.Data.FindAnimation(animationName);

            if (anim == null)
            {
                Debug.LogError($"Animation '{animationName}' not found in {gameObject.name}!");
            }
            else
            {
                Debug.Log($" {gameObject.name} is playing animation {animationName}");
                skeletonAnimation.AnimationState.SetAnimation(0, anim, true);
            }
        }
    }
}
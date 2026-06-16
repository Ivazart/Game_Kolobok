using System;
using Cysharp.Threading.Tasks;
using Global;
using Spine.Unity;
using UnityEngine;

namespace StartingLab
{
    [RequireComponent(typeof(SkeletonAnimation))]
    public class StartingLabAnimation : MonoBehaviour
    {
        protected const string IdleAnimationName = "idle";
        protected const string AlarmAnimationName = "alarm";
        protected const string ActiveAnimationName = "activate";

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
            PlayAnimation(animationName).Forget();
        }

        protected virtual async UniTask PlayAnimation(string animationName, bool loop = true)
        {
           await UniTaskUtils.PlayAnimation(skeletonAnimation, animationName, loop: loop );
        }
        
    }
}
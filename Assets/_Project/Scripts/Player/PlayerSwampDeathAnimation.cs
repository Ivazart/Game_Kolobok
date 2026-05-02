using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Global;
using Spine.Unity;
using UnityEngine;

namespace _Project.Player
{
    public class PlayerSwampDeathAnimation: MonoBehaviour
    {
        
        private SkeletonAnimation skeletonAnimation;
        
        [SerializeField] private string Death;
        
        private void Awake()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            
        }
        
        public async UniTask PlaySwampDeath()
        {
            await UniTaskUtils.PlayAnimation(skeletonAnimation, Death);
        }
  
    }
}
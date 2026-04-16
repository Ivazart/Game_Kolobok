using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global;
using Spine.Unity;
using UnityEngine;

namespace _Project.Player
{
    public class PlayerAnimation: MonoBehaviour
    {
        
        private SkeletonAnimation skeletonAnimation;
        
        [SerializeField] private string idle;
        [SerializeField] private string deathPoison;
        [SerializeField] private string deathFire;
        
        private void Awake()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
        }

        public async UniTask PlayDeath(DeathType deathType)
        {
            string death = deathType switch
            {
                DeathType.Poison => deathPoison,
                DeathType.Fire => deathFire,
                DeathType.Swamp => deathPoison,
                _ => deathPoison
            };
            await UniTaskUtils.PlayAnimation(skeletonAnimation, death);
        }

        public async UniTask PlayIdle(DeathType deathType)
        {
           await UniTaskUtils.PlayAnimation(skeletonAnimation, idle, loop:true);
        }

        
    }
}
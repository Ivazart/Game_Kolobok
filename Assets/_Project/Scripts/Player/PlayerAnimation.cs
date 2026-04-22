using System;
using System.Threading;
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
        [SerializeField] private string staticIdle;
        [SerializeField] private string deathEyes;
        [SerializeField] private string deathPoison;
        [SerializeField] private string deathFire;
        [SerializeField] private string drag;
        [SerializeField] private string eyesDown;
        [SerializeField] private string eyesUp;
        [SerializeField] private string blinking;
        
        private bool idleStart = false;
        private string[] idleAnimations = { "idle", "static2" };
        private CancellationTokenSource cts = new();
        private void Awake()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            UniTaskUtils.RunWithCancellationAsync(Blinking, cts.Token).Forget();
        }

        private async UniTask Blinking (CancellationToken token)
        {
            while (token.IsCancellationRequested == false)
            {
                await UniTaskUtils.PlayAnimation(skeletonAnimation, blinking, 4, loop: false, token: token);
                await UniTask.WaitForSeconds(2f, cancellationToken: token);
            }
        }
        
        public async UniTask PlayDeath(DeathType deathType)
        {
            cts.Cancel();
            string death = deathType switch
            {
                DeathType.Poison => deathPoison,
                DeathType.Fire => deathFire,
                DeathType.Swamp => deathPoison,
                _ => deathPoison
            };
            await UniTaskUtils.PlayAnimation(skeletonAnimation, deathEyes, 1, true, 0.7f);
            await UniTaskUtils.PlayAnimation(skeletonAnimation, death);
        }
        
        public async UniTask PlayEyesDown()
        {
            await UniTaskUtils.PlayAnimation(skeletonAnimation, eyesDown);
        }
        
        public async UniTask PlayEyesUp()
        {
            await UniTaskUtils.PlayAnimation(skeletonAnimation, eyesUp, 3);
        }

        public async UniTask PlayIdle()
        {
            if (!idleStart)
            {
                idleStart = true;
                await UniTaskUtils.PlayAnimation(skeletonAnimation, staticIdle, 1, loop:true);
            }
        }
        
        public async UniTask PlayStaticIdle()
        {
            var time = UnityEngine.Random.Range(1000, 2000);
            
            idleStart = true;
            string animName = idleAnimations[UnityEngine.Random.Range(0, idleAnimations.Length)];
            await UniTaskUtils.PlayAnimation(skeletonAnimation, animName, 1, loop:true);
            await UniTask.Delay(time);
            StopIdle();
        }

        public void StopIdle()
        {
            if (idleStart)
            {
                idleStart = false;
                skeletonAnimation.AnimationState.SetEmptyAnimation(1, 0.5f);
            }
        }
        
        public async UniTask PlayDrag()
        {
            await UniTaskUtils.PlayAnimation(skeletonAnimation, drag);
        }

        private void OnDestroy()
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Global;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine;

namespace _Project.Player
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(SkeletonAnimation))]
    public class PlayerAnimation: MonoBehaviour
    {
        [SerializeField] private PlayerSwampDeathAnimation swampDeathAnimation;
        [SerializeField] private string idle;
        [SerializeField] private string staticIdle;
        [SerializeField] private string deathEyes;
        [SerializeField] private string deathPoison;
        [SerializeField] private string deathFire;
        [SerializeField] private string drag;
        [SerializeField] private string eyesDown;
        [SerializeField] private string eyesUp;
        [SerializeField] private string blinking;
        [SerializeField] private string acidDeath;
        [SerializeField] private string infectionDeath;
        [SerializeField] private string splashDeath;
        
        private bool idleStart = false;
        private string[] idleAnimations = { "idle", "static2" };
        private CancellationTokenSource cts = new();
        
        private SkeletonAnimation skeletonAnimation;
        private MeshRenderer meshRenderer;
       
        private void Awake()
        {
            skeletonAnimation = GetComponent<SkeletonAnimation>();
            meshRenderer = GetComponent<MeshRenderer>();
            UniTaskUtils.RunWithCancellationAsync(Blinking, cts.Token).Forget();
            
        }

        private void Start()
        {
            swampDeathAnimation.gameObject.SetActive(false);
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
                DeathType.Acid => acidDeath,
                DeathType.Infection => infectionDeath,
                DeathType.Splash => splashDeath,
                _ => deathPoison
            };
            if (deathType == DeathType.Swamp)
            {
                swampDeathAnimation.gameObject.SetActive(true);
                meshRenderer.enabled = false;
                await swampDeathAnimation.PlaySwampDeath();
                return;
            }

            StopAllAnimation();
            await UniTaskUtils.PlayAnimation(skeletonAnimation, deathEyes, 1, true, 0.7f);
            await UniTaskUtils.PlayAnimation(skeletonAnimation, death, 1);
            
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

        public void StopAllAnimation()
        {
            skeletonAnimation.AnimationState.ClearTracks();
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
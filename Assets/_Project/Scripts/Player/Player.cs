using System;
using Cysharp.Threading.Tasks;
using Global;
using Spine.Unity;
using UnityEngine;

namespace _Project.Player
{
    [RequireComponent(typeof(Rigidbody2D) )]
    public class Player: MonoBehaviour
    {
        [SerializeField] private PlayerAnimation playerAnimation;
        [SerializeField] private MovementDetector movementDetector;
        
        public MovementDetector MovementDetector => movementDetector;
        
        private Rigidbody2D rb;
        private GameController gameManager => GameController.Instance;

        public PlayerAnimation PlayerAnimation => playerAnimation;

        private bool isDying;
        
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            gameManager.OnPlayerDeath += GameManager_OnPlayerDeath;
        }

        private void GameManager_OnPlayerDeath(DeathType death)
        {
            PlayerDeath(death).Forget();
        }

        private async UniTask PlayerDeath(DeathType death)
        {
            if (isDying)
                return;
            
            isDying = true;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            await playerAnimation.PlayDeath(death);
            gameManager.PlayerDeathAnimationFinished();
        }

        private void OnDestroy()
        {
            if (gameManager)
                gameManager.OnPlayerDeath -= GameManager_OnPlayerDeath;
        }
        
        public void Push(Vector2 force)
        {
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}
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
        
        private Rigidbody2D rb;
        private GameController gameManager => GameController.Instance;

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
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.linearVelocity = Vector2.zero;
            await playerAnimation.PlayDeath(death);
            gameManager.PlayerDeathAnimationFinished();
        }

        private void OnDestroy()
        {
            if (gameManager!=null)
                gameManager.OnPlayerDeath -= GameManager_OnPlayerDeath;
        }
    }
}
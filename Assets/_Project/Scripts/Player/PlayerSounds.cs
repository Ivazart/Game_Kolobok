using System;
using Global;
using UnityEngine;

namespace _Project.Player
{
    public class PlayerSounds : MonoBehaviour
    {
        [SerializeField] private AudioClip deathSoundPoison;
        [SerializeField] private AudioClip deathSoundFire;
        [SerializeField] private AudioClip deathSoundSwamp;

        private AudioManager audioManager => AudioManager.Instance;
        private GameController gameController => GameController.Instance;

        private void Start()
        {
            gameController.OnPlayerDeath += OnPlayerDeath;
        }

        private void OnPlayerDeath(DeathType deathType)
        {
            AudioClip deathSound = deathType switch
            {
                DeathType.Poison => deathSoundPoison,
                DeathType.Fire => deathSoundFire,
                DeathType.Swamp => deathSoundSwamp,
                _ => throw new ArgumentOutOfRangeException(nameof(deathType), deathType, null)
            };

            if (deathSound != null)
                audioManager.PlaySFX(deathSound);
        }

        private void OnDestroy()
        {
            try
            {
                gameController.OnPlayerDeath -= OnPlayerDeath;
            }
            catch 
            {
                // ignored
            }
        }
    }
}
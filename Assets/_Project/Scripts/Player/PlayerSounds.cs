using System;
using Cysharp.Threading.Tasks;
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
        private bool isPlaying;
        
        private void Start()
        {
            gameController.OnPlayerDeath += OnPlayerDeath;
            isPlaying = false;
        }

        private void OnPlayerDeath(DeathType deathType)
        {
            if (isPlaying)
                return;
            
            AudioClip deathSound = deathType switch
            {
                DeathType.Poison => deathSoundPoison,
                DeathType.Fire => deathSoundFire,
                DeathType.Swamp => deathSoundSwamp,
                DeathType.Infection => deathSoundFire,
                DeathType.Acid => deathSoundFire,
                DeathType.Splash => deathSoundFire,
                _ => throw new ArgumentOutOfRangeException(nameof(deathType), deathType, null)
            };

            if (deathSound != null)
            {
                isPlaying = true;
                audioManager.PlaySFX(deathSound);
            }
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
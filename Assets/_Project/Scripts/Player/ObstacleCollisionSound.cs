using System;
using System.Collections.Generic;
using Global;
using UnityEngine;

namespace _Project.Player
{
    [RequireComponent(typeof(CollisionLogic))]
    public class ObstacleCollisionSound : MonoBehaviour
    {
        [Serializable]
        public struct SceneSound
        {
            public SceneName scene;
            public AudioClip clip;
        }

        [SerializeField] private SceneSound[] sceneSounds;
        [SerializeField] private List<string> obstacleTags;
        [SerializeField] private float minRelativeVelocity = 0.3f;
        [SerializeField] private float soundInterval = 0.2f;      // минимальное время между звуками в пределах одного прыжка
        
        private int remainingSounds = 3;          // сколько звуков ещё можно проиграть после текущего прыжка
        private float lastSoundTime = -10f;       // время последнего проигранного звука столкновения
        private CollisionLogic collision;
        
        // Громкости: 1-й звук -> 1f, 2-й -> 0.5f, 3-й -> 0.2f
        private static readonly float[] Volumes = { 1f, 0.5f, 0.2f };
        private GameController gameController => GameController.Instance;
        private SceneController sceneController => SceneController.Instance;
        private AudioManager audioManager => AudioManager.Instance;

        private void Start()
        {
            collision = GetComponent<CollisionLogic>();
            gameController.OnDragEnded += OnJump;
            gameController.OnPlayerDeath += OnDeath;
            collision.OnEnterPlayerSolidEnemySolid += CollisionEnterHandler;
        }

        private void OnDestroy()
        {
            try
            {
                gameController.OnDragEnded -= OnJump;
                gameController.OnPlayerDeath -= OnDeath;
            }
            catch { }
        }

        private void OnJump()
        {
            remainingSounds = 3;
        }

        private void OnDeath(DeathType deathType)
        {
            remainingSounds = 3;
        }

        private void CollisionEnterHandler(CollisionEventData data)
        {
            Collision2D collisionData = data.FullCollision;
            
            if (collisionData.relativeVelocity.magnitude < minRelativeVelocity)
                return;

            if (!HasObstacleTag(collisionData.gameObject))
                return;

            // Проверяем, не слишком ли часто
            if (Time.time - lastSoundTime < soundInterval)
                return;

            if (remainingSounds <= 0)
                return;

            // Вычисляем громкость в зависимости от того, какой по счёту звук (3, 2, 1)
            int soundIndex = 3 - remainingSounds; // 0, 1, 2
            float volume = Volumes[soundIndex];

            if (PlaySceneSound(volume))
            {
                remainingSounds--;
                lastSoundTime = Time.time;
            }
        }

        private bool HasObstacleTag(GameObject obj)
        {
            foreach (string tag in obstacleTags)
            {
                if (obj.CompareTag(tag))
                    return true;
            }
            return false;
        }

        private bool PlaySceneSound(float volume)
        {
            if (sceneController == null) return false;

            SceneName currentScene = sceneController.CurrentScene;

            foreach (var ss in sceneSounds)
            {
                if (ss.scene == currentScene)
                {
                    if (ss.clip != null)
                    {
                        audioManager.PlaySFX(ss.clip, volume);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
using System;
using System.Collections.Generic;
using Global;
using UnityEngine;

namespace _Project.Player
{
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
        [SerializeField] private float exitCooldown = 0.2f;
        [SerializeField] private float globalCooldown = 2f;   // минимальный интервал между любыми звуками столкновений

        private int contactCount;
        private float lastZeroTime = -10f;
        private float lastSoundTime = -10f;

        private SceneController sceneController => SceneController.Instance;
        private AudioManager audioManager => AudioManager.Instance;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.relativeVelocity.magnitude < minRelativeVelocity)
                return;

            if (!HasObstacleTag(collision.gameObject))
                return;

            bool wasZero = (contactCount == 0);
            contactCount++;

            if (wasZero && Time.time - lastZeroTime > exitCooldown)
            {
                if (Time.time - lastSoundTime >= globalCooldown)
                {
                    lastSoundTime = Time.time;
                    PlaySceneSound();
                }
            }
        }

        private void OnCollisionExit2D(Collision2D collision)
        {
            if (!HasObstacleTag(collision.gameObject))
                return;

            contactCount--;
            if (contactCount <= 0)
            {
                contactCount = 0;
                lastZeroTime = Time.time;
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

        private void PlaySceneSound()
        {
            if (sceneController == null) return;

            SceneName currentScene = sceneController.CurrentScene;

            foreach (var ss in sceneSounds)
            {
                if (ss.scene == currentScene)
                {
                    if (ss.clip != null)
                        audioManager.PlaySFX(ss.clip);
                    return;
                }
            }
        }
    }
}
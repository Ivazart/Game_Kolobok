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
        [SerializeField] private float cooldown = 0.5f; // секунд между повторными звуками с одного коллайдера

        private readonly Dictionary<Collider2D, float> lastCollisionTimes = new();
        private SceneController sceneController => SceneController.Instance;
        private AudioManager audioManager => AudioManager.Instance;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            foreach (string obstacleTag in obstacleTags)
            {
                if (collision.gameObject.CompareTag(obstacleTag))
                {
                    if (CanPlaySound(collision.collider))
                        PlaySceneSound();
                    break; // достаточно одного совпадения
                }
            }
        }

        private bool CanPlaySound(Collider2D collider)
        {
            if (collider == null) return false;
            float now = Time.time;
            if (lastCollisionTimes.TryGetValue(collider, out float lastTime))
            {
                if (now - lastTime < cooldown)
                    return false;
            }
            lastCollisionTimes[collider] = now;
            return true;
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

        // Опционально: очистка старых записей, если коллайдеры уничтожаются/переиспользуются
        private void OnDisable() => lastCollisionTimes.Clear();
    }
}
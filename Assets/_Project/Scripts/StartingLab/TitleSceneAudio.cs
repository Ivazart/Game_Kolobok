using UnityEngine;
using Global;

namespace StartingLab
{
    public class TitleSceneAudio : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip labAmbientSound;
        [SerializeField] private AudioClip alarmSound;

        private AudioManager audioManager => AudioManager.Instance;
        private AudioSource labAmbientSource;
        private AudioSource alarmSource;

        /// <summary> Запускает зацикленный шум лаборатории. </summary>
        public void PlayLabAmbient()
        {
            if (labAmbientSound != null)
                labAmbientSource = audioManager.PlaySFXLoop(labAmbientSound);
        }

        /// <summary> Запускает зацикленный звук тревоги. </summary>
        public void PlayAlarm()
        {
            if (alarmSound != null)
                alarmSource = audioManager.PlaySFXLoop(alarmSound);
        }

        /// <summary> Останавливает и удаляет все фоновые звуки сцены. </summary>
        public void StopAll()
        {
            StopLabAmbient();
            StopAlarm();
        }

        public void StopLabAmbient()
        {
            if (labAmbientSource != null)
            {
                audioManager.StopSFXLoop(labAmbientSource);
                labAmbientSource = null;
            }
        }

        public void StopAlarm()
        {
            if (alarmSource != null)
            {
                audioManager.StopSFXLoop(alarmSource);
                alarmSource = null;
            }
        }

        private void OnDestroy()
        {
            // Гарантированная очистка при выгрузке сцены
            StopAll();
        }
    }
}
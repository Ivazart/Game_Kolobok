using UnityEngine;
using UnityEngine.Audio;

namespace Global
{
    public class AudioManager : SingletonBase<AudioManager>
    {
        [Header("Mixer")]
        [SerializeField] private AudioMixer audioMixer;
        [SerializeField] private string masterVolumeParam = "MasterVolume";
        [SerializeField] private string musicVolumeParam = "MusicVolume";
        [SerializeField] private string sfxVolumeParam = "SFXVolume";

        [Header("Mixer Groups (drag here)")]
        [SerializeField] private AudioMixerGroup musicGroup;
        [SerializeField] private AudioMixerGroup sfxGroup;

        [Header("Defaults (0..1)")]
        [SerializeField] private float defaultMasterVolume = 1f;
        [SerializeField] private float defaultMusicVolume = 0.8f;
        [SerializeField] private float defaultSFXVolume = 1f;

        private const string MasterVolumeKey = "MasterVolume";
        private const string MusicVolumeKey = "MusicVolume";
        private const string SFXVolumeKey = "SFXVolume";

        // Источники звука
        private AudioSource musicSource;
        private AudioSource sfxSource;  // для коротких звуков (можно позже сделать пул)

        public float MasterVolume
        {
            get => PlayerPrefs.GetFloat(MasterVolumeKey, defaultMasterVolume);
            set
            {
                PlayerPrefs.SetFloat(MasterVolumeKey, Mathf.Clamp01(value));
                ApplyMasterVolume();
            }
        }

        public float MusicVolume
        {
            get => PlayerPrefs.GetFloat(MusicVolumeKey, defaultMusicVolume);
            set
            {
                PlayerPrefs.SetFloat(MusicVolumeKey, Mathf.Clamp01(value));
                ApplyMusicVolume();
            }
        }

        public float SFXVolume
        {
            get => PlayerPrefs.GetFloat(SFXVolumeKey, defaultSFXVolume);
            set
            {
                PlayerPrefs.SetFloat(SFXVolumeKey, Mathf.Clamp01(value));
                ApplySFXVolume();
            }
        }

        protected override void Awake()
        {
            base.Awake();

            // Создаём и настраиваем AudioSource для музыки
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.outputAudioMixerGroup = musicGroup;
            musicSource.loop = true;
            musicSource.playOnAwake = false;

            // Создаём AudioSource для звуковых эффектов
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.outputAudioMixerGroup = sfxGroup;
            sfxSource.playOnAwake = false;

            ApplyAllVolumes();
        }

        /// <summary> Воспроизведение музыки (зациклено по умолчанию). </summary>
        public void PlayMusic(AudioClip clip)
        {
            if (clip == null) return;
            if (musicSource.clip == clip && musicSource.isPlaying) return;
            musicSource.Stop();
            musicSource.clip = clip;
            musicSource.Play();
        }

        /// <summary> Остановка музыки. </summary>
        public void StopMusic()
        {
            if (musicSource != null)
                musicSource.Stop();
        }

        /// <summary> Воспроизведение звукового эффекта (не перебивает другие звуки). </summary>
        public void PlaySFX(AudioClip clip)
        {
            if (clip == null)
            {
                Debug.LogError("AudioManager: PlaySFX called with null clip.");
                return;
            }
            sfxSource.PlayOneShot(clip);  // позволяет накладывать звуки друг на друга
        }
        
        /// <summary>
        /// Создаёт и запускает зацикленный SFX. Возвращает источник, чтобы потом остановить.
        /// </summary>
        public AudioSource PlaySFXLoop(AudioClip clip)
        {
            if (clip == null) return null;

            var source = gameObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = true;
            source.outputAudioMixerGroup = sfxGroup;
            source.Play();
            return source;
        }

        /// <summary>
        /// Останавливает и удаляет источник, созданный через PlaySFXLoop.
        /// </summary>
        public void StopSFXLoop(AudioSource source)
        {
            if (source != null)
            {
                source.Stop();
                Destroy(source);
            }
        }

        // Управление громкостью (вызывается из UI)
        public void SetMasterVolume(float value) => MasterVolume = value;
        public void SetMusicVolume(float value) => MusicVolume = value;
        public void SetSFXVolume(float value) => SFXVolume = value;

        // Применение громкости к микшеру
        private void ApplyAllVolumes()
        {
            ApplyMasterVolume();
            ApplyMusicVolume();
            ApplySFXVolume();
        }

        private void ApplyMasterVolume() => SetMixerVolume(masterVolumeParam, MasterVolume);
        private void ApplyMusicVolume() => SetMixerVolume(musicVolumeParam, MusicVolume);
        private void ApplySFXVolume() => SetMixerVolume(sfxVolumeParam, SFXVolume);

        private void SetMixerVolume(string paramName, float normalizedValue)
        {
            if (audioMixer == null) return;

            // Не даём уйти в минус бесконечность (при 0)
            float clamped = Mathf.Max(0.0001f, normalizedValue);
            // Перевод в децибелы с логарифмической шкалой: 1.0 → 0 dB, 0.0001 → -80 dB
            float db = 20f * Mathf.Log10(clamped);
            audioMixer.SetFloat(paramName, db);
        }
    }
}
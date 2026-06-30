using UnityEngine;
using Global;

namespace _Project.UI
{
    public class VolumeSettingsUI : MonoBehaviour
    {
        [SerializeField] private SoundBar masterBar;
        [SerializeField] private SoundBar musicBar;
        [SerializeField] private SoundBar sfxBar;

        private AudioManager audioManager;

        private void Start()
        {
            audioManager = AudioManager.Instance;

            masterBar.Init(audioManager.MasterVolume, v => audioManager.SetMasterVolume(v));
            musicBar.Init(audioManager.MusicVolume, v => audioManager.SetMusicVolume(v));
            sfxBar.Init(audioManager.SFXVolume, v => audioManager.SetSFXVolume(v));
        }
    }
}
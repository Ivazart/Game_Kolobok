using UnityEngine;
using UnityEngine.UI;
using Global;

namespace _Project.UI
{
    public class VolumeSettingsUI : MonoBehaviour
    {
        [SerializeField] private Slider masterSlider;
        [SerializeField] private Slider musicSlider;
        [SerializeField] private Slider sfxSlider;

        private AudioManager audioManager;

        private void Start()
        {
            audioManager = AudioManager.Instance;
            
            masterSlider.value = audioManager.MasterVolume;
            musicSlider.value = audioManager.MusicVolume;
            sfxSlider.value = audioManager.SFXVolume;
            
            masterSlider.onValueChanged.AddListener(audioManager.SetMasterVolume);
            musicSlider.onValueChanged.AddListener(audioManager.SetMusicVolume);
            sfxSlider.onValueChanged.AddListener(audioManager.SetSFXVolume);
        }

        private void OnDestroy()
        {
            if (audioManager != null)
            {
                masterSlider.onValueChanged.RemoveListener(audioManager.SetMasterVolume);
                musicSlider.onValueChanged.RemoveListener(audioManager.SetMusicVolume);
                sfxSlider.onValueChanged.RemoveListener(audioManager.SetSFXVolume);
            }
        }
    }
}
using UnityEngine;
using Global;

public class SceneMusic : MonoBehaviour
{
    [SerializeField] private AudioClip musicClip;

    private void Start()
    {
        if (musicClip != null)
            AudioManager.Instance.PlayMusic(musicClip);
    }

    private void OnDestroy()
    {
        if (AudioManager.Instance != null)
            AudioManager.Instance.StopMusic();
    }
}
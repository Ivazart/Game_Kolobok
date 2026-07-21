using Global;
using UnityEngine;

namespace _Project.Player
{
    public class PlayerDragSounds : MonoBehaviour
    {
        [Header("Audio Clips")]
        [SerializeField] private AudioClip stretchClip;
        [SerializeField] private AudioClip jumpClip;   

        [Header("Stretch Settings")]
        [SerializeField] private float minPitch = 0.8f;
        [SerializeField] private float maxPitch = 1.5f;
        [SerializeField] private float maxForceForPitch = 14f;
        [SerializeField] private float minForceChange = 0.1f;
        [SerializeField] private float stretchSilenceDelay = 0.05f;
        [Range(0f, 1f)]
        [SerializeField] private float stretchVolume = 1f;

        [Header("Jump Settings")]
        [Range(0f, 1f)]
        [SerializeField] private float jumpVolume = 1f;

        private DragHandler dragHandler;
        private Player player;
        private AudioManager audioManager => AudioManager.Instance;
        private GameController gameController => GameController.Instance;

        private AudioSource stretchSource;

        private Vector2 lastForce;
        private float stretchSilenceTimer;

        private void Start()
        {
            player = GetComponent<GameManager>().GetPlayer();
            dragHandler = GetComponent<DragHandler>();

            if (dragHandler == null || player == null)
            {
                Debug.LogError($"Missing components on {gameObject.name}!");
                enabled = false;
                return;
            }

            dragHandler.OnDragStarted += OnDragStarted;
            dragHandler.OnDragEnded += OnDragEnded;
            if (gameController != null)
                gameController.OnPlayerDeath += OnPlayerDeath;
        }

        private void Update()
        {
            if (dragHandler == null || player == null) return;

            if (dragHandler.IsDragging)
            {
                Vector2 currentForce = dragHandler.CurrentForce;
                float change = (currentForce - lastForce).magnitude;

                if (change > minForceChange)
                {
                    if (stretchSource == null && stretchClip != null)
                        stretchSource = audioManager.PlaySFXLoop(stretchClip, stretchVolume);

                    if (stretchSource != null)
                    {
                        float t = Mathf.Clamp01(currentForce.magnitude / maxForceForPitch);
                        stretchSource.pitch = Mathf.Lerp(minPitch, maxPitch, t);
                    }

                    lastForce = currentForce;
                    stretchSilenceTimer = 0f;
                }
                else
                {
                    stretchSilenceTimer += Time.deltaTime;
                    if (stretchSilenceTimer >= stretchSilenceDelay && stretchSource != null)
                    {
                        audioManager.StopSFXLoop(stretchSource);
                        stretchSource = null;
                    }
                }
            }
        }

        private void OnDragStarted()
        {
            lastForce = Vector2.zero;
            stretchSilenceTimer = 0f;
        }

        private void OnDragEnded()
        {
            if (stretchSource != null)
            {
                audioManager.StopSFXLoop(stretchSource);
                stretchSource = null;
            }

            if (jumpClip != null)
                audioManager.PlaySFX(jumpClip, jumpVolume);
        }

        private void OnPlayerDeath(DeathType deathType)
        {
            StopAllSounds();
        }

        private void StopAllSounds()
        {
            if (stretchSource != null)
            {
                audioManager.StopSFXLoop(stretchSource);
                stretchSource = null;
            }
        }

        private void OnDestroy()
        {
            try{
            if (dragHandler != null)
            {
                dragHandler.OnDragStarted -= OnDragStarted;
                dragHandler.OnDragEnded -= OnDragEnded;
            }
            if (gameController != null)
                gameController.OnPlayerDeath -= OnPlayerDeath;
            }
            catch
            {
                // ignored
            }

            StopAllSounds();
        }
    }
}
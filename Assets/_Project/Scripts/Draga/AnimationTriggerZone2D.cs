using UnityEngine;

public class AnimationTriggerZone2D : MonoBehaviour
{
    [Header("Кто может активировать")]
    public string activatorTag = "Player";

    [Header("Объект с анимацией")]
    public SpineAnimationPlayer targetAnimationPlayer;

    [Header("Какую анимацию запустить")]
    public string actionAnimation = "action";

    [Header("Настройки")]
    public bool triggerOnce = false;

    private bool wasTriggered;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggerOnce && wasTriggered)
            return;

        if (!other.CompareTag(activatorTag))
            return;

        if (targetAnimationPlayer == null)
            return;

        wasTriggered = true;
        targetAnimationPlayer.PlayAnimationOnce(actionAnimation);
    }
}


using UnityEngine;
using Spine;
using Spine.Unity;

[RequireComponent(typeof(SkeletonAnimation))]
public class SpineAnimationPlayer : MonoBehaviour
{
    [Header("Idle-анимация")]
    public string idleAnimation = "idle";

    [Header("Настройки")]
    public int trackIndex = 0;

    private SkeletonAnimation skeletonAnimation;
    private bool isPlayingAction;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    private void Start()
    {
        PlayIdle();
    }

    public void PlayAnimationOnce(string animationName)
    {
        if (string.IsNullOrEmpty(animationName))
            return;

        if (isPlayingAction)
            return;

        isPlayingAction = true;

        TrackEntry entry = skeletonAnimation.AnimationState.SetAnimation(
            trackIndex,
            animationName,
            false
        );

        entry.Complete += OnActionComplete;
    }

    private void OnActionComplete(TrackEntry entry)
    {
        entry.Complete -= OnActionComplete;

        isPlayingAction = false;
        PlayIdle();
    }

    public void PlayIdle()
    {
        skeletonAnimation.AnimationState.SetAnimation(
            trackIndex,
            idleAnimation,
            true
        );
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using StartingLab;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(SkeletonAnimation))]
[RequireComponent(typeof(SpineAnimationFader))]
public class PlayButton : MonoBehaviour , IPointerClickHandler
{
    public event Action OnButtonClicked;
    
    private SpineAnimationFader spineFader;
    private SkeletonAnimation buttonSpineAnimation;
    private bool isPressed;
    private StartingLabState currentState = StartingLabState.Idle;
    private enum AnimationNames
    {
        n_active,
        alarme,
        press
    }
    
    private void Awake()
    {
        buttonSpineAnimation = GetComponent<SkeletonAnimation>();
        spineFader = GetComponent<SpineAnimationFader>();
    }
    
    public void SetLabState(StartingLabState state)
    {
        currentState = state;
        AnimationNames animationName = state switch
        {
            StartingLabState.Alarm => AnimationNames.alarme,
            StartingLabState.Active => AnimationNames.n_active,
            StartingLabState.Idle => AnimationNames.n_active,
            _ => throw new ArgumentOutOfRangeException(nameof(state), state, null)
        };

        TryPlayAnimation(animationName);
    }
    
    public void OnPointerClick(PointerEventData eventData)
    {
        if (isPressed == false && currentState == StartingLabState.Alarm)
        {
            MouseDownHandler().Forget();
        }
    }

    private async UniTask MouseDownHandler()
    {
        TryPlayAnimation(AnimationNames.press, false);
        isPressed = true;
        UniTask.Post(() => OnButtonClicked?.Invoke());
        //await UniTask.WaitForSeconds(.3f);
        await spineFader.FadeOutSpine(.7f);
    }
    
    private void TryPlayAnimation(AnimationNames animationName, bool loop = true)
    {
        Spine.Animation anim = buttonSpineAnimation.Skeleton.Data.FindAnimation(animationName.ToString());

        if (anim == null)
        {
            Debug.LogError($"Animation '{animationName}' not found in {gameObject.name}!");
        }
        else
        {
            Debug.Log($" {gameObject.name} is playing animation {animationName}");
            buttonSpineAnimation.AnimationState.SetAnimation(0, anim, loop);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using StartingLab;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(SkeletonAnimation))]
public class Button : MonoBehaviour 
{
    public event Action OnButtonClicked;
    
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
    
    private void OnMouseDown()
    {
        if (isPressed == false && currentState == StartingLabState.Alarm)
        {
            TryPlayAnimation(AnimationNames.press);
            isPressed = true;
            OnButtonClicked?.Invoke();
        }
    }
    
    private void TryPlayAnimation(AnimationNames animationName)
    {
        Spine.Animation anim = buttonSpineAnimation.Skeleton.Data.FindAnimation(animationName.ToString());

        if (anim == null)
        {
            Debug.LogError($"Animation '{animationName}' not found in {gameObject.name}!");
        }
        else
        {
            Debug.Log($" {gameObject.name} is playing animation {animationName}");
            buttonSpineAnimation.AnimationState.SetAnimation(0, anim, true);
        }
    }

 










}

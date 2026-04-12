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
    
    private void Awake()
    {
        buttonSpineAnimation = GetComponent<SkeletonAnimation>();
    }

    public void SetLabState(StartingLabState state)
    {
        currentState = state;
        if (state == StartingLabState.Alarm)
            buttonSpineAnimation.AnimationState.SetAnimation(0, "alarme", false);
    }

    private void OnMouseDown()
    {
        if (isPressed == false && currentState == StartingLabState.Alarm)
        {
            buttonSpineAnimation.AnimationState.SetAnimation(0, "press", false);
            isPressed = true;
            OnButtonClicked?.Invoke();
        }
    }










}

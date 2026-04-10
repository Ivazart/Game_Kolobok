using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

[RequireComponent(typeof(SkeletonAnimation))]
public class Button : MonoBehaviour 
{
    [SerializeField] private TitleGameManager gm;
    
    private SkeletonAnimation buttonSpineAnimation;
    private bool isPressed = false;
    private bool isAlarmed = false;

    private void Awake()
    {
        buttonSpineAnimation = GetComponent<SkeletonAnimation>();
    }

    private void OnMouseDown()
    {
        if (isPressed == false && isAlarmed)
        {
            gm.Active();
            buttonSpineAnimation.AnimationState.SetAnimation(0, "press", false);
            isPressed = true;
            gm.Space();
        }
    }
    
    public void Alarm()
    {
        buttonSpineAnimation.AnimationState.SetAnimation(0, "alarme", false);
        isAlarmed = true;
    }









}

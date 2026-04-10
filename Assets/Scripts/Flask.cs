using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class Flask : MonoBehaviour
{
    public SkeletonAnimation fl;
    public AudioSource process;
    
    public bool alarm;
    
    public void alarm_anim()
    {
        fl.AnimationState.SetAnimation(0, "alarm", true);
    }

    public void Activate()
    {
        Invoke(nameof(SetActivate), 2);
    }
    
    public void SetActivate()
    {
        fl.AnimationState.SetAnimation(0, "activate", true);
        process.Play();
    }
}

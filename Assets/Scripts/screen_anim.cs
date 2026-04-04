using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class screen_anim : MonoBehaviour
{

    public SkeletonAnimation screen;
    public AnimationReferenceAsset al;
    public AnimationReferenceAsset act;
    public bool alarm;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }
    public void alarm_anim()
    {
        screen.AnimationState.SetAnimation(0, "alarm", true);
    }

    public void Activ()
    {
        Invoke("SetActivate", 2);
        
    }
    public void SetActivate()
    {
        screen.AnimationState.SetAnimation(0, "activate", true);
    }

}

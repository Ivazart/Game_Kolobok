using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class man_anim : MonoBehaviour
{
    public SkeletonAnimation man;
    public AnimationReferenceAsset cp;
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
        man.AnimationState.SetAnimation(0, "alarm", true);
    }
    public void activ_anim()
    {
        man.AnimationState.SetAnimation(0, "activate", false);
    }
}

using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class flask : MonoBehaviour
{

    public SkeletonAnimation fl;

    public bool alarm;
    public AudioSource process;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }
    public void alarm_anim()
    {
        fl.AnimationState.SetAnimation(0, "alarm", true);
    }

    public void Activ()
    {
        Invoke("SetActivate", 2);

    }
    public void SetActivate()
    {
       fl.AnimationState.SetAnimation(0, "activate", true);
        process.Play();
    }
}

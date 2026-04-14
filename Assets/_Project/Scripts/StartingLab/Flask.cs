using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using StartingLab;
using UnityEngine;

public class Flask : StartingLabAnimation
{
    public AudioSource process;
    
    public override void SetState(StartingLabState state)
    {
        base.SetState(state);
        if (state == StartingLabState.Active)
            process.Play();
    }
}

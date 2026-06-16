using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Spine.Unity;
using StartingLab;
using UnityEngine;

public class Flask : StartingLabAnimation
{
    public AudioSource process;
    
    public override void SetState(StartingLabState state)
    {
        SetStateAsync(state).Forget();
    }

    private async UniTask SetStateAsync(StartingLabState state)
    {
        if (state == StartingLabState.Active)
        {
            await UniTask.WaitForSeconds(2f);
        }
        base.SetState(state);
        if (state == StartingLabState.Active)
            process.Play();
    }
    
}

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StartingLab
{
    public class StartingLabManAnimation : StartingLabAnimation
    {
        public event Action OnManClickButtonAnimFinish; 
        
        public override void SetState(StartingLabState state)
        {
            if (state != StartingLabState.Active)
                base.SetState(state);
           
            ClickButtonAnim().Forget();
        }

        private async UniTask ClickButtonAnim()
        {
            await PlayAnimation(ActiveAnimationName, false);
            base.SetState(StartingLabState.Idle);
            OnManClickButtonAnimFinish?.Invoke();
        }
    }
}
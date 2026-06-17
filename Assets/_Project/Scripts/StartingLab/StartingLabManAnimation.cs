using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace StartingLab
{
    public class StartingLabManAnimation : StartingLabAnimation
    {
        public event Action OnManClickButtonAnimFinish;

        private bool hasPlayedClick; // гарантирует однократное воспроизведение

        public override void SetState(StartingLabState state)
        {
            if (state == StartingLabState.Active && !hasPlayedClick)
            {
                hasPlayedClick = true;
                ClickButtonAnim().Forget();
                return;
            }

            base.SetState(state);
        }

        private async UniTask ClickButtonAnim()
        {
            await PlayAnimation(ActiveAnimationName, false);
            base.SetState(StartingLabState.Idle);
            OnManClickButtonAnimFinish?.Invoke();
        }
    }
}
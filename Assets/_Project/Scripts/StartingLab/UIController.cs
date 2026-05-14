using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace StartingLab
{
    public class UIController : MonoBehaviour
    {
        [Header("Fade settings")]
        [SerializeField] private SpriteRenderer[] spritesToFade; // Перетащите сюда ваши спрайты в инспекторе
        [SerializeField] private float duration = 1.5f;         // Длительность исчезновения

        [Header("Movement settings")]
        [SerializeField] private Transform[] objectsToMove; // Поместите сюда 2 объекта
        [SerializeField] private float moveDuration = 1.0f;
        [SerializeField] private float targetY = 2.0f;
        
        public void SetState(StartingLabState state)
        {
            if (state == StartingLabState.Active)
            {
                SetActiveState().Forget();
            }
        }

        private async UniTask SetActiveState()
        {
            await FadeOutSprites();
            await MoveGameObjects();
        }
        
        // Вызовите этот метод, когда нужно скрыть спрайты
        private async UniTask FadeOutSprites()
        {
            // Создаем последовательность (Sequence)
            Sequence mySequence = DOTween.Sequence();

            foreach (SpriteRenderer sprite in spritesToFade)
            {
                if (sprite != null)
                {
                    // Добавляем анимацию прозрачности (альфы) до 0
                    // Join заставляет все анимации запускаться одновременно
                    _= mySequence.Join(sprite.DOFade(0f, duration));
                }
            }
            await mySequence.ToUniTask();
        }

        private async UniTask MoveGameObjects()
        {
            Sequence mySequence = DOTween.Sequence();
            foreach (Transform obj in objectsToMove)
            {
                if (obj != null)
                {
                    // Используем DOMoveY для изменения только одной оси
                    // Append гарантирует, что это начнется ПОСЛЕ завершения Fade
                    _= mySequence.Join(obj.DOMoveY(targetY, moveDuration).SetEase(Ease.OutQuad));
                }
            }
            await mySequence.ToUniTask();
        }
    }
    
}
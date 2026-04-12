using DG.Tweening;
using UnityEngine;

namespace StartingLab
{
    public class UIController : MonoBehaviour
    {
        [Header("Fade settings")]
        [SerializeField] private SpriteRenderer[] spritesToFade; // Перетащите сюда ваши спрайты в инспекторе
        [SerializeField] private float duration = 1.5f;         // Длительность исчезновения
        [SerializeField] private GameObject playButton;
        
        [Header("Movement settings")]
        [SerializeField] private Transform[] objectsToMove; // Поместите сюда 2 объекта
        [SerializeField] private float moveDuration = 1.0f;
        [SerializeField] private float targetY = 2.0f;
        
        public void SetState(StartingLabState state)
        {
           
            if (state == StartingLabState.Active)
            {
                Sequence mySequence = FadeOutSprites();
                
            }
            
        }
        // Вызовите этот метод, когда нужно скрыть спрайты
        private Sequence FadeOutSprites()
        {
            // Создаем последовательность (Sequence)
            Sequence mySequence = DOTween.Sequence();

            foreach (SpriteRenderer sprite in spritesToFade)
            {
                if (sprite != null)
                {
                    // Добавляем анимацию прозрачности (альфы) до 0
                    // Join заставляет все анимации запускаться одновременно
                    mySequence.Join(sprite.DOFade(0f, duration));
                }
            }
            // 2. После исчезновения сдвигаем объекты по Y (одновременно друг с другом)

        
            // Дополнительно: можно что-то сделать по завершении
            mySequence.OnComplete(() => Debug.Log("All sprites hided!"));
            playButton.SetActive(false);
            return mySequence;
        }

        private Sequence MoveGameObjects()
        {
            Sequence mySequence = DOTween.Sequence();
            foreach (Transform obj in objectsToMove)
            {
                if (obj != null)
                {
                    // Используем DOMoveY для изменения только одной оси
                    // Append гарантирует, что это начнется ПОСЛЕ завершения Fade
                    mySequence.Join(obj.DOMoveY(targetY, moveDuration).SetEase(Ease.OutQuad));
                }
            }

            return mySequence;
        }
    }
    
}
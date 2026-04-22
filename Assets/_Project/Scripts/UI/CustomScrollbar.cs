using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace _Project.UI
{
    public class CustomScrollbar : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        [Header("Components")] 
        [SerializeField] private ScrollRect targetScrollRect;
        [SerializeField] private RectTransform handleRect;
        [SerializeField] private RectTransform backgroundRect;

        [Header("Settings")] 
        [SerializeField] private Direction direction = Direction.Vertical; // Вертикальный или горизонтальный
        [SerializeField] private bool invertDirection;

        private float scrollLength; // Длина доступного хода ручки (высота/ширина фона минус размер ручки)
        private float handleSize; // Размер ручки (высота для вертикального, ширина для горизонтального)

        private bool isDragging;

        private enum Direction
        {
            Vertical,
            Horizontal
        }
        
        private void Start()
        {
            if (targetScrollRect == null)
            {
                Debug.LogError("Target ScrollRect is not assigned!", this);
                enabled = false;
                return;
            }

            if (handleRect == null || backgroundRect == null)
            {
                Debug.LogError("Handle or Background RectTransform is not assigned!", this);
                enabled = false;
                return;
            }

            // Подписываемся на изменение скролла
            targetScrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);

            // Вычисляем начальные размеры
            CalculateSizes();

            // Устанавливаем начальную позицию ручки
            UpdateHandlePositionFromScrollRect();
        }

        private void OnDestroy()
        {
            if (targetScrollRect != null)
                targetScrollRect.onValueChanged.RemoveListener(OnScrollRectValueChanged);
        }

        // Вызывается при изменении размеров UI (например, при повороте экрана)
        private void OnRectTransformDimensionsChange()
        {
            CalculateSizes();
            UpdateHandlePositionFromScrollRect();
        }

        private void CalculateSizes()
        {
            if (direction == Direction.Vertical)
            {
                handleSize = handleRect.rect.height;
                scrollLength = backgroundRect.rect.height - handleSize;
            }
            else
            {
                handleSize = handleRect.rect.width;
                scrollLength = backgroundRect.rect.width - handleSize;
            }
        }

        // Вызывается, когда двигается сам ScrollRect
        private void OnScrollRectValueChanged(Vector2 normalizedPosition)
        {
            if (isDragging) return; // Не обновляем позицию ручки, пока пользователь её тащит

            UpdateHandlePositionFromScrollRect();
        }

        private void UpdateHandlePositionFromScrollRect()
        {
            float normalized;
            if (direction == Direction.Vertical)
                normalized = targetScrollRect.verticalNormalizedPosition;
            else
                normalized = targetScrollRect.horizontalNormalizedPosition;

            // Принудительно ограничиваем диапазон [0, 1]
            normalized = Mathf.Clamp01(normalized);

            if (invertDirection)
                normalized = 1f - normalized;

            SetHandleNormalizedPosition(normalized);
        }

        private void SetHandleNormalizedPosition(float normalized)
        {
            // Дополнительная страховка от некорректных значений
            normalized = Mathf.Clamp01(normalized);

            if (scrollLength <= 0) return;

            float pos = normalized * scrollLength;

            if (direction == Direction.Vertical)
                handleRect.anchoredPosition = new Vector2(0, pos);
            else
                handleRect.anchoredPosition = new Vector2(pos, 0);
        }

        private float GetHandleNormalizedPosition()
        {
            if (scrollLength <= 0) return 0;

            float currentPos = direction == Direction.Vertical ? 
                handleRect.anchoredPosition.y : 
                handleRect.anchoredPosition.x;

            return Mathf.Clamp01(currentPos / scrollLength);
        }

        // Обработка перетаскивания ручки
        public void OnBeginDrag(PointerEventData eventData)
        {
            isDragging = true;
        }

        public void OnDrag(PointerEventData eventData)
        {
            // Переводим позицию курсора в локальные координаты backgroundRect
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                backgroundRect,
                eventData.position,
                eventData.pressEventCamera,
                out Vector2 localPoint
            );

            float normalized;
            if (direction == Direction.Vertical)
            {
                // От 0 (низ) до 1 (верх)
                float y = Mathf.Clamp(localPoint.y - handleSize / 2f, 0, scrollLength);
                normalized = y / scrollLength;
            }
            else
            {
                float x = Mathf.Clamp(localPoint.x - handleSize / 2f, 0, scrollLength);
                normalized = x / scrollLength;
            }

            normalized = Mathf.Clamp01(normalized);
            if (invertDirection)
                normalized = 1f - normalized;

            // Применяем к ScrollRect
            if (direction == Direction.Vertical)
                targetScrollRect.verticalNormalizedPosition = normalized;
            else
                targetScrollRect.horizontalNormalizedPosition = normalized;

            // Обновляем позицию ручки (она и так обновится через событие onValueChanged,
            // но так избегаем задержек)
            SetHandleNormalizedPosition(invertDirection ? 1f - normalized : normalized);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            isDragging = false;
        }

        // Для клика по фону (быстрый переход)
        public void OnPointerDown(PointerEventData eventData)
        {
            // Вызываем OnDrag, чтобы сразу переместить ручку
            OnDrag(eventData);
        }

       
    }
}
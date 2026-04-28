using UnityEngine;

namespace _Project.UI
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeAreaFitter : MonoBehaviour
    {
        private RectTransform rectTransform;

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        void Start()
        {
            ApplySafeArea();
        }
        
        void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;

            // Переводим пиксельные координаты safe area в нормализованные (0..1) для Canvas
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            // Применяем к RectTransform
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;

            // Сбрасываем смещения, потому что теперь вся работа через якоря
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }
}
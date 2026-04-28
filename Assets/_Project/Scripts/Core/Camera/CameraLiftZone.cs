using UnityEngine;

namespace _Project.Core.Camera
{
    using UnityEngine;

    public class CameraLiftZone : MonoBehaviour
    {
        [SerializeField] private Transform startMarker;
        [SerializeField] private Transform endMarker;

        /// <summary>
        /// Возвращает высоту камеры для заданной координаты X (по прямой между маркерами).
        /// Если X за пределами зоны, возвращает конечную точку (clamp).
        /// </summary>
        public float GetYForX(float x)
        {
            if (startMarker == null || endMarker == null)
                return 0f;

            float startX = startMarker.position.x;
            float endX = endMarker.position.x;

            // На случай, если маркеры расставлены в обратном порядке
            if (Mathf.Approximately(startX, endX))
                return startMarker.position.y;

            float t = Mathf.InverseLerp(startX, endX, x);
            return Mathf.Lerp(startMarker.position.y, endMarker.position.y, t);
        }

        /// <summary>
        /// Показывает, попадает ли координата X внутрь зоны.
        /// </summary>
        public bool ContainsX(float x)
        {
            if (startMarker == null || endMarker == null)
                return false;
            float minX = Mathf.Min(startMarker.position.x, endMarker.position.x);
            float maxX = Mathf.Max(startMarker.position.x, endMarker.position.x);
            return x >= minX && x <= maxX;
        }

        // Для наглядности в редакторе рисуем линию
        private void OnDrawGizmosSelected()
        {
            if (startMarker != null && endMarker != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(startMarker.position, endMarker.position);
                Gizmos.DrawSphere(startMarker.position, 0.2f);
                Gizmos.DrawSphere(endMarker.position, 0.2f);
            }
        }
    }
}
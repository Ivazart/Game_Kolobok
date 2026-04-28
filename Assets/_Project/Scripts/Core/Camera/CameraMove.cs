using System.Collections.Generic;
using UnityEngine;

namespace _Project.Core.Camera
{
    public class CameraMove : MonoBehaviour
    {
        [Header("Dynamic Horizontal Follow")] 
        [SerializeField] private float damping = 1.5f; // базовое время сглаживания
        [SerializeField] private float speedInfluence = 0.1f; // влияние скорости игрока
        [SerializeField] private float distanceInfluence = 0.05f; // влияние расстояния до цели
        [SerializeField] private float minSmoothTime = 0.5f;
        [SerializeField] private float maxSmoothTime = 2f;

        [Header("Vertical Follow (Zones)")] 
        [SerializeField] private float verticalSmoothTime = 0.5f; // время сглаживания подъёма/спуска
        [SerializeField] private List<CameraLiftZone> liftZones = new List<CameraLiftZone>();

        [Header("Base Offset")] 
        [SerializeField] private Vector2 offset = new Vector2(0f, 0f);
        [SerializeField] private float offsetYForGroundVision = 1f;
        
        [Header("ScreenScale")]
        [SerializeField] private float targetWorldWidth = 19.2f; // ширина в юнитах, которую всегда видно
        [SerializeField] private bool adjustCameraSize = true;
        
        private const float OffsetZ = -10f;
        private Transform player;
        private Vector3 lastPlayerPosition;
        private float playerSpeed;
        private UnityEngine.Camera followCamera;
        
        // Раздельные скорости для осей
        private float velocityX = 0f;
        private float velocityY = 0f;

        // Текущая плавная высота, которую отрабатывает камера
        private float currentDesiredY; // значение, к которому стремится камера по Y (обновляется редко)
        private float targetDesiredY; // что мы вычисляем из зон

        public void SetPlayer(Transform player)
        {
            this.player = player;
            if (player != null)
            {
                lastPlayerPosition = player.position;
                currentDesiredY = GetBaseY();
            }
        }

        public void InstantMove()
        {
            if (player == null) 
                return;
            
            MoveToPosition(instantMove: true);
        }
        
        private void Awake()
        {
            followCamera = GetComponent<UnityEngine.Camera>();
            UpdateCameraOrthoSize();
        }
        
        private void UpdateCameraOrthoSize()
        {
            if (!adjustCameraSize || followCamera == null) return;

            float targetSize = targetWorldWidth / (2f * followCamera.aspect);
            // опционально ограничь, чтобы не уезжало в крайности (очень высокий/широкий экран)
            Debug.Log("targetSize: " + targetSize);
            targetSize = Mathf.Clamp(targetSize, 6f, 10f);

            followCamera.orthographicSize = targetSize;
        }
        
        private void Update()
        {
            // Если игра на паузе — не двигаем камеру и сбрасываем скорость
            if (Time.timeScale <= 0.0001f || Time.deltaTime <= 0.0001f)
            {
                playerSpeed = 0f;
                return;
            }
            
            Vector3 playerPos = player.position;
            if (float.IsNaN(playerPos.x) || float.IsNaN(playerPos.y))
                return;

            // Расчёт скорости игрока для динамического демпфирования
            Vector3 playerDelta = playerPos - lastPlayerPosition;
            if (float.IsNaN(playerDelta.x) || float.IsNaN(playerDelta.y))
                playerDelta = Vector3.zero;

            playerSpeed = playerDelta.magnitude / Time.deltaTime;
            lastPlayerPosition = playerPos;

            MoveToPosition();
        }

        private void MoveToPosition(bool instantMove = false)
        {
            Vector3 pos = transform.position;

            // Цель по X (относительно игрока)
            float targetX = player.position.x - offset.x;

            // Определяем желаемую высоту камеры в зависимости от зон
            targetDesiredY = GetDesiredYForX(targetX);

            if (instantMove)
            {
                // Мгновенное перемещение и сброс скоростей
                transform.position = new Vector3(targetX, targetDesiredY, OffsetZ);
                velocityX = 0f;
                velocityY = 0f;
                currentDesiredY = targetDesiredY;
                return;
            }

            // --- Горизонтальное движение с динамическим сглаживанием ---
            float distance = Mathf.Abs(targetX - pos.x);
            float dynamicSmoothTime = damping - (playerSpeed * speedInfluence) - (distance * distanceInfluence);
            dynamicSmoothTime = Mathf.Clamp(dynamicSmoothTime, minSmoothTime, maxSmoothTime);

            float newX = Mathf.SmoothDamp(pos.x, targetX, ref velocityX, dynamicSmoothTime);

            // --- Вертикальное движение (независимое сглаживание) ---
            currentDesiredY = Mathf.SmoothDamp(currentDesiredY, targetDesiredY, ref velocityY, verticalSmoothTime);
            // currentDesiredY — та высота, к которой камера подтягивается плавно

            transform.position = new Vector3(newX, currentDesiredY, OffsetZ);
        }

        /// <summary>
        /// Базовая высота камеры вне зон подъёма.
        /// </summary>
        private float GetBaseY()
        {
            return offset.y - offsetYForGroundVision;
        }

        /// <summary>
        /// Вычисляет желаемую высоту камеры для заданной мировой X (точки, куда движется камера).
        /// Если X внутри одной из зон, возвращает интерполированное значение;
        /// иначе — базовую высоту.
        /// </summary>
        private float GetDesiredYForX(float worldX)
        {
            foreach (var zone in liftZones)
            {
                if (zone != null && zone.ContainsX(worldX))
                {
                    return zone.GetYForX(worldX);
                }
            }

            return GetBaseY();
        }

        // Визуализация базовой высоты
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Vector3 basePos = new Vector3(transform.position.x, GetBaseY(), transform.position.z);
            Gizmos.DrawWireSphere(basePos, 0.2f);
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace _Project.Core.Camera
{
    public class CameraMove : MonoBehaviour
    {
        [Header("Dynamic Horizontal Follow")] 
        [SerializeField] private float damping = 1.5f;
        [SerializeField] private float speedInfluence = 0.1f;
        [SerializeField] private float distanceInfluence = 0.05f;
        [SerializeField] private float minSmoothTime = 0.5f;
        [SerializeField] private float maxSmoothTime = 2f;

        [Header("Vertical Follow (Zones)")] 
        [SerializeField] private float verticalSmoothTime = 0.5f;
        [SerializeField] private List<CameraLiftZone> liftZones = new List<CameraLiftZone>();

        [Header("Base Offset")] 
        [SerializeField] private Vector2 offset = new Vector2(0f, 0f);
        [SerializeField] private float offsetYForGroundVision = 1f;
        
        [Header("ScreenScale")]
        [SerializeField] private float targetWorldWidth = 19.2f;
        [SerializeField] private bool adjustCameraSize = true;
        
        [Header("Boundaries")]
        [SerializeField] private Transform startMarker;
        [SerializeField] private Transform finishMarker;
        
        private const float OffsetZ = -10f;
        private Transform player;
        private Vector3 lastPlayerPosition;
        private float playerSpeed;
        private UnityEngine.Camera followCamera;
        
        private float velocityX = 0f;
        private float velocityY = 0f;

        private float currentDesiredY;
        private float targetDesiredY;

        // --- Кэшированные значения ---
        private float cameraHalfWidth;      // orthographicSize * aspect
        private float baseCameraY;          // offset.y - offsetYForGroundVision
        private bool hasStartMarker;
        private bool hasFinishMarker;
        private float startMarkerX;         // если маркеры статичны
        private float finishMarkerX;

        public void SetPlayer(Transform player)
        {
            this.player = player;
            if (player != null)
            {
                lastPlayerPosition = player.position;
                currentDesiredY = baseCameraY;
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
            
            // Кэшируем полуширину камеры (если размер динамический – нужно будет обновлять)
            cameraHalfWidth = followCamera.orthographicSize * followCamera.aspect;
            
            // Кэшируем базовую высоту
            baseCameraY = offset.y - offsetYForGroundVision;
            
            // Кэшируем информацию о маркерах
            hasStartMarker = startMarker != null;
            hasFinishMarker = finishMarker != null;
            if (hasStartMarker) startMarkerX = startMarker.position.x;
            if (hasFinishMarker) finishMarkerX = finishMarker.position.x;
        }
        
        private void UpdateCameraOrthoSize()
        {
            if (!adjustCameraSize || followCamera == null) return;

            float targetSize = targetWorldWidth / (2f * followCamera.aspect);
            targetSize = Mathf.Clamp(targetSize, 6f, 10f);
            Debug.Log("targetSize: " + targetSize);
            followCamera.orthographicSize = targetSize;
            // После изменения размера обязательно обновить кэш полуширины
            cameraHalfWidth = targetSize * followCamera.aspect;
        }
        
        private void Update()
        {
            if (Time.timeScale <= 0.0001f || Time.deltaTime <= 0.0001f)
            {
                playerSpeed = 0f;
                return;
            }
            
            Vector3 playerPos = player.position;
            if (float.IsNaN(playerPos.x) || float.IsNaN(playerPos.y))
                return;

            Vector3 playerDelta = playerPos - lastPlayerPosition;
            if (float.IsNaN(playerDelta.x) || float.IsNaN(playerDelta.y))
                playerDelta = Vector3.zero;

            playerSpeed = playerDelta.magnitude / Time.deltaTime;
            lastPlayerPosition = playerPos;

            MoveToPosition();
        }

        private void MoveToPosition(bool instantMove = false)
        {
            float targetX = player.position.x - offset.x;
            targetX = ClampTargetX(targetX);

            targetDesiredY = GetDesiredYForX(targetX);

            if (instantMove)
            {
                transform.position = new Vector3(targetX, targetDesiredY, OffsetZ);
                velocityX = 0f;
                velocityY = 0f;
                currentDesiredY = targetDesiredY;
                return;
            }

            Vector3 pos = transform.position;
            float distance = Mathf.Abs(targetX - pos.x);
            float dynamicSmoothTime = damping - (playerSpeed * speedInfluence) - (distance * distanceInfluence);
            dynamicSmoothTime = Mathf.Clamp(dynamicSmoothTime, minSmoothTime, maxSmoothTime);

            float newX = Mathf.SmoothDamp(pos.x, targetX, ref velocityX, dynamicSmoothTime);
            currentDesiredY = Mathf.SmoothDamp(currentDesiredY, targetDesiredY, ref velocityY, verticalSmoothTime);

            transform.position = new Vector3(newX, currentDesiredY, OffsetZ);
        }

        private float ClampTargetX(float desiredX)
        {
            // Используем закэшированные данные
            float minX = hasStartMarker ? startMarkerX + cameraHalfWidth : float.MinValue;
            float maxX = hasFinishMarker ? finishMarkerX - cameraHalfWidth : float.MaxValue;

            if (minX > maxX) // Уровень уже камеры, фиксируем по центру
            {
                float center = (startMarkerX + finishMarkerX) * 0.5f;
                return center;
            }

            return Mathf.Clamp(desiredX, minX, maxX);
        }

        private float GetBaseY()
        {
            return baseCameraY;
        }

        private float GetDesiredYForX(float worldX)
        {
            foreach (var zone in liftZones)
            {
                if (zone != null && zone.ContainsX(worldX))
                {
                    return zone.GetYForX(worldX);
                }
            }
            return baseCameraY;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Vector3 basePos = new Vector3(transform.position.x, baseCameraY, transform.position.z);
            Gizmos.DrawWireSphere(basePos, 0.2f);
        }
    }
}
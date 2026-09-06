using System.Collections.Generic;
using UnityEngine;

namespace _Project.Core.Camera
{
    [RequireComponent(typeof(UnityEngine.Camera))]
    public class CameraMove : MonoBehaviour
    {
        [Header("Horizontal Follow")]
        [SerializeField] private float damping = 1.2f;
        [SerializeField] private float speedInfluence = 0.1f;
        [SerializeField] private float distanceInfluence = 0.05f;
        [SerializeField] private float minSmoothTime = 0.4f;
        [SerializeField] private float maxSmoothTime = 1.5f;

        [Header("Vertical")]
        [SerializeField] private float verticalSmoothTime = 0.35f;

        [Header("Offset")]
        [SerializeField] private Vector2 offset;
        [SerializeField] private float offsetYForGroundVision = 1f;

        [Header("Screen Scale")]
        [SerializeField] private float targetWorldWidth = 19.2f;
        [SerializeField] private bool adjustCameraSize = true;

        [Header("Level Limits")]
        [SerializeField] private Transform startMarker;
        [SerializeField] private Transform finishMarker;

        private const float CameraZ = -10f;

        private Transform player;
        private UnityEngine.Camera cam;

        private float velocityX;
        private float velocityY;

        private float currentY;
        private float playerSpeed;
        private Vector3 lastPlayerPos;

        private float cameraHalfWidth;
        private float baseCameraY;

        private readonly List<ActiveArea> activeAreas = new();

        private struct ActiveArea
        {
            public CameraArea area;
            public int enterOrder;
        }

        private int enterCounter;
        
        public void SetPlayer(Transform target)
        {
            player = target;

            if (player == null)
                return;

            lastPlayerPos = player.position;
            currentY = baseCameraY;
        }

        public void InstantMove()
        {
            if (player != null)
                MoveCamera(true);
        }

        private void Awake()
        {
            cam = GetComponent<UnityEngine.Camera>();

            UpdateCameraSize();

            cameraHalfWidth = cam.orthographicSize * cam.aspect;
            baseCameraY = offset.y - offsetYForGroundVision;
        }

        private void Update()
        {
            if (player == null)
                return;

            Vector3 delta = player.position - lastPlayerPos;
            playerSpeed = delta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);
            lastPlayerPos = player.position;

            MoveCamera();
        }

        private void MoveCamera(bool instant = false)
        {
            CameraArea area = GetCurrentArea();

            float targetX = player.position.x - offset.x;
            float targetY = baseCameraY;

            bool useCustom = area != null && area.useCustomScreenPosition;

            if (useCustom)
            {
                CameraState state = area.Evaluate(player.position, 0f,
                    cam.orthographicSize, cameraHalfWidth);
                targetX = state.targetX;
                targetY = state.targetY;
            }
            else if (area != null)
            {
                CameraState state = area.Evaluate(player.position, 0f,
                    cam.orthographicSize, cameraHalfWidth);

                if (!state.followX)
                    targetX = state.targetX;

                targetY = state.targetY;
            }

            targetX = ClampX(targetX);

            if (instant)
            {
                transform.position = new Vector3(targetX, targetY, CameraZ);
                currentY = targetY;
                velocityX = velocityY = 0f;
                return;
            }

            float distance = Mathf.Abs(targetX - transform.position.x);

            float smooth = damping
                - playerSpeed * speedInfluence
                - distance * distanceInfluence;

            smooth = Mathf.Clamp(smooth, minSmoothTime, maxSmoothTime);

            float newX = Mathf.SmoothDamp(
                transform.position.x,
                targetX,
                ref velocityX,
                smooth);

            currentY = Mathf.SmoothDamp(
                currentY,
                targetY,
                ref velocityY,
                verticalSmoothTime);

            transform.position = new Vector3(newX, currentY, CameraZ);
        }

        private CameraArea GetCurrentArea()
        {
            if (activeAreas.Count == 0)
                return null;

            ActiveArea best = activeAreas[0];

            for (int i = 1; i < activeAreas.Count; i++)
            {
                ActiveArea candidate = activeAreas[i];

                if (candidate.area.order > best.area.order)
                {
                    best = candidate;
                }
                else if (candidate.area.order == best.area.order &&
                         candidate.enterOrder > best.enterOrder)
                {
                    best = candidate;
                }
            }

            return best.area;
        }

        private float ClampX(float x)
        {
            if (startMarker == null || finishMarker == null)
                return x;

            float min = startMarker.position.x + cameraHalfWidth;
            float max = finishMarker.position.x - cameraHalfWidth;

            return Mathf.Clamp(x, min, max);
        }

        private void UpdateCameraSize()
        {
            if (!adjustCameraSize)
                return;

            float size = targetWorldWidth / (2f * cam.aspect);

            cam.orthographicSize = Mathf.Clamp(size, 6f, 10f);
        }

        public void EnterArea(CameraArea area)
        {
            if (area == null)
                return;

            if (activeAreas.Exists(a => a.area == area))
                return;

            activeAreas.Add(new ActiveArea
            {
                area = area,
                enterOrder = ++enterCounter
            });
        }

        public void ExitArea(CameraArea area)
        {
            activeAreas.RemoveAll(a => a.area == area);
        }
    }
}
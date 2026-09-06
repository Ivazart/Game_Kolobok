using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace _Project.Core.Camera
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class CameraArea : MonoBehaviour
    {
        public enum Mode
        {
            Horizontal,
            Elevator,
            Slope,
            Fixed
        }
        
        [Header("Priority")]
        [Tooltip("Как Sorting Order: большее значение имеет приоритет.")]
        public int order = 0;

        [Header("General")]
        public Mode mode = Mode.Horizontal;

        [Header("Horizontal")]
        [Tooltip("Смещение по Y относительно позиции зоны.")]
        public float fixedY = 0f;

        [Header("Elevator")]
        [Tooltip("Точка, к которой камера должна прийти по X. Если не задана, используется X зоны.")]
        public Transform elevatorXMarker;
        [Tooltip("Устарело: теперь X берётся из маркера или позиции зоны.")]
        public bool lockXOnEnter = true;

        [Header("Slope")]
        public Transform startMarker;
        public Transform endMarker;
        public float halfWidth = 2f;
        public float projectionPadding = 0.05f;

        [Header("Fixed")]
        [Tooltip("Если не задан, используется позиция самой зоны.")]
        public Transform fixedCameraMarker;

        [Header("Screen Position")]
        public bool useCustomScreenPosition = false;
        [Tooltip("Положение игрока на экране (0..1). 0.5 – центр.")]
        public Vector2 playerScreenPosition = new Vector2(0.5f, 0.5f);

        private void Reset()
        {
            GetComponent<BoxCollider2D>().isTrigger = true;

            // Автосоздание маркера для Fixed-режима
            if (fixedCameraMarker == null)
            {
                Transform existing = transform.Find("FixedCameraMarker");
                if (existing != null)
                    fixedCameraMarker = existing;
                else
                {
                    GameObject marker = new GameObject("FixedCameraMarker");
                    marker.transform.SetParent(transform);
                    marker.transform.localPosition = Vector3.zero;
                    fixedCameraMarker = marker.transform;
                }
            }
        }

        public CameraState Evaluate(Vector2 playerPos, float lockedX, float orthoSize, float halfWidth)
        {
            CameraState state = new CameraState();

            switch (mode)
            {
                case Mode.Horizontal:
                    state.followX = true;
                    state.followY = false;
                    state.targetY = transform.position.y + fixedY;
                    break;

                case Mode.Elevator:
                    state.followX = false;
                    state.followY = true;
                    // X берём из маркера, если он есть, иначе из позиции зоны
                    float targetX = elevatorXMarker != null 
                        ? elevatorXMarker.position.x 
                        : transform.position.x;
                    state.targetX = targetX;
                    state.targetY = playerPos.y;
                    break;

                case Mode.Fixed:
                    state.followX = false;
                    state.followY = false;
                    Vector2 fixedPos = fixedCameraMarker != null 
                        ? (Vector2)fixedCameraMarker.position 
                        : (Vector2)transform.position;
                    state.targetX = fixedPos.x;
                    state.targetY = fixedPos.y;
                    break;

                case Mode.Slope:
                    if (TryProject(playerPos, out float t))
                    {
                        state.followX = true;
                        state.followY = false;
                        state.targetY = Mathf.Lerp(startMarker.position.y, endMarker.position.y, t);
                    }
                    else
                    {
                        state.followX = true;
                        state.followY = false;
                        state.targetY = transform.position.y + fixedY;
                    }
                    break;
            }

            // Применяем кастомную экранную позицию, если нужно
            if (useCustomScreenPosition && mode != Mode.Fixed)
            {
                float halfHeight = orthoSize;
                Vector2 sp = playerScreenPosition;

                if (state.followX)
                {
                    state.targetX = playerPos.x - (sp.x - 0.5f) * 2f * halfWidth;
                }
                else
                {
                    state.targetX += (0.5f - sp.x) * 2f * halfWidth;
                }

                if (state.followY)
                {
                    state.targetY = playerPos.y + (0.5f - sp.y) * 2f * halfHeight;
                }
                else
                {
                    state.targetY += (0.5f - sp.y) * 2f * halfHeight;
                }
            }

            return state;
        }

        private bool TryProject(Vector2 pos, out float t)
        {
            t = 0f;

            if (startMarker == null || endMarker == null)
                return false;

            Vector2 a = startMarker.position;
            Vector2 b = endMarker.position;

            float dy = b.y - a.y;
            if (Mathf.Abs(dy) < 0.0001f)
                return false;

            float rawT = (pos.y - a.y) / dy;

            if (rawT < -projectionPadding || rawT > 1f + projectionPadding)
                return false;

            t = Mathf.Clamp01(rawT);

            float lineX = Mathf.Lerp(a.x, b.x, t);
            return Mathf.Abs(pos.x - lineX) <= halfWidth;
        }

#if UNITY_EDITOR

        private void OnDrawGizmos()
        {
            if (mode == Mode.Slope)
                DrawSlopeGizmo(false);
        }

        private void OnDrawGizmosSelected()
        {
            if (mode == Mode.Slope)
                DrawSlopeGizmo(true);
            else if (mode == Mode.Fixed)
                DrawFixedCameraGizmo();
            else if (mode == Mode.Elevator)
                DrawElevatorGizmo();
        }

        private void DrawSlopeGizmo(bool selected)
        {
            if (startMarker == null || endMarker == null)
                return;

            Vector2 a = startMarker.position;
            Vector2 b = endMarker.position;

            Vector2 aRight = new Vector2(a.x + halfWidth, a.y);
            Vector2 aLeft  = new Vector2(a.x - halfWidth, a.y);
            Vector2 bRight = new Vector2(b.x + halfWidth, b.y);
            Vector2 bLeft  = new Vector2(b.x - halfWidth, b.y);

            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(a, 0.12f);
            Gizmos.DrawSphere(b, 0.12f);

            Handles.color = selected
                ? Color.yellow
                : new Color(1f, 1f, 0f, 0.7f);

            Handles.DrawAAPolyLine(3f, a, b);

            Handles.DrawAAPolyLine(2f, aRight, bRight);
            Handles.DrawAAPolyLine(2f, aLeft, bLeft);
            Handles.DrawAAPolyLine(2f, aRight, aLeft);
            Handles.DrawAAPolyLine(2f, bRight, bLeft);

            Color fill = new Color(1f, 1f, 0f, selected ? 0.18f : 0.08f);
            Handles.DrawSolidRectangleWithOutline(
                new Vector3[] { aRight, bRight, bLeft, aLeft },
                fill,
                Color.clear);

            Handles.Label((a + b) * 0.5f + Vector2.right * (halfWidth + 0.3f),
                $"Slope  width={halfWidth:0.0}");
        }

        private void DrawFixedCameraGizmo()
        {
            Vector2 fixedPos = fixedCameraMarker != null 
                ? (Vector2)fixedCameraMarker.position 
                : (Vector2)transform.position;

            float halfHeight = 5f;
            float halfWidth = halfHeight * 1.777f;

            var camMove = FindFirstObjectByType<CameraMove>();
            if (camMove != null)
            {
                var cam = camMove.GetComponent<UnityEngine.Camera>();
                if (cam != null)
                {
                    halfHeight = cam.orthographicSize;
                    halfWidth = halfHeight * cam.aspect;
                }
            }

            Vector3 center = fixedPos;
            Vector3 size = new Vector3(halfWidth * 2f, halfHeight * 2f, 0f);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(center, size);

            Handles.Label(center + Vector3.up * (halfHeight + 0.3f), "Fixed Camera");
        }

        private void DrawElevatorGizmo()
        {
            float x = elevatorXMarker != null ? elevatorXMarker.position.x : transform.position.x;
            Vector3 p1 = new Vector3(x, transform.position.y - 5f, 0f);
            Vector3 p2 = new Vector3(x, transform.position.y + 5f, 0f);

            Gizmos.color = Color.green;
            Handles.DrawAAPolyLine(3f, p1, p2);
            Handles.Label(new Vector3(x, transform.position.y + 5.5f, 0f), "Elevator X");
        }

#endif
    }

    public struct CameraState
    {
        public bool followX;
        public bool followY;

        public float targetX;
        public float targetY;
    }
}
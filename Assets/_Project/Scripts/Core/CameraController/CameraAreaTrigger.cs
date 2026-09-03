using UnityEngine;

namespace _Project.Core.Camera
{
    [RequireComponent(typeof(CameraArea))]
    public class CameraAreaTrigger : MonoBehaviour
    {
        private CameraArea area;
        private CameraMove cameraMove;

        private void Awake()
        {
            area = GetComponent<CameraArea>();
        }

        private void Start()
        {
            cameraMove = FindFirstObjectByType<CameraMove>();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            cameraMove?.EnterArea(area);
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            cameraMove?.ExitArea(area);
        }
    }
}
using System;
using _Project.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks; // если нужно для Forget()

public class DragHandler : MonoBehaviour
{
    [SerializeField] private float pushForce = 4f;
    [SerializeField] private float heightStartAnimation = 8.5f;

    [Header("References")]
    [SerializeField] private Trajectory trajectory;

    public event Action OnDragEnded;
    public event Action OnDragStarted;
    
    private Camera cam;

    private bool isDragging = false;
    private Vector2 startPoint, endPoint, direction, force;
    private float distance;
    
    private Player player;
    
    public void Init(Player playerRef)
    {
        player = playerRef;
    }
    
    private void Start()
    {
        cam = Camera.main;
    }

    private void Update()
    {
        if (!CanStartDrag()) return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (IsPointerOverUI()) return;
            StartDrag();
        }

        if (Mouse.current.leftButton.wasReleasedThisFrame && isDragging)
        {
            if (IsPointerOverUI()) return;
            EndDrag();
        }

        if (isDragging)
        {
            DragUpdate();
        }
    }

    private bool CanStartDrag()
    {
        // Условие: либо игрок может двигаться, либо уже тащим
        return player.MovementDetector.CanMove || isDragging;
    }

    private void StartDrag()
    {
        isDragging = true;
        startPoint = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        trajectory.Show();
        player.PlayerAnimation.PlayDrag().Forget();
        player.Stopper.IsPushed = true;
        OnDragStarted?.Invoke(); // GameManager может сбросить isIdle и т.п.
    }

    private void DragUpdate()
    {
        endPoint = cam.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        distance = Mathf.Clamp(distance, 0.0f, 3.5f);
        force = direction * distance * pushForce;

        float angle = Vector2.Angle(Vector2.up, direction);
        if (angle < 20f)
            player.PlayerAnimation.PlayEyesUp().Forget();

        Debug.DrawLine(startPoint, endPoint);
        trajectory.UpdateDots(player.transform.position, force);
    }

    private void EndDrag()
    {
        isDragging = false;

        if (force.y > heightStartAnimation)
            player.PlayerAnimation.PlayEyesDown().Forget();

        player.Push(force);
        trajectory.Hide();
        player.Stopper.IsPushed = false;

        OnDragEnded?.Invoke(); // GameManager увеличит счётчик прыжков
    }

    private static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}
using System;
using _Project.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using Cysharp.Threading.Tasks;
using Global;

public class DragHandler : MonoBehaviour
{
    [SerializeField] private float pushForce = 4f;
    [SerializeField] private float heightStartAnimation = 8.5f;

    [Header("References")] [SerializeField]
    private Trajectory trajectory;

    private GameController gameController => GameController.Instance;
    public event Action OnDragEnded;
    public event Action OnDragStarted;

    private Camera cam;
    private Player player;
    private Pointer pointer; // кешируем указатель

    private bool isDragging;
    private bool isDead;
    private Vector2 startPoint, endPoint, direction, force;
    private float distance;
    public void Init(Player playerRef) => player = playerRef;

    private void Start()
    {
        cam = Camera.main;
        pointer = Pointer.current;
        if (gameController != null)
            gameController.OnPlayerDeath += GameController_OnPlayerDeath;
    }

    private void GameController_OnPlayerDeath(DeathType obj)
    {
        isDead = true;
    }

    private void Update()
    {
        if (!CanStartDrag() || pointer == null)
            return;

        if (pointer.press.wasPressedThisFrame)
        {
            if (IsPointerOverUI()) return;
            StartDrag(pointer.position.ReadValue());
        }

        if (pointer.press.wasReleasedThisFrame && isDragging)
        {
            if (IsPointerOverUI()) return;
            EndDrag();
        }

        if (isDragging)
        {
            DragUpdate(pointer.position.ReadValue());
        }
    }

    private bool CanStartDrag() => (player.MovementDetector.CanMove || isDragging) && !isDead;

    private void StartDrag(Vector2 pointerPos)
    {
        isDragging = true;
        startPoint = cam.ScreenToWorldPoint(pointerPos);
        trajectory.Show();
        player.PlayerAnimation.PlayDrag().Forget();
        player.Stopper.IsPushed = true;
        OnDragStarted?.Invoke();
    }

    private void DragUpdate(Vector2 pointerPos)
    {
        endPoint = cam.ScreenToWorldPoint(pointerPos);
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

        OnDragEnded?.Invoke();
    }

    private static bool IsPointerOverUI() =>
        EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();

    private void OnDestroy()
    {
        try
        {
            gameController.OnPlayerDeath -= GameController_OnPlayerDeath;
        }
        catch (Exception ex)
        {
            // ignored
        }
    }
}


/*using System;
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
}*/
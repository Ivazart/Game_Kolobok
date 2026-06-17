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

    // === НОВОЕ: InputAction для универсального нажатия ===
    private InputAction pressAction;
    // ====================================================

    private bool isDragging;
    private bool isDead;
    private Vector2 startPoint, endPoint, direction, force;
    private float distance;

    public void Init(Player playerRef) => player = playerRef;

    private void Awake()
    {
        // Создаём экшен, который реагирует на любой указатель (мышь, тач, стилус)
        pressAction = new InputAction("Press", type: InputActionType.Button);
        pressAction.AddBinding("<Pointer>/press");       // основной для мыши и тачскрина
        pressAction.AddBinding("<Mouse>/leftButton");    // на случай, если Pointer/press не поддерживается
        pressAction.AddBinding("<Touchscreen>/press");   // для тачскринов, где нет Pointer
        pressAction.Enable();
    }

    private void Start()
    {
        cam = Camera.main;
        if (gameController != null)
            gameController.OnPlayerDeath += GameController_OnPlayerDeath;
    }

    private void GameController_OnPlayerDeath(DeathType obj) => isDead = true;

    private void Update()
    {
        if (!CanStartDrag())
            return;

        // === ИСПОЛЬЗУЕМ НАШ ЭКШЕН вместо Pointer.current.press ===
        if (pressAction.WasPressedThisFrame())
        {
            if (IsPointerOverUI()) return;
            StartDrag(GetPointerPosition());
        }

        if (pressAction.WasReleasedThisFrame() && isDragging)
        {
            if (IsPointerOverUI()) return;
            EndDrag();
        }

        if (isDragging)
        {
            DragUpdate(GetPointerPosition());
        }
    }

    private bool CanStartDrag() => (player.MovementDetector.CanMove || isDragging) && !isDead;

    // Вспомогательный метод для безопасного получения позиции указателя
    private Vector2 GetPointerPosition()
    {
        // Пробуем через Pointer (работает даже без press)
        if (Pointer.current != null)
            return Pointer.current.position.ReadValue();

        // Запасные варианты
        if (Touchscreen.current != null)
            return Touchscreen.current.primaryTouch.position.ReadValue();

        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();

        return Vector2.zero;
    }

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
        try { gameController.OnPlayerDeath -= GameController_OnPlayerDeath; }
        catch { /* ignored */ }

        pressAction?.Disable();
        pressAction?.Dispose();
    }
}
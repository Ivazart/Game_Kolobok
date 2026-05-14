using _Project.Core;
using _Project.Player;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Global;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelProgress levelProgress;
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private DragHandler dragHandler;   // ссылка на новый компонент

    private SaveController saveController => SaveController.Instance;
    private JumpsCounterController jumpController => JumpsCounterController.Instance;

    private Player player;
    private bool isIdle;

    private void Start()
    {
        player = playerSpawner.Player.GetComponent<Player>();
        dragHandler.Init(player);
        playerSpawner.MoveToLastPoint();
        levelProgress.StartDistanceCalculation(player.transform);
        
        Time.timeScale = 1f;
        Debug.Log("Timescale: " + Time.timeScale);
        // Подписываемся на события DragHandler
        if (dragHandler != null)
        {
            dragHandler.OnDragStarted += HandleDragStarted;
            dragHandler.OnDragEnded += HandleDragEnded;
        }
    }

    private void OnDestroy()
    {
        if (dragHandler != null)
        {
            dragHandler.OnDragStarted -= HandleDragStarted;
            dragHandler.OnDragEnded -= HandleDragEnded;
        }
    }

    private void Update()
    {
        AnimationLoop(); // только анимационная логика
    }

    private void HandleDragStarted()
    {
        saveController.TutorFinished();
        // здесь можно сбросить isIdle, если нужно
    }

    private void HandleDragEnded()
    {
        jumpController.IncreaseJumpCounter();
    }

    private void AnimationLoop()
    {
        bool canMove = player.MovementDetector.CanMove;

        if (canMove)
        {
            if (isIdle || player.isDying)
                return;

            isIdle = true;
            player.PlayerAnimation.PlayIdle().Forget();
        }
        else
        {
            if (!isIdle)
                return;

            isIdle = false;
            player.PlayerAnimation.StopIdle();
        }
    }
}
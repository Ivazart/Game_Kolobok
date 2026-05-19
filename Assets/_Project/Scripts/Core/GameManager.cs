using System;
using _Project.Core;
using _Project.Player;
using UnityEngine;
using Cysharp.Threading.Tasks;
using Global;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelProgress levelProgress;
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private DragHandler dragHandler; 

    private SaveController saveController => SaveController.Instance;
    private JumpsCounterController jumpController => JumpsCounterController.Instance;

    private Player player;
    private bool isIdle;
    private bool isDead;

    private void Start()
    {
        player = playerSpawner.Player.GetComponent<Player>();
        dragHandler.Init(player);
        playerSpawner.MoveToLastPoint();
        levelProgress.StartDistanceCalculation(player.transform);

        Time.timeScale = 1f;
        Debug.Log("Timescale: " + Time.timeScale);
        
        dragHandler.OnDragStarted += HandleDragStarted;
        dragHandler.OnDragEnded += HandleDragEnded;
    }
    
    private void Update()
    {
        AnimationLoop();
    }

    private void HandleDragStarted()
    {
        saveController.TutorFinished();
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
    
    private void OnDestroy()
    {
        try
        {
            dragHandler.OnDragStarted -= HandleDragStarted;
            dragHandler.OnDragEnded -= HandleDragEnded;
        }
        catch (Exception ex)
        {
            // ignored
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using _Project.Core;
using _Project.Player;
using Cysharp.Threading.Tasks;
using Global;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private LevelProgress levelProgress;
    [SerializeField] private PlayerSpawner playerSpawner;
    [SerializeField] private Trajectory trajectory;
    [SerializeField] private float pushForce = 4f;
    [SerializeField] private float heightStartAnimation = 8.5f;
  
    private SaveController saveController => SaveController.Instance;
    
    private bool isDragging = false;
    private bool isIdle;
    private bool isMoving => player.MovementDetector.IsMoving;
    private Player player;
    private Camera cam;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 direction;
    private Vector2 force;
    private float distance;
    private float time;

    

    private void Start()
    {
        player = playerSpawner.Player.GetComponent<Player>();
        playerSpawner.MoveToLastPoint();
        cam = Camera.main;
        levelProgress.StartDistanceCalculation(playerSpawner.Player.transform);
        Time.timeScale = 1f;
        Debug.Log ("Timescale: " + Time.timeScale);
    }
    
    private void Update()
    {
        MouseHandlerLoop();
        AnimationLoop();
    }

    private void MouseHandlerLoop()
    {
        if (!isMoving || isDragging)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (IsPointerOverUI())
                    return;
                isDragging = true;
                OnDragStart();
            }

            if (Input.GetMouseButtonUp(0) && isDragging)
            {
                OnDragEnd();
                isDragging = false;
            }

            if (isDragging)
            {
                OnDrag();
            }
        }
    }
    
    private void AnimationLoop()
    {
        if (!isMoving)
        {
            if (!isIdle)
            {
                isIdle = true;
                player.PlayerAnimation.PlayIdle().Forget();
            }
        }
        else
        {
            if (isIdle)
            {
                isIdle = false;
                player.PlayerAnimation.StopIdle();
            }
              
        }
    }
    
    private static bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
    
    //-Drag----
    private void OnDragStart()
    {
        saveController.TutorFinished();
        startPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        trajectory.Show();
        player.PlayerAnimation.PlayDrag().Forget();
    }

    private void OnDrag()
    {
        endPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        distance = Mathf.Clamp(distance, 0.0f, 3.5f);
        force =  direction * distance * pushForce;
        float angle = Vector2.Angle(Vector2.up, direction);
        if (angle < 20f) 
            player.PlayerAnimation.PlayEyesUp().Forget();
        Debug.DrawLine(startPoint, endPoint);
        var pos = player.transform.position;
        trajectory.UpdateDots (pos, force);
    }

    private void OnDragEnd()
    {
        if (force.y > heightStartAnimation)
            player.PlayerAnimation.PlayEyesDown().Forget();
        player.Push(force);
        trajectory.Hide();
        saveController.IncreaseJumpCounter();
    }
    
}

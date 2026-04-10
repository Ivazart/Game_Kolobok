using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    #region Singleton class: GameManager

    public static GameManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    #endregion


    [SerializeField] private Ball ball;
    [SerializeField] private Trajectory trajectory;
    [SerializeField] private float pushForce = 4f;

    public bool isDragging = false;
    public bool isPushed = false;

    private Camera cam;
    private Vector2 startPoint;
    private Vector2 endPoint;
    private Vector2 direction;
    private Vector2 force;
    private float distance;


    private void Start()
    {
        cam = Camera.main;
        ball.ActivateRb();
    }

    private void Update()
    {
        if (ball.isGrounded || isDragging)
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                OnDragStart();
                isPushed = true;
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

    //-Drag----
    private void OnDragStart()
    {
        startPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        trajectory.Show();
    }

    private void OnDrag()
    {
        endPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        distance = Mathf.Clamp(distance, 0.0f, 3.5f);
        force =  direction * distance * pushForce; 
        
        Debug.DrawLine(startPoint, endPoint);
        trajectory.UpdateDots (ball.pos, force);
    }

    private void OnDragEnd()
    {
        ball.Push(force);
        trajectory.Hide();
    }
    
}

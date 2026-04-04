using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton class: GameManager

    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    #endregion

    Camera cam;

    public Ball ball;
    public Trajectory trajectory;
    [SerializeField] float pushForse = 4f;

    public bool isDragging = false;
    public bool push = false;

    Vector2 startPoint;
    Vector2 endPoint;
    Vector2 direction;
    Vector2 force;
    float distance;


    void Start()
    {
        cam = Camera.main;
        ball.ActivateRb();

    }

    void Update()
    {
        if (ball.isGrounded == true || isDragging == true)
        {
           
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                OnDragStart();
                push = true;
            }

            if (Input.GetMouseButtonUp(0) && isDragging == true)
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

    void OnDragStart()
    {
        //ball.DesactivateRb();
        startPoint = cam.ScreenToWorldPoint(Input.mousePosition);

        trajectory.Show();

      

    }

    void OnDrag()
    {
        endPoint = cam.ScreenToWorldPoint(Input.mousePosition);
        distance = Vector2.Distance(startPoint, endPoint);
        direction = (startPoint - endPoint).normalized;
        distance = Mathf.Clamp(distance, 0.0f, 3.5f);
        force =  direction * distance * pushForse; 
        

        Debug.DrawLine(startPoint, endPoint);

        trajectory.UpdateDots (ball.pos, force);

       
    }

    void OnDragEnd()
    {

        //ball.ActivateRb();

        ball.Push(force);

        trajectory.Hide();



    }
    

}

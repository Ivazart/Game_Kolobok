using System;
using System.Collections;
using System.Collections.Generic;
using StartingLab;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleGameManager : MonoBehaviour
{
    [SerializeField] private Button startingButton;
    [SerializeField] private AudioSource al;
    [SerializeField] private List<StartingLabAnimation> elements = new();

    private void Awake()
    {
        startingButton.OnButtonClicked += StartingButton_OnButtonClicked;
    }
    
    private void Start()
    {
        SetState(StartingLabState.Idle);
        SetStateWithDelay(7f, StartingLabState.Alarm);
    }
    
    private void StartingButton_OnButtonClicked()
    {
        StartCoroutine(OnStartClickedCoroutine()); 
    }
    
    private void SetStateWithDelay(float delay, StartingLabState state)
    {
        IEnumerator coroutine = WaitAndChangeState(delay, state);
        StartCoroutine(coroutine); 
    }

    private void SetState(StartingLabState state)
    {
        foreach (StartingLabAnimation startingLabAnimation in elements)
        {
            startingLabAnimation.SetState(state);
        }
        startingButton.SetLabState(state);
        if (state == StartingLabState.Alarm)
            al.Play();
    }
    
    private IEnumerator WaitAndChangeState(float waitTime, StartingLabState state)
    {
        yield return new WaitForSeconds(waitTime);
        SetState(state);
    }

    private IEnumerator OnStartClickedCoroutine()
    {
        yield return WaitAndChangeState(2f, StartingLabState.Active);
        yield return new WaitForSeconds(6f);
        LoadScene();
    }
    private void LoadScene()
    {
        SceneManager.LoadScene("space 1");
    }
//start idle
//alarm +7
//active +2
//space + 8

    /*private void Start()
    {
        Invoke(nameof(Alarm), 7);
    }

    public void Active()
    {
        man.activ_anim();
        foreach (var screenAnim in screens)
        {
            screenAnim.ActiveState();
        }
        flask.Activate();

    }
    public void Alarm()
    {
        but.Alarm();
        man.alarm_anim();
        foreach (var screenAnim in screens)
        {
            screenAnim.alarm_anim();
        }
        al.Play();

    }
    public void Space()
    {
        Invoke(nameof(LoadScene), 8);

    }
    private void LoadScene()
    {
        SceneManager.LoadScene("space 1");
    }*/
}

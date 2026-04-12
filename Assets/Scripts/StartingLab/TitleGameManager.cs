using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using StartingLab;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleGameManager : MonoBehaviour
{
    [SerializeField] private PlayButton startingButton;
    [SerializeField] private UIController uiController;
    [SerializeField] private AudioSource al;
    [SerializeField] private List<StartingLabAnimation> elements = new();

    private void Awake()
    {
        startingButton.OnButtonClicked += () => StartingButton_OnButtonClicked().Forget();
    }
    
    private void Start()
    {
        StartAnimations().Forget();
    }
    
    private async UniTaskVoid StartAnimations()
    {
        SetState(StartingLabState.Idle);
        await UniTask.WaitForSeconds(7f);
        SetState(StartingLabState.Alarm);
    }
    private async UniTaskVoid StartingButton_OnButtonClicked()
    {
        await UniTask.WaitForSeconds(2f);
        SetState(StartingLabState.Active);
        await UniTask.WaitForSeconds(6f);
        LoadScene();
    }

    private void SetState(StartingLabState state)
    {
        foreach (StartingLabAnimation startingLabAnimation in elements)
        {
            startingLabAnimation.SetState(state);
        }
        startingButton.SetLabState(state);
        uiController.SetState(state);
        if (state == StartingLabState.Alarm)
            al.Play();
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

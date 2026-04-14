using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global;
using StartingLab;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleGameManager : MonoBehaviour
{
    [SerializeField] private PlayButton startingButton;
    [SerializeField] private UIController uiController;
    [SerializeField] private AudioSource al;
    [SerializeField] private List<StartingLabAnimation> elements = new();
    
    private SceneController sceneController => SceneController.Instance;
    
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
        sceneController.LoadScene(SceneName.Space);
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
}

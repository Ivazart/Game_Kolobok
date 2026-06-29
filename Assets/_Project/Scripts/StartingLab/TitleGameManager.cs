using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Global;
using StartingLab;
using UnityEngine;

public class TitleGameManager : MonoBehaviour
{
    [SerializeField] private PlayButton startingButton;
    [SerializeField] private StartingLabManAnimation manAnimation;
    [SerializeField] private UIController uiController;
    [SerializeField] private TitleSceneAudio sceneAudio;
    [SerializeField] private List<StartingLabAnimation> elements = new();

    private SceneController sceneController => SceneController.Instance;
    private AudioManager audioManager => AudioManager.Instance;
    
    private CancellationTokenSource cts = new();
    private bool _isProcessing; // защита от повторного клика

    private void Awake()
    {
        Debug.Log($"TitleGameManager instance: {GetInstanceID()}", this);

        startingButton.OnButtonClicked += () =>
        {
            if (_isProcessing) return;
            _isProcessing = true;
            UniTaskUtils.RunWithCancellationAsync(HandleButtonClicked, cts.Token).Forget();
        };
    }

    private void Start()
    {
        UniTaskUtils.RunWithCancellationAsync(PlayIntroSequence, cts.Token).Forget();
    }

    /// <summary> Шаг 1-2: Idle → Alarm по таймеру. </summary>
    private async UniTask PlayIntroSequence(CancellationToken token)
    {
        sceneAudio.PlayLabAmbient(); 
        
        SetStateForAll(StartingLabState.Idle);
        await UniTask.WaitForSeconds(7f, cancellationToken: token);
        SetStateForAll(StartingLabState.Alarm);
        sceneAudio.PlayAlarm();
    }

    /// <summary> Шаг 3-5: клик → анимация человека → активация → загрузка сцены. </summary>
    private async UniTask HandleButtonClicked(CancellationToken token)
    {
        manAnimation.SetState(StartingLabState.Active);
        
        bool manFinished = false;
        void Handler() => manFinished = true;
        manAnimation.OnManClickButtonAnimFinish += Handler;
        await UniTask.WaitUntil(() => manFinished, cancellationToken: token);
        
        sceneAudio.StopAlarm();
        manAnimation.OnManClickButtonAnimFinish -= Handler;   

        SetStateForAllExceptMan(StartingLabState.Active);
        startingButton.SetLabState(StartingLabState.Active);
        uiController.SetState(StartingLabState.Active);

        await UniTask.WaitForSeconds(8f, cancellationToken: token);
        sceneController.LoadScene(SceneName.Space);
    }

    private void SetStateForAll(StartingLabState state)
    {
        Debug.Log("SetState: " + state);
        foreach (var element in elements)
            element?.SetState(state);

        startingButton.SetLabState(state);
        uiController.SetState(state);
    }

    private void SetStateForAllExceptMan(StartingLabState state)
    {
        foreach (var element in elements)
        {
            if (element != null && element != manAnimation)
                element.SetState(state);
        }
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
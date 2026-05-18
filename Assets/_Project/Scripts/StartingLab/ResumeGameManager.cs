using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Global;
using StartingLab;
using UnityEngine;


public class ResumeGameManager : MonoBehaviour
{
    [SerializeField] private PlayButton startingButton;
    [SerializeField] private UIController uiController;
    [SerializeField] private AudioSource al;
    [SerializeField] private List<StartingLabAnimation> elements = new();

    private SaveController saveController => SaveController.Instance;
    private CancellationTokenSource cts = new();

    private void Awake()
    {
        Debug.Log($"TitleGameManager instance: {GetInstanceID()}", this);
        startingButton.OnButtonClicked += () =>
            UniTaskUtils.RunWithCancellationAsync(StartingButton_OnButtonClicked, cts.Token).Forget();
    }

    private void Start()
    {
        StartAnimations();
    }

    private void StartAnimations()
    {
        SetState(StartingLabState.Alarm);
    }

    private async UniTask StartingButton_OnButtonClicked(CancellationToken token)
    {
        await UniTask.WaitForSeconds(2f, cancellationToken: token);
        SetState(StartingLabState.Active);
        await UniTask.WaitForSeconds(6f, cancellationToken: token);
        saveController.LoadLastSave();
    }

    private void SetState(StartingLabState state)
    {
        Debug.Log("SetState: " + state);
        foreach (StartingLabAnimation startingLabAnimation in elements)
        {
            if (startingLabAnimation == null)
            {
                Debug.LogError("startingLabAnimation is null");
                continue;
            }

            startingLabAnimation.SetState(state);
        }

        startingButton.SetLabState(state);
        uiController.SetState(state);
        if (state == StartingLabState.Alarm)
            al.Play();
    }
    
    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
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
    [SerializeField] private AudioSource al;
    [SerializeField] private List<StartingLabAnimation> elements = new();
    
    private SceneController sceneController => SceneController.Instance;
    private CancellationTokenSource cts = new();

    private void Awake()
    {
        Debug.Log($"TitleGameManager instance: {GetInstanceID()}", this);
        startingButton.OnButtonClicked += () =>
            UniTaskUtils.RunWithCancellationAsync(StartingButton_OnButtonClicked, cts.Token).Forget();
        manAnimation.OnManClickButtonAnimFinish += ManAnimation_OnManClickButtonAnimFinish;
    }

    private void ManAnimation_OnManClickButtonAnimFinish()
    {
        //throw new System.NotImplementedException();
    }

    private void Start()
    {
        UniTaskUtils.RunWithCancellationAsync(StartAnimations, cts.Token).Forget();
    }

    private async UniTask StartAnimations(CancellationToken token)
    {
        SetState(StartingLabState.Idle);
        await UniTask.WaitForSeconds(7f, cancellationToken: token); 
        SetState(StartingLabState.Alarm);
    }

    private async UniTask StartingButton_OnButtonClicked(CancellationToken token)
    {
        await UniTask.WaitForSeconds(2f, cancellationToken: token);
        SetState(StartingLabState.Active);
        await UniTask.WaitForSeconds(8f, cancellationToken: token);
        sceneController.LoadScene(SceneName.Space);
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
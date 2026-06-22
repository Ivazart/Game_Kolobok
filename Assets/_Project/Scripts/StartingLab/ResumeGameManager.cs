using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Global;
using StartingLab;
using UnityEngine;

public class ResumeGameManager : MonoBehaviour
{
    [SerializeField] private PlayButton startingButton;
    [SerializeField] private UIController uiController;   // оставлен, но не используется
    [SerializeField] private AudioSource al;               // не используется
    [SerializeField] private List<StartingLabAnimation> elements = new(); // только для инициализации в Idle

    private SaveController saveController => SaveController.Instance;
    private CancellationTokenSource cts = new();
    private bool _isProcessing;

    private void Awake()
    {
        Debug.Log($"ResumeGameManager instance: {GetInstanceID()}", this);
        startingButton.OnButtonClicked += () =>
        {
            if (_isProcessing) return;
            _isProcessing = true;
            UniTaskUtils.RunWithCancellationAsync(HandleButtonClicked, cts.Token).Forget();
        };
    }

    private void Start()
    {
        // Фон — всё в Idle
        foreach (var element in elements)
            element?.SetState(StartingLabState.Idle);
        
        // Кнопка становится доступной сразу
        startingButton.SetLabState(StartingLabState.Alarm);
        
        // UIController ничего не делает в Idle, можно не вызывать
    }

    private async UniTask HandleButtonClicked(CancellationToken token)
    {
        // Кнопка уже запустила свою анимацию press + fade out (0.7 сек).
        // Дождёмся окончания этой анимации.
        await UniTask.WaitForSeconds(0.7f, cancellationToken: token);
        
        // Загружаем сохранение и уходим на последний уровень
        saveController.LoadLastSave();
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}
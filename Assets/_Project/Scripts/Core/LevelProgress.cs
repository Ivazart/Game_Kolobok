using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Global;
using UnityEngine;
using UnityEngine.UI;

public class LevelProgress : MonoBehaviour
{
    //[SerializeField] private Transform playerTransform;
    [SerializeField] private Transform endLineTransform;
    [SerializeField] private Transform startLineTransform;
    [SerializeField] private Slider slider;

    private Vector3 endLinePosition;
    private float fullDistance;
    private Transform playerTransform;
    private CancellationTokenSource cts = new ();
    
    public void StartDistanceCalculation(Transform playerTransf)
    {
        playerTransform = playerTransf;
        endLinePosition = endLineTransform.position;
        fullDistance = Vector3.Distance(startLineTransform.position, endLinePosition);
        UniTaskUtils.RunWithCancellationAsync(DisplayDistance,cts.Token).Forget();
    }
    
    private async UniTask DisplayDistance(CancellationToken token)
    {
        while (token.IsCancellationRequested == false)
        {
            float newDistance = GetDistance();
            slider.value = Mathf.InverseLerp( fullDistance, 0f, newDistance);
            await UniTask.WaitForSeconds(.5f, cancellationToken: token);
        }
    }

    private float GetDistance ()
    {
        return Vector3.Distance(playerTransform.position, endLinePosition);
    }

    private void OnDestroy()
    {
        cts?.Cancel();
        cts?.Dispose();
    }
}

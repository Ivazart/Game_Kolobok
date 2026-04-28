using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Global;
using UnityEngine;

namespace _Project.UI
{
    public class UITutor : MonoBehaviour
    {
        [SerializeField] private GameObject tutor;
        private SaveController saveController => SaveController.Instance;

        private CancellationTokenSource cts = new ();
        private void Awake()
        {
            UniTaskUtils.RunWithCancellationAsync(StartTutor, cts.Token).Forget();
            tutor.SetActive(false);
        }

        private async UniTask StartTutor(CancellationToken token)
        {
            await UniTask.WaitForSeconds(4f, cancellationToken: token);
            if (saveController.SaveData.IsTutorFinished == false)
            {
                tutor.gameObject.SetActive(true);
                saveController.OnTutorFinished += SaveController_OnTutorFinished;
            }
        }

        private void SaveController_OnTutorFinished()
        {
            tutor.gameObject.SetActive(false);
            saveController.OnTutorFinished -= SaveController_OnTutorFinished;
        }

        private void OnDestroy()
        {
            cts.Cancel();
            cts.Dispose();
        }
    }
}
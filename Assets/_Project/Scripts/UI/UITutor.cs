using System;
using Cysharp.Threading.Tasks;
using Global;
using UnityEngine;

namespace _Project.UI
{
    public class UITutor : MonoBehaviour
    {
        [SerializeField] private GameObject tutor;
        private SaveController saveController => SaveController.Instance;

        private void Awake()
        {
            StartTutor().Forget();
            tutor.SetActive(false);
        }

        private async UniTask StartTutor()
        {
            await UniTask.WaitForSeconds(4f);
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
        
    }
}
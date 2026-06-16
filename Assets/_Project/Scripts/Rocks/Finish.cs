using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    private SaveController saveController => SaveController.Instance;
    private bool isTrigger;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isTrigger)
        {
            isTrigger = true;
            LevelFinished().Forget();
        }
    }

    private async UniTask LevelFinished()
    {
        await UniTask.WaitForSeconds(2.0f);
        saveController.LevelCompleted();
    }

}

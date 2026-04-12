using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Global;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SkeletonAnimation))]
public class Rocket : MonoBehaviour
{
    private SkeletonAnimation line;
    private SceneController sceneController => SceneController.Instance;
    
    private void Awake()
    {
        line = GetComponent<SkeletonAnimation>();
    }

    private void Start()
    {
        Play().Forget();
    }

    private async UniTask Play()
    {
        await UniTask.WaitForSeconds(2f);
        line.AnimationState.SetAnimation(0, "animation", false);
        await UniTask.WaitForSeconds(2.6f);
        sceneController.LoadScene(SceneName.Rocks);
    }

}
using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(SkeletonAnimation))]
public class Rocket : MonoBehaviour
{
    private SkeletonAnimation line;

    private void Awake()
    {
        line = GetComponent<SkeletonAnimation>();
    }

    private void Start()
    {
        Invoke(nameof(Animation), 2f);
        Invoke(nameof(LoadScene), 2.6f);
    }

    private void Animation()
    {
        line.AnimationState.SetAnimation(0, "animation", false);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene("Rocks");
    }
}

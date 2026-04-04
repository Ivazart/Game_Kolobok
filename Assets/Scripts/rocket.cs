using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.SceneManagement;

public class rocket : MonoBehaviour
{
    public SkeletonAnimation line;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Anim", 2);
        Invoke("Load", 2.6f);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Anim()
    {
        line.AnimationState.SetAnimation(0, "animation", false);

    }
    void Load()
    {
        SceneManager.LoadScene("Rocks");

    }
}

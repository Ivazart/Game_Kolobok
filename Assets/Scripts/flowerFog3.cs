using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class flowerFog3 : MonoBehaviour
{
    public Collider2D col;
    public SkeletonAnimation skAnim;
    public AnimationReferenceAsset cycle;
    public AnimationReferenceAsset disappearance;
    public bool activ;


    void Awake()
    {
        skAnim.AnimationState.SetAnimation(0, "cycle", true);

    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && activ == true)
        {

            skAnim.state.SetAnimation(0, "disappearance", false);
            activ = false;   


        }


    }
}

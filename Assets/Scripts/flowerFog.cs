using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Spine.Unity;
using UnityEngine;

public class flowerFog : MonoBehaviour
{

    public Collider2D col;
    public SkeletonAnimation skAnim;
    public AnimationReferenceAsset cycle;
    public AnimationReferenceAsset disappearance;
   
    

    // Start is called before the first frame update
    void Awake()
    {
        skAnim.AnimationState.SetAnimation(0, "cycle", true);

    }


    // Update is called once per frame
    void Update()
    {



    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {

            fade();


        }
    }
    public void fade()
    {
        StartCoroutine(Fadefog());
    
    
    }

    IEnumerator Fadefog()
    {
        var track = skAnim.state.SetAnimation(0, "disappearance", false);
        yield return new WaitForSpineAnimationComplete(track);

    }



}
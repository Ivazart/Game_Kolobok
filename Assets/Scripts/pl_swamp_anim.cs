using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;


public class pl_swamp_anim : MonoBehaviour

{
    public bool swamp = false;
    bool alive = true;

    public SkeletonAnimation skAnim;
    public AnimationReferenceAsset cp;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (swamp == true && alive == true)
        {
            GetComponent<MeshRenderer>().enabled = true;
            alive= false;
            skAnim.AnimationState.SetAnimation(0, "pl_swamp", false);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class final_fog : MonoBehaviour
{
    public Collider2D col;
    public SkeletonAnimation skAnim;
    public AnimationReferenceAsset cp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            
            skAnim.AnimationState.SetAnimation(0, "animation", false);
        }
    }
}

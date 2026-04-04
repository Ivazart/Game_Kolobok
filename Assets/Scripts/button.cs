using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Device;
using UnityEngine.EventSystems;

public class button : MonoBehaviour 
{

   
    public Collider2D col;
    public title_game_m gm;
    public SkeletonAnimation but;
    bool pressid = false;
    bool alarme = false;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


    }

    void OnMouseDown()
    {
        if (pressid == false && alarme == true )
        {
            gm.Active();
            but.AnimationState.SetAnimation(0, "press", false);
            pressid = true;
            gm.Space();


        }
        

    }
    public void Alarm()
    {
        but.AnimationState.SetAnimation(0, "alarme", false);
        alarme= true;
    }









}

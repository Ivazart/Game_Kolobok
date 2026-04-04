using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class title_game_m : MonoBehaviour
{
    public man_anim man;
    public flask flask;
    public screen_anim[] screens = new screen_anim[5];
    public bool idle;
    public bool alarm;
    public bool activ;
    public button but;
    public AudioSource al;
    

    void Start()
    {
        idle = true;
        alarm = false;
        activ = false;
        Invoke("Alarme", 7);
    }

    // Update is called once per frame
    void Update()
    { 
       
        
    }

    public void Active()
    {
        man.activ_anim();
        foreach (var screen_anim in screens)
        {
            screen_anim.Activ();
        }
        flask.Activ();

    }
    public void Alarme()
    {
        but.Alarm();
        man.alarm_anim();
        foreach (var screen_anim in screens)
        {
            screen_anim.alarm_anim();
        }
        al.Play();

    }
    public void Space()
    {
        Invoke("LoadSc", 8);

    }
    public void LoadSc()
    {
        SceneManager.LoadScene("space 1");
    }
    


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TitleGameManager : MonoBehaviour
{
    [SerializeField] private ManAnimation man;
    [SerializeField] private Flask flask;
    [SerializeField] private ScreenAnimation[] screens = new ScreenAnimation[5];
    [SerializeField] private Button but;
    [SerializeField] private AudioSource al;
    
    private void Start()
    {
        Invoke(nameof(Alarm), 7);
    }

    public void Active()
    {
        man.activ_anim();
        foreach (var screenAnim in screens)
        {
            screenAnim.ActiveState();
        }
        flask.Activate();

    }
    public void Alarm()
    {
        but.Alarm();
        man.alarm_anim();
        foreach (var screenAnim in screens)
        {
            screenAnim.alarm_anim();
        }
        al.Play();

    }
    public void Space()
    {
        Invoke(nameof(LoadScene), 8);

    }
    private void LoadScene()
    {
        SceneManager.LoadScene("space 1");
    }
}

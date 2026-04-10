using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public string lname;
    
    public void RestartGame()
    {
        Debug.Log("End Game");
        SceneManager.LoadScene(lname);
    }
}

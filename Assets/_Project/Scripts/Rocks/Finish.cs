using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Finish : MonoBehaviour
{
    private SaveController saveController => SaveController.Instance;
    
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            saveController.LevelCompleted();
        }
    }
}

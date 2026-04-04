using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class swamp : MonoBehaviour
{
    public ScenesManager gm;
    
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Restart()
    {
        gm.RestartGame();

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Invoke("Restart", 3f);
            
            
          
            
          
           

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;

public class DeathChecker : MonoBehaviour
{
    private GameController gameController => GameController.Instance;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("poison"))
            gameController.PlayerDeath(deathType: DeathType.Poison);
        if (collision.gameObject.CompareTag("fire"))
            gameController.PlayerDeath(deathType: DeathType.Fire);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("swamp")) 
            gameController.PlayerDeath(deathType: DeathType.Swamp);
        
    }
}

using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;
using UnityEngine.Serialization;

public class FireD : MonoBehaviour
{
    private GameController gameController => GameController.Instance;
    private bool isBurning;
    
    public void Burned()
    {
        if (isBurning)
            return;
        
        gameController.PlayerDeath(DeathType.Fire);
        isBurning = true;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using Global;
using UnityEngine;
using UnityEngine.Serialization;

public class FireD : MonoBehaviour
{
    private GameController gameController => GameController.Instance;
    
    public void Burned()
    {
        gameController.PlayerDeath(DeathType.Fire);
    }
    
}

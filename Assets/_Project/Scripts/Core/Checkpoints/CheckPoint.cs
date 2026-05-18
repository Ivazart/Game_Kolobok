using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private CheckPointTrigger trigger;
    
    public static event Action<CheckPoint> OnCheckpointEnter;
    
    private void Awake()
    {
        trigger.OnCheckpointEnter += () => OnCheckpointEnter?.Invoke(this);
    }
}

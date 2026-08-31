using System;
using UnityEngine;

public class CollisionLogic : MonoBehaviour
{
    [SerializeField] private CollisionHub hub;

    public event Action<CollisionEventData> OnEnterPlayerTriggerEnemySolid;
    public event Action<CollisionEventData> OnEnterPlayerTriggerEnemyTrigger;
    public event Action<CollisionEventData> OnEnterPlayerSolidEnemyTrigger;
    public event Action<CollisionEventData> OnEnterPlayerSolidEnemySolid;
    
    public event Action<CollisionEventData> OnExitPlayerTrigger;
    public event Action<CollisionEventData> OnEnterPlayerTrigger;
    
    
    private void OnEnable()
    {
        if (hub == null)
        {
            Debug.LogError($"[HeroLogic] hub не назначен у {name}", this);
            return;
        }

        hub.OnCollisionEvent += OnCol;
    }

    private void OnDisable()
    {
        if (hub == null) return;
        hub.OnCollisionEvent -= OnCol;
    }

    private void OnCol(CollisionEventData data)
    {
        if (data.Phase == CollisionPhase.Stay) return;

        string role = data.SelfCollider.name;
        bool isOuter = role == "OuterTrigger";
        bool isInner = role == gameObject.name; // Player(Clone)
        bool otherIsTrigger = data.OtherCollider != null && data.OtherCollider.isTrigger;

        if (isOuter && data.Kind == CollisionKind.Trigger && !otherIsTrigger)
        {
            //Debug.Log("1. внешний trigger + вражеский solid");
            if (data.Phase == CollisionPhase.Enter)
            {
                OnEnterPlayerTriggerEnemySolid?.Invoke(data);
                OnEnterPlayerTrigger?.Invoke(data);
            }
            else if (data.Phase == CollisionPhase.Exit)
                OnExitPlayerTrigger?.Invoke(data);
        }
        else if (isInner && data.Kind == CollisionKind.Collision)
        {
            //Debug.Log("2. внутренний solid + вражеский solid");
            if (data.Phase == CollisionPhase.Enter)
                OnEnterPlayerSolidEnemySolid?.Invoke(data);
        }
        else if (isOuter && data.Kind == CollisionKind.Trigger && otherIsTrigger)
        {
            //Debug.Log("3. внешний trigger + вражеский trigger");
            if (data.Phase == CollisionPhase.Enter)
            {
                OnEnterPlayerTriggerEnemyTrigger?.Invoke(data);
                OnEnterPlayerTrigger?.Invoke(data);
            }
            else if (data.Phase == CollisionPhase.Exit)
                OnExitPlayerTrigger?.Invoke(data);
        }
        else if (isInner && data.Kind == CollisionKind.Trigger && otherIsTrigger)
        {
            //Debug.Log("4. внутренний solid + вражеский trigger");
            if (data.Phase == CollisionPhase.Enter)
                OnEnterPlayerSolidEnemyTrigger?.Invoke(data);
        }
    }
}
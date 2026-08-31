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
    
    private enum HeroColliderRole { Unspecified, OuterTrigger, InnerCollider }
    
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

        var role = ResolveRole(data.SelfCollider.name);
        bool isOuter = role == HeroColliderRole.OuterTrigger;
        bool isInner = role == HeroColliderRole.InnerCollider;
        
        bool otherIsTrigger = data.OtherCollider != null && data.OtherCollider.isTrigger;

        if (isOuter && data.Kind == CollisionKind.Trigger && !otherIsTrigger)
        {
//            Debug.Log($"1. внешний trigger + вражеский solid {data.OtherCollider.gameObject.name}");
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
   //         Debug.Log($"2. внутренний solid + вражеский solid {data.OtherCollider.gameObject.name}");
            if (data.Phase == CollisionPhase.Enter)
                OnEnterPlayerSolidEnemySolid?.Invoke(data);
        }
        else if (isOuter && data.Kind == CollisionKind.Trigger && otherIsTrigger)
        {
     //       Debug.Log($"3. внешний trigger + вражеский trigger {data.OtherCollider.gameObject.name}");
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
     //       Debug.Log($"4. внутренний solid + вражеский trigger {data.OtherCollider.gameObject.name}");
            if (data.Phase == CollisionPhase.Enter)
                OnEnterPlayerSolidEnemyTrigger?.Invoke(data);
        }
    }
    
    private HeroColliderRole ResolveRole(string selfName)
    {
        switch (selfName)
        {
            case nameof(HeroColliderRole.OuterTrigger): return HeroColliderRole.OuterTrigger;
            case nameof(HeroColliderRole.InnerCollider): return HeroColliderRole.InnerCollider;
            default:
                Debug.LogWarning(
                    $"[HeroLogic] Имя коллайдера \"{selfName}\" не совпадает ни с одной известной ролью героя " +
                    $"(\"{nameof(HeroColliderRole.OuterTrigger)}\" / \"{nameof(HeroColliderRole.InnerCollider)}\").", this);
                return HeroColliderRole.Unspecified;
        }
    }
}
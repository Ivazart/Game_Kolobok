using UnityEngine;

public class HeroLogic : MonoBehaviour
{
    // Роли специфичны для героя: именно HeroLogic знает, что у его коллайдеров
    // есть "внешний" и "внутренний" — другой объект (враг, снаряд и т.п.)
    // будет иметь свой скрипт с собственным набором ролей, никак не связанным с этим enum.
    private enum HeroColliderRole { Unspecified, OuterTrigger, InnerCollider }

    public CollisionHub hub;

    void OnEnable()
    {
        if (hub == null)
        {
            Debug.LogError($"[HeroLogic] hub не назначен у {name}", this);
            return;
        }

        hub.OnCollisionEvent += OnCol;
    }

    void OnDisable()
    {
        if (hub == null) return;
        hub.OnCollisionEvent -= OnCol;
    }

    void OnCol(CollisionEventData data)
    {
        if (data.Phase != CollisionPhase.Enter) return;

        var role = ResolveRole(data.SelfCollider.name);
        bool isOuter = role == HeroColliderRole.OuterTrigger;
        bool isInner = role == HeroColliderRole.InnerCollider;
        bool otherIsTrigger = data.OtherCollider != null && data.OtherCollider.isTrigger;

        if (isOuter && data.Kind == CollisionKind.Trigger && !otherIsTrigger)
        {
            Debug.Log("1. внешний trigger + вражеский solid");
        }
        else if (isInner && data.Kind == CollisionKind.Collision)
        {
            Debug.Log("2. внутренний solid + вражеский solid");
        }
        else if (isOuter && data.Kind == CollisionKind.Trigger && otherIsTrigger)
        {
            Debug.Log("3. внешний trigger + вражеский trigger");
        }
        else if (isInner && data.Kind == CollisionKind.Trigger && otherIsTrigger)
        {
            Debug.Log("4. внутренний solid + вражеский trigger");
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
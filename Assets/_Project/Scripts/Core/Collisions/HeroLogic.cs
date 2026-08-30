using UnityEngine;

public class HeroLogic : MonoBehaviour
{
    [SerializeField] private CollisionHub hub;

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
        if (data.Phase != CollisionPhase.Enter) return;

        string role = data.SelfCollider.name;
        bool isOuter = role == "OuterTrigger";
        bool isInner = role == gameObject.name; // Player(Clone)
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
}
using UnityEngine;

public class CollisionEventData
{
    public Collider2D SelfCollider { get; }
    public Collider2D OtherCollider { get; }
    public CollisionKind Kind { get; }
    public CollisionPhase Phase { get; }
    public Collision2D FullCollision { get; }

    public CollisionEventData(
        Collider2D selfCollider,
        Collider2D otherCollider,
        CollisionKind kind,
        CollisionPhase phase,
        Collision2D fullCollision = null)
    {
        SelfCollider = selfCollider;
        OtherCollider = otherCollider;
        Kind = kind;
        Phase = phase;
        FullCollision = fullCollision; // всегда null для Trigger-веток, заполняется только для Collision
    }
}
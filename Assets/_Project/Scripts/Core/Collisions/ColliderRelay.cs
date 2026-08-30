using UnityEngine;

/// <summary>
/// No need to place manually but possible. Will be placed by Hub automatically 
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ColliderRelay : MonoBehaviour
{
    private CollisionHub hub;
    private Collider2D myCollider;

    private void Awake()
    {
        myCollider = GetComponent<Collider2D>();
    }

    public void SetHub(CollisionHub owningHub)
    {
        hub = owningHub;
    }

    // ==================== Trigger ====================

    private void OnTriggerEnter2D(Collider2D other)
    {
        Send(CollisionKind.Trigger, CollisionPhase.Enter, other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Send(CollisionKind.Trigger, CollisionPhase.Stay, other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Send(CollisionKind.Trigger, CollisionPhase.Exit, other);
    }

    // ==================== Collision ====================

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Send(CollisionKind.Collision, CollisionPhase.Enter, collision.collider, collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Send(CollisionKind.Collision, CollisionPhase.Stay, collision.collider, collision);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        Send(CollisionKind.Collision, CollisionPhase.Exit, collision.collider, collision);
    }

    // ==================== Internal ====================

    private void Send(CollisionKind kind, CollisionPhase phase, Collider2D other, Collision2D fullCollision = null)
    {
        if (hub == null)
        {
            Debug.LogWarning(
                $"[ColliderRelay] No hub for {name}. Call CollisionHub.EnsureRelay first", this);
            return;
        }

        var data = new CollisionEventData(
            myCollider,
            other,
            kind,
            phase,
            fullCollision
        );

        hub.Raise(data);
    }
}
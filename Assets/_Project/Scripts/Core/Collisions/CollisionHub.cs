using UnityEngine;

/// <summary>
/// Just Place it on a root object
/// </summary>
public class CollisionHub : MonoBehaviour
{
    [Header("Auto Setup")]
    [Tooltip("Автоматически добавлять ColliderRelay на все дочерние коллайдеры")]
    [SerializeField] private bool autoAddRelays = true;

    // Только код: подписка через event, без UnityEvent/инспектора
    public event System.Action<CollisionEventData> OnCollisionEvent;

    private void Awake()
    {
        if (autoAddRelays)
            AutoAddRelays();
    }

    private void AutoAddRelays()
    {
        var colliders = GetComponentsInChildren<Collider2D>(includeInactive: true);

        foreach (var col in colliders)
            EnsureRelay(col);
    }

    /// <summary>
    /// For dynamic created objects. Not tested.
    /// </summary>
    public ColliderRelay EnsureRelay(Collider2D col)
    {
        if (col == null) return null;

        if (!col.TryGetComponent<ColliderRelay>(out var relay))
            relay = col.gameObject.AddComponent<ColliderRelay>();

        relay.SetHub(this);
        return relay;
    }

    public void Raise(CollisionEventData data)
    {
        OnCollisionEvent?.Invoke(data);
    }
}
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class RigidLoopMove2D : MonoBehaviour
{
    [Header("Точки пути")]
    [SerializeField] private Transform startPoint;
    [SerializeField] private Transform endPoint;

    [Header("Движение")]
    [SerializeField] private float speed = 2f;

    [Header("Связанные физические объекты")]
    [Tooltip("Все Rigidbody2D, которые физически привязаны к этому объекту. Например, через Joint2D.")]
    [SerializeField] private Rigidbody2D[] linkedBodies;

    [Tooltip("Автоматически добавить Rigidbody2D из дочерних объектов.")]
    [SerializeField] private bool includeChildrenRigidbodies = true;

    private Rigidbody2D rb;

    private Vector2 direction;
    private float pathLength;

    private Rigidbody2D[] allBodies;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        rb.useFullKinematicContacts = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        BuildBodyList();
    }

    private void FixedUpdate()
    {
        if (startPoint == null || endPoint == null)
            return;

        Vector2 start = startPoint.position;
        Vector2 end = endPoint.position;

        direction = (end - start).normalized;
        pathLength = Vector2.Distance(start, end);

        if (pathLength <= 0.001f)
            return;

        Vector2 nextPosition = rb.position + direction * speed * Time.fixedDeltaTime;

        float distanceFromStart = Vector2.Dot(nextPosition - start, direction);

        if (distanceFromStart >= pathLength)
        {
            TeleportWholeAssembly(start);
            return;
        }

        rb.MovePosition(nextPosition);
    }

    private void TeleportWholeAssembly(Vector2 newMainPosition)
    {
        Vector2 delta = newMainPosition - rb.position;

        if (allBodies == null || allBodies.Length == 0)
        {
            rb.position = newMainPosition;
            return;
        }

        for (int i = 0; i < allBodies.Length; i++)
        {
            Rigidbody2D body = allBodies[i];

            if (body == null)
                continue;

            body.position += delta;
        }

        Physics2D.SyncTransforms();
    }

    private void BuildBodyList()
    {
        if (includeChildrenRigidbodies)
        {
            Rigidbody2D[] childrenBodies = GetComponentsInChildren<Rigidbody2D>();

            int linkedCount = linkedBodies != null ? linkedBodies.Length : 0;

            allBodies = new Rigidbody2D[childrenBodies.Length + linkedCount];

            int index = 0;

            for (int i = 0; i < childrenBodies.Length; i++)
            {
                allBodies[index] = childrenBodies[i];
                index++;
            }

            if (linkedBodies != null)
            {
                for (int i = 0; i < linkedBodies.Length; i++)
                {
                    allBodies[index] = linkedBodies[i];
                    index++;
                }
            }
        }
        else
        {
            int linkedCount = linkedBodies != null ? linkedBodies.Length : 0;

            allBodies = new Rigidbody2D[linkedCount + 1];

            allBodies[0] = rb;

            if (linkedBodies != null)
            {
                for (int i = 0; i < linkedBodies.Length; i++)
                {
                    allBodies[i + 1] = linkedBodies[i];
                }
            }
        }
    }
}

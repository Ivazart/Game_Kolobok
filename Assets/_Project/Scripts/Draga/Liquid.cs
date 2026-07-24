using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Liquid2D : MonoBehaviour
{
    [Header("Основные настройки жидкости")]
    [SerializeField] private float fluidDensity = 2.0f;
    [SerializeField] private float linearWaterDrag = 4.0f;
    [SerializeField] private float angularWaterDrag = 5.0f;
    [SerializeField] private float maxSubmersionDepth = 1.0f;

    [Header("Поверхность жидкости")]
    [SerializeField] private bool useColliderTopAsSurface = true;
    [SerializeField] private float surfaceOffset = 0.0f;

    [Header("Точки расчёта плавучести")]
    [SerializeField] private int horizontalSamples = 7;
    [SerializeField] private int verticalSamples = 3;

    [Header("Стабилизация длинных объектов")]
    [SerializeField] private bool stabilizeLongObjects = true;
    [SerializeField] private float minAspectRatioForStabilization = 1.5f;
    [SerializeField] private float stabilizationStrength = 5.0f;
    [SerializeField] private float stabilizationDamping = 2.5f;
    [Range(0f, 1f)]
    [SerializeField] private float minSubmergedRatioForStabilization = 0.15f;

    [Header("Ограничения")]
    [SerializeField] private float maxForcePerPoint = 0.0f;
    [SerializeField] private bool ignoreOtherTriggers = true;

    private Collider2D liquidCollider;

    private void Awake()
    {
        liquidCollider = GetComponent<Collider2D>();
        liquidCollider.isTrigger = true;
    }

    private void Reset()
    {
        liquidCollider = GetComponent<Collider2D>();
        liquidCollider.isTrigger = true;
    }

    private void OnValidate()
    {
        horizontalSamples = Mathf.Max(1, horizontalSamples);
        verticalSamples = Mathf.Max(1, verticalSamples);
        maxSubmersionDepth = Mathf.Max(0.01f, maxSubmersionDepth);
        minAspectRatioForStabilization = Mathf.Max(1f, minAspectRatioForStabilization);

        Collider2D col = GetComponent<Collider2D>();

        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (ignoreOtherTriggers && other.isTrigger)
            return;

        Rigidbody2D rb = other.attachedRigidbody;

        if (rb == null)
            return;

        if (rb.bodyType != RigidbodyType2D.Dynamic)
            return;

        ApplyBuoyancy(rb, other);
    }

    private void ApplyBuoyancy(Rigidbody2D rb, Collider2D objectCollider)
    {
        Bounds bounds = objectCollider.bounds;

        float objectWidth = bounds.size.x;
        float objectHeight = bounds.size.y;

        if (objectWidth <= 0.001f || objectHeight <= 0.001f)
            return;

        float gravity = Mathf.Abs(Physics2D.gravity.y);

        if (gravity <= 0.001f)
            gravity = 9.81f;

        float surfaceY = GetSurfaceY();

        int totalSamples = horizontalSamples * verticalSamples;
        float estimatedArea = objectWidth * objectHeight;
        float sampleArea = estimatedArea / totalSamples;

        int submergedSamples = 0;

        for (int x = 0; x < horizontalSamples; x++)
        {
            for (int y = 0; y < verticalSamples; y++)
            {
                float normalizedX = (x + 0.5f) / horizontalSamples;
                float normalizedY = (y + 0.5f) / verticalSamples;

                Vector2 samplePoint = new Vector2(
                    Mathf.Lerp(bounds.min.x, bounds.max.x, normalizedX),
                    Mathf.Lerp(bounds.min.y, bounds.max.y, normalizedY)
                );

                if (!liquidCollider.OverlapPoint(samplePoint))
                    continue;

                if (samplePoint.y > surfaceY)
                    continue;

                float depth = surfaceY - samplePoint.y;
                float submersionFactor = Mathf.Clamp01(depth / maxSubmersionDepth);

                if (submersionFactor <= 0.001f)
                    continue;

                submergedSamples++;

                Vector2 pointVelocity = rb.GetPointVelocity(samplePoint);

                Vector2 buoyancyForce =
                    Vector2.up *
                    fluidDensity *
                    sampleArea *
                    gravity *
                    submersionFactor;

                Vector2 dragForce =
                    -pointVelocity *
                    linearWaterDrag *
                    sampleArea *
                    submersionFactor;

                Vector2 totalForce = buoyancyForce + dragForce;

                if (maxForcePerPoint > 0.0f)
                {
                    totalForce = Vector2.ClampMagnitude(totalForce, maxForcePerPoint);
                }

                rb.AddForceAtPosition(totalForce, samplePoint, ForceMode2D.Force);
            }
        }

        if (submergedSamples > 0)
        {
            float submergedRatio = submergedSamples / (float)totalSamples;

            rb.AddTorque(
                -rb.angularVelocity * angularWaterDrag * submergedRatio,
                ForceMode2D.Force
            );

            if (stabilizeLongObjects)
            {
                ApplyLongObjectStabilization(rb, objectCollider, submergedRatio);
            }
        }
    }

    private void ApplyLongObjectStabilization(
        Rigidbody2D rb,
        Collider2D objectCollider,
        float submergedRatio)
    {
        if (submergedRatio < minSubmergedRatioForStabilization)
            return;

        Vector2 size = GetApproximateLocalColliderSize(objectCollider);

        float width = Mathf.Max(size.x, 0.001f);
        float height = Mathf.Max(size.y, 0.001f);

        float aspectRatio = Mathf.Max(width, height) / Mathf.Min(width, height);

        if (aspectRatio < minAspectRatioForStabilization)
            return;

        float localLongAxisAngle = width >= height ? 0f : 90f;

        float worldLongAxisAngle = rb.rotation + localLongAxisAngle;

        float errorTo0 = Mathf.DeltaAngle(worldLongAxisAngle, 0f);
        float errorTo180 = Mathf.DeltaAngle(worldLongAxisAngle, 180f);

        float angleError = Mathf.Abs(errorTo0) < Mathf.Abs(errorTo180)
            ? errorTo0
            : errorTo180;

        float torque =
            angleError *
            stabilizationStrength *
            submergedRatio *
            rb.mass
            -
            rb.angularVelocity *
            stabilizationDamping *
            submergedRatio *
            rb.mass;

        rb.AddTorque(torque, ForceMode2D.Force);
    }

    private Vector2 GetApproximateLocalColliderSize(Collider2D col)
    {
        if (col is BoxCollider2D box)
            return box.size;

        if (col is CapsuleCollider2D capsule)
            return capsule.size;

        if (col is CircleCollider2D circle)
            return Vector2.one * circle.radius * 2f;

        if (col is PolygonCollider2D polygon)
        {
            Bounds localBounds = new Bounds();
            bool initialized = false;

            for (int p = 0; p < polygon.pathCount; p++)
            {
                Vector2[] path = polygon.GetPath(p);

                for (int i = 0; i < path.Length; i++)
                {
                    Vector2 point = path[i] + polygon.offset;

                    if (!initialized)
                    {
                        localBounds = new Bounds(point, Vector3.zero);
                        initialized = true;
                    }
                    else
                    {
                        localBounds.Encapsulate(point);
                    }
                }
            }

            if (initialized)
                return localBounds.size;
        }

        return col.bounds.size;
    }

    private float GetSurfaceY()
    {
        if (useColliderTopAsSurface)
        {
            return liquidCollider.bounds.max.y + surfaceOffset;
        }

        return transform.position.y + surfaceOffset;
    }

    private void OnDrawGizmosSelected()
    {
        Collider2D col = GetComponent<Collider2D>();

        if (col == null)
            return;

        float surfaceY;

        if (useColliderTopAsSurface)
        {
            surfaceY = col.bounds.max.y + surfaceOffset;
        }
        else
        {
            surfaceY = transform.position.y + surfaceOffset;
        }

        Gizmos.color = Color.cyan;

        Vector3 left = new Vector3(col.bounds.min.x, surfaceY, 0f);
        Vector3 right = new Vector3(col.bounds.max.x, surfaceY, 0f);

        Gizmos.DrawLine(left, right);
    }
}

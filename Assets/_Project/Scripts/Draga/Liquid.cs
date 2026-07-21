using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Liquid2D : MonoBehaviour
{
    [Header("Основные настройки жидкости")]
    [Tooltip("Плотность жидкости. Чем больше значение, тем сильнее объект выталкивается вверх.")]
    [SerializeField] private float fluidDensity = 2.0f;

    [Tooltip("Сопротивление жидкости движению объекта.")]
    [SerializeField] private float linearWaterDrag = 4.0f;

    [Tooltip("Сопротивление жидкости вращению объекта.")]
    [SerializeField] private float angularWaterDrag = 2.0f;

    [Tooltip("Глубина, на которой точка считается полностью погруженной.")]
    [SerializeField] private float maxSubmersionDepth = 1.0f;

    [Header("Поверхность жидкости")]
    [Tooltip("Если включено, верхняя граница коллайдера считается поверхностью жидкости.")]
    [SerializeField] private bool useColliderTopAsSurface = true;

    [Tooltip("Смещение поверхности жидкости. Работает от верхней границы коллайдера или от позиции объекта.")]
    [SerializeField] private float surfaceOffset = 0.0f;

    [Header("Точки расчёта плавучести")]
    [Tooltip("Количество точек по горизонтали. Больше — точнее наклон и плавание.")]
    [SerializeField] private int horizontalSamples = 3;

    [Tooltip("Количество точек по вертикали. Больше — точнее погружение.")]
    [SerializeField] private int verticalSamples = 3;

    [Header("Ограничения")]
    [Tooltip("Максимальная сила в одной точке. 0 — без ограничения.")]
    [SerializeField] private float maxForcePerPoint = 0.0f;

    [Tooltip("Игнорировать триггер-коллайдеры объектов.")]
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

                Vector2 buoyancyForce = Vector2.up * fluidDensity * sampleArea * gravity * submersionFactor;

                Vector2 dragForce = -pointVelocity * linearWaterDrag * sampleArea * submersionFactor;

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

            float angularDampingForce = -rb.angularVelocity * angularWaterDrag * submergedRatio;

            rb.AddTorque(angularDampingForce, ForceMode2D.Force);
        }
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

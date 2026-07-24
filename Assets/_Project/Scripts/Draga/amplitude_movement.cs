using UnityEngine;

public class SmoothObjectMove2D : MonoBehaviour
{
    [Header("Движение по X")]
    [SerializeField] private bool moveX = true;
    [SerializeField] private float amplitudeX = 1f;
    [SerializeField] private float speedX = 1f;

    [Header("Движение по Y")]
    [SerializeField] private bool moveY = true;
    [SerializeField] private float amplitudeY = 1f;
    [SerializeField] private float speedY = 1f;

    [Header("Настройки")]
    [SerializeField] private bool useLocalPosition = false;
    [SerializeField] private bool randomStartOffset = false;

    private Vector3 startPosition;
    private float timeOffset;

    private void Start()
    {
        startPosition = useLocalPosition ? transform.localPosition : transform.position;

        if (randomStartOffset)
        {
            timeOffset = Random.Range(0f, 100f);
        }
    }

    private void Update()
    {
        float time = Time.time + timeOffset;

        float offsetX = moveX ? Mathf.Sin(time * speedX) * amplitudeX : 0f;
        float offsetY = moveY ? Mathf.Sin(time * speedY) * amplitudeY : 0f;

        Vector3 newPosition = startPosition + new Vector3(offsetX, offsetY, 0f);

        if (useLocalPosition)
        {
            transform.localPosition = newPosition;
        }
        else
        {
            transform.position = newPosition;
        }
    }
}
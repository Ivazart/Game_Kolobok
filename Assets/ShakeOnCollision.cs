using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeOnCollision : MonoBehaviour
{
    public ObjectEmitter2D emitter;
    public string targetTag = "Player"; // Тег объекта, с которым должно быть столкновение
    public float shakeIntensity = 0.5f;  // Интенсивность тряски
    public float shakeDuration = 1f;     // Длительность тряски
    public float shakeFrequency = 0.05f; // Частота тряски (чем меньше, тем быстрее)

    private bool isShaking = false;      // Флаг: трясётся ли объект
    private Vector3 originalPosition;    // Оригинальная позиция объекта
    private float shakeTimeRemaining;    // Оставшееся время тряски

    // Start is called before the first frame update
    void Start()
    {
        originalPosition = transform.localPosition;

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Проверяем, если объект столкнулся с определённым тегом
        if (collision.gameObject.CompareTag(targetTag) && !isShaking)
        {
            StartShake();
            emitter.isSpawning = true;
        }
    }

    private void Update()
    {
        // Если объект трясётся, выполняем тряску
        if (isShaking)
        {
            if (shakeTimeRemaining > 0)
            {
                PerformShake();
                shakeTimeRemaining -= Time.deltaTime;
            }
            else
            {
                StopShake();
            }
        }
    }
    private void StartShake()
    {
        // Начинаем тряску
        isShaking = true;
        shakeTimeRemaining = shakeDuration;
    }
    // Update is called once per frame
    private void PerformShake()
    {
        // Генерируем случайное смещение в пределах интенсивности
        float offsetX = Random.Range(-shakeIntensity, shakeIntensity);
        float offsetY = Random.Range(-shakeIntensity, shakeIntensity);
        float offsetZ = Random.Range(-shakeIntensity, shakeIntensity);

        // Позиция во время тряски
        transform.localPosition = originalPosition + new Vector3(offsetX, offsetY, offsetZ);
    }

    private void StopShake()
    {
        // Останавливаем тряску и возвращаем объект в исходное положение
        isShaking = false;
        transform.localPosition = originalPosition;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeOnCollision : MonoBehaviour
{
    [SerializeField] private ObjectEmitter2D emitter;
    [SerializeField] private string targetTag = "Player"; // Тег объекта, с которым должно быть столкновение
    [SerializeField] private float shakeIntensity = 0.5f;  // Интенсивность тряски
    [SerializeField] private float shakeDuration = 1f;     // Длительность тряски
    //[SerializeField] private float shakeFrequency = 0.05f; // Частота тряски (чем меньше, тем быстрее)

    private bool isShaking = false;      // Флаг: трясётся ли объект
    private Vector3 originalPosition;    // Оригинальная позиция объекта
    private float shakeTimeRemaining;    // Оставшееся время тряски

    // Start is called before the first frame update
    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    private void OnCollisionEnter2D(Collision2D collision)
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

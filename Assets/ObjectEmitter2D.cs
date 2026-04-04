using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class ObjectEmitter2D : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject objectToSpawn; // Префаб объекта для создания
    public bool isSpawning = false;  // Флаг включения/выключения генерации
    public float spawnInterval = 1f; // Интервал генерации

    [Header("Force Settings")]
    public float force = 5f;         // Сила прикладываемая к объекту
    public Vector2 direction = Vector2.up; // Направление прикладываемой силы

    [Header("Randomization Settings")]
    public float directionSpread = 10f;  // Разброс направления
    public float forceSpread = 2f;       // Разброс силы
    public float minScale = 0.5f;        // Минимальный масштаб
    public float maxScale = 1.5f;        // Максимальный масштаб

    [Header("Timing Settings")]
    public float activeDuration = 5f;    // Длительность активной генерации
    public float inactiveDuration = 5f;  // Длительность неактивного состояния

    private void Start()
    {
        // Запускаем корутину для генерации объектов
        StartCoroutine(SpawnerRoutine());
    }

    IEnumerator SpawnerRoutine()
    {
        while (true)
        {
            if (isSpawning)
            {
                // Активная фаза генерации
                yield return StartCoroutine(SpawnObjects());

                // Переход в неактивное состояние
                Debug.Log("Spawning Disabled");
                yield return new WaitForSeconds(inactiveDuration);
            }
            else
            {
                yield return null;
            }
        }
    }

    IEnumerator SpawnObjects()
    {
        float spawnEndTime = Time.time + activeDuration;
        while (Time.time < spawnEndTime)
        {
            // Создаем объект
            GameObject newObject = Instantiate(objectToSpawn, transform.position, Quaternion.identity);

            // Настраиваем случайный масштаб
            float randomScale = Random.Range(minScale, maxScale);
            newObject.transform.localScale = new Vector3(randomScale, randomScale, 1f);

            // Получаем Rigidbody2D
            Rigidbody2D rb = newObject.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Генерируем и нормализуем случайное направление
                Vector2 randomizedDirection = direction + new Vector2(
                    Random.Range(-directionSpread, directionSpread),
                    Random.Range(-directionSpread, directionSpread)
                ).normalized;

                // Генерируем случайную силу в заданном диапазоне
                float randomizedForce = force + Random.Range(-forceSpread, forceSpread);

                // Применяем силу
                rb.AddForce(randomizedDirection * randomizedForce, ForceMode2D.Impulse);
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
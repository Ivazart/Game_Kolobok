using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroer : MonoBehaviour
{
    [Header("Destruction Settings")]
    public float targetYPosition = -10f; // Целевая позиция по оси Y для удаления объекта

    void Update()
    {
        // Проверяем текущую позицию объекта по оси Y
        if (transform.position.y <= targetYPosition)
        {
            // Удаляем объект
            Destroy(gameObject);
        }
    }
}

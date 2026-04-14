using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyer : MonoBehaviour
{
    [Header("Destruction Settings")]
    [SerializeField] private float targetYPosition = -10f; // Целевая позиция по оси Y для удаления объекта

    private void Update()
    {
        // Проверяем текущую позицию объекта по оси Y
        if (transform.position.y <= targetYPosition)
        {
            // Удаляем объект
            Destroy(gameObject);
        }
    }
}

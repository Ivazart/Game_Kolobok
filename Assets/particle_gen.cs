using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particle_gen : MonoBehaviour
{
    // Интервалы времени для включения/выключения
    public float activateTime = 2f; // Время активности частиц
    public float deactivateTime = 2f; // Время неактивности частиц

    private new ParticleSystem particleSystem; // Ссылка на компонент Particle System
    private bool isActive = false; // Переключатель состояния (включено/выключено)

    void Start()
    {
        // Получаем компонент ParticleSystem на объекте
        particleSystem = GetComponent<ParticleSystem>();

        if (particleSystem == null)
        {
            Debug.LogError("На объекте не найден ParticleSystem!");
            return;
        }

        // Запускаем переключение
        StartCoroutine(ToggleParticles());
    }

    private System.Collections.IEnumerator ToggleParticles()
    {
        while (true)
        {
            // Переключаем состояние
            if (isActive)
            {
                particleSystem.Stop(); // Останавливаем генерацию частиц
                isActive = false;
                yield return new WaitForSeconds(deactivateTime); // Ждем время неактивности
            }
            else
            {
                particleSystem.Play(); // Включаем генерацию частиц
                isActive = true;
                yield return new WaitForSeconds(activateTime); // Ждем время активности
            }
        }
    }
}

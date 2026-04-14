using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleGenerator : MonoBehaviour
{
    // Интервалы времени для включения/выключения
    [SerializeField] private float activateTime = 2f; // Время активности частиц
    [SerializeField] private float deactivateTime = 2f; // Время неактивности частиц

    private ParticleSystem particleSystemComponent; // Ссылка на компонент Particle System
    private bool isActive = false; // Переключатель состояния (включено/выключено)

    private void Start()
    {
        // Получаем компонент ParticleSystem на объекте
        particleSystemComponent = GetComponent<ParticleSystem>();

        if (particleSystemComponent == null)
        {
            Debug.LogError("На объекте не найден ParticleSystem!");
            return;
        }
        // Запускаем переключение
        StartCoroutine(ToggleParticles());
    }

    private IEnumerator ToggleParticles()
    {
        while (true)
        {
            // Переключаем состояние
            if (isActive)
            {
                particleSystemComponent.Stop(); // Останавливаем генерацию частиц
                isActive = false;
                yield return new WaitForSeconds(deactivateTime); // Ждем время неактивности
            }
            else
            {
                particleSystemComponent.Play(); // Включаем генерацию частиц
                isActive = true;
                yield return new WaitForSeconds(activateTime); // Ждем время активности
            }
        }
    }
}

using System;
using UnityEngine;

public class Throwable : MonoBehaviour
{
    [SerializeField] float delay = 3f;
    [SerializeField] float damageRadius = 50f;
    [SerializeField] float explosionForce = 4000f;

    float countdown;

    bool hasExploded = false;
    public bool hasBeenThrown = false;

    public enum ThrowableType 
    { 
        None,
        HighExplosiveGrenade,
        SmokeGrenade
    }

    public ThrowableType throwableType;

    private void Start()
    {
        countdown = delay;
    }

    private void Update()
    {
        if (hasBeenThrown)
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0f && hasExploded == false)
            {
                Explode();
            }
        }
    }

    // Метод, срабатывающий при взрыве:
    private void Explode()
    {
        // Запускаем эффект:
        GetThrowableEffect();

        // Уничтожаем объект со сцены:
        Destroy(gameObject);

        hasExploded = true;
    }

    // Метод для получения эффекта в зависимости от типа:
    private void GetThrowableEffect()
    {
        switch (throwableType)
        {
            case ThrowableType.HighExplosiveGrenade:
                HighExplosiveEffect();
                break;
            case ThrowableType.SmokeGrenade:
                SmokeGrenadeEffect();
                break;
            default:
                print("UNKNOWN THROWABLE TYPE!");
                break;
        }
    }

    // Метод для запуска эффекта от осколочной гранаты:
    private void HighExplosiveEffect()
    {
        // ВИЗУАЛЬНЫЙ ЭФФЕКТ:
        // Получаем ВИЗУАЛЬНЫЙ эффект взрыва осколочной гранаты:
        GameObject explosionEffect = GlobalReferences.Instance.highExplosionGrenadeEffect;

        // Создаем эффект взрыва осколочной гранаты в сцене:
        Instantiate(explosionEffect, transform.position, transform.rotation);

        // Проигрываем звук взрыва:
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.highExplosiveGrenadeSound);

        // ФИЗИЧЕСКИЙ ЭФФЕКТ:
        // Полезная инфа: Physics.OverlapSphere() определяет все Collider, которые задеты сферой
        // Получаем Collider всех объектом в радиусе взрыва:
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, damageRadius);

        // Проходимся по всем Collider, которые зацеплены гранатой:
        foreach (Collider colliderInRange in collidersInRange)
        {
            // Пытаемся получить Rigidbody у объекта, которому принадлежит данный Collider:
            Rigidbody rb = colliderInRange.GetComponent<Rigidbody>();

            // Если удалось получить Rigidbody, то применяем к объекту взрывную силу:
            if(rb != null)
            {
                // Полезная инфа: метод AddExplosionForce() объекта Rigidbody применяет к нему силу взрыва
                rb.AddExplosionForce(explosionForce, transform.position, damageRadius);
            }
        }
    }

    // Метод для запуска эффекта дымовой гранаты:
    private void SmokeGrenadeEffect()
    {
        // ВИЗУАЛЬНЫЙ ЭФФЕКТ:
        // Получаем ВИЗУАЛЬНЫЙ эффект дымовой гранаты:
        GameObject smokeEffect = GlobalReferences.Instance.smokeGrenadeEffect;

        // Создаем эффект взрыва выхода дыма в сцене:
        Instantiate(smokeEffect, transform.position, transform.rotation);

        // Проигрываем звук выхода дыма:
        SoundManager.Instance.throwablesChannel.PlayOneShot(SoundManager.Instance.smokeGrenadeSound);

        // ФИЗИЧЕСКИЙ ЭФФЕКТ
        // Получаем Collider всех объектом в радиусе действия:
        Collider[] collidersInRange = Physics.OverlapSphere(transform.position, damageRadius);

        // ФИЗИЧЕСКИЙ ЭФФЕКТ
        // Проходимся по всем Collider, которые зацеплены гранатой:
        foreach (Collider colliderInRange in collidersInRange)
        {
            // Пытаемся получить Rigidbody у объекта, которому принадлежит данный Collider:
            Rigidbody rb = colliderInRange.GetComponent<Rigidbody>();

            // Если удалось получить Rigidbody, то применяем к объекту дымовой эффект:
            if (rb != null)
            {
                // TODO: Предусмотреть ослепление мобов
            }
        }
    }
}

using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    // Стрельба:
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    // Стрельба очередью:
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    // Разброс:
    public float spreadIntensity;

    // Пуля:
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 100F;
    public float bulletPrefabLifeTime = 3F;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    public ShootingMode currentShootingMode;

    // Бинды:
    public const KeyCode SHOOT_KEY_CODE = KeyCode.Mouse0;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
    }

    private void Update()
    {
        // Полезная инфа:
        // Input.GetKey() срабатывает, когда кнопка зажата
        // Input.GetKeyDown() срабатывает, когда кнопка нажата

        // При автоматическом режиме стрельба будет вестись только тогда, когда игрок зажимает кнопку:
        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(SHOOT_KEY_CODE);
        } else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(SHOOT_KEY_CODE);
        }

        if (readyToShoot && isShooting)
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }
    }

    private void FireWeapon()
    {
        // Сразу меняем на ЛОЖЬ, потому что мы не хотим стрелять сразу снова:
        readyToShoot = false;

        // Задаем направление стрельбы:
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // Можно по-разному определять стрельбу (например, создавать лучи). В данном случае по-настоящему создается пуля:
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        // Направляем пулю в сторону "съемки":
        bullet.transform.forward = shootingDirection;

        // Задаем движение пуле
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // Уничтожаем пулю через некоторое время:
        // Coroutine – действие, которое заканчивается через некоторое время
        StartCoroutine(DestroyBulletAfterSomeTime(bullet, bulletPrefabLifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // Если стрельба очередью:
        if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        // "Стреляем" с центра камеры, чтобы проверить, во что мы попадем:
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // Когда попадаем по чему-то:
            targetPoint = hit.point;
        }
        else
        {
            // Когда стреляем по воздуху:
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        // Возвращаем направление стрельбы с учетом разброса:
        return direction + new Vector3(x, y, 0);
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowReset = true;
    }

    private IEnumerator DestroyBulletAfterSomeTime(GameObject bullet, float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(bullet);
    }
}

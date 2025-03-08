using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class Weapon : MonoBehaviour
{

    // ��������:
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    // �������� ��������:
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    // �������:
    public float spreadIntensity;

    // ����:
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

    // �����:
    public const KeyCode SHOOT_KEY_CODE = KeyCode.Mouse0;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
    }

    private void Update()
    {
        // �������� ����:
        // Input.GetKey() �����������, ����� ������ ������
        // Input.GetKeyDown() �����������, ����� ������ ������

        // ��� �������������� ������ �������� ����� ������� ������ �����, ����� ����� �������� ������:
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
        // ����� ������ �� ����, ������ ��� �� �� ����� �������� ����� �����:
        readyToShoot = false;

        // ������ ����������� ��������:
        Vector3 shootingDirection = CalculateDirectionAndSpread().normalized;

        // ����� ��-������� ���������� �������� (��������, ��������� ����). � ������ ������ ��-���������� ��������� ����:
        GameObject bullet = Instantiate(bulletPrefab, bulletSpawn.position, Quaternion.identity);

        // ���������� ���� � ������� "������":
        bullet.transform.forward = shootingDirection;

        // ������ �������� ����
        bullet.GetComponent<Rigidbody>().AddForce(shootingDirection * bulletVelocity, ForceMode.Impulse);

        // ���������� ���� ����� ��������� �����:
        // Coroutine � ��������, ������� ������������� ����� ��������� �����
        StartCoroutine(DestroyBulletAfterSomeTime(bullet, bulletPrefabLifeTime));

        if (allowReset)
        {
            Invoke("ResetShot", shootingDelay);
            allowReset = false;
        }

        // ���� �������� ��������:
        if(currentShootingMode == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    private Vector3 CalculateDirectionAndSpread()
    {
        // "��������" � ������ ������, ����� ���������, �� ��� �� �������:
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit hit;

        Vector3 targetPoint;
        if (Physics.Raycast(ray, out hit))
        {
            // ����� �������� �� ����-��:
            targetPoint = hit.point;
        }
        else
        {
            // ����� �������� �� �������:
            targetPoint = ray.GetPoint(100);
        }

        Vector3 direction = targetPoint - bulletSpawn.position;

        float x = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);
        float y = UnityEngine.Random.Range(-spreadIntensity, spreadIntensity);

        // ���������� ����������� �������� � ������ ��������:
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

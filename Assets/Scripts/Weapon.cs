using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // ���������:
    private const string RECOIL_ANIM_TAG = "RECOIL";
    private const string RELOAD_ANIM_TAG = "RELOAD";

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

    // ������� ������:
    public GameObject muzzleEffect;

    // ��������:
    public Animator animator;

    // �����������:
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

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
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;
    }

    private void Update()
    {
        // �������� ����:
        // Input.GetKey() �����������, ����� ������ ������
        // Input.GetKeyDown() �����������, ����� ������ ������

        // ���� ������������ �������� ����������, �� � ������ ��� ������, �� ����������� ����:
        if(bulletsLeft <= 0 && isShooting)
        {
            // TODO: �������������� ����:
            SoundManager.Instance.AK47_EmptyMagazine.Play();
        }

        // ��� �������������� ������ �������� ����� ������� ������ �����, ����� ����� �������� ������:
        if (currentShootingMode == ShootingMode.Auto)
        {
            isShooting = Input.GetKey(SHOOT_KEY_CODE);
        } else if (currentShootingMode == ShootingMode.Single || currentShootingMode == ShootingMode.Burst)
        {
            isShooting = Input.GetKeyDown(SHOOT_KEY_CODE);
        }

        /// ������� �����������:
        // ����������� �� ������� �������:
        if (Input.GetKeyDown(KeyCode.R) && (bulletsLeft < magazineSize) && (isReloading == false))
        {
            Reload();
        }

        // ����������� �� ��������� �������� � ������:
        //if (readyToShoot && (isShooting == false) && (isReloading == false) && (bulletsLeft <= 0))
        //{
        //    Reload();
        //}

        // ��������:
        if ((readyToShoot && isShooting) && (bulletsLeft > 0))
        {
            burstBulletsLeft = bulletsPerBurst;
            FireWeapon();
        }

        // ��������� ������������ ���������� �������� � ������:
        if(AmmoManager.Instance.ammoCountDisplay != null)
        {
            AmmoManager.Instance.ammoCountDisplay.text = $"{bulletsLeft}/{magazineSize}";
        }
    }

    private void FireWeapon()
    {
        // ��������� ���������� ���� � ������:
        bulletsLeft--;

        // ��������� ������� ������:
        muzzleEffect.GetComponent<ParticleSystem>().Play();

        // ��������� �������� ��������:
        animator.SetTrigger(RECOIL_ANIM_TAG);

        // TODO: �������������� ����:
        // ��������� ���� ��������:
        SoundManager.Instance.AK47_Shot.Play();

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

    // ����� ��� �����������:
    private void Reload()
    {
        isReloading = true;

        // ��������� �������� �����������:
        animator.SetTrigger(RELOAD_ANIM_TAG);

        // TODO: �������������� ����:
        // ��������� ���� �����������:
        SoundManager.Instance.AK47_Reload.Play();
        Invoke("ReloadCompleted", reloadTime);
    }

    // ����� ��� �������������� ���������� �������� � ������:
    private void ReloadCompleted()
    {
        bulletsLeft = magazineSize;
        isReloading = false;
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

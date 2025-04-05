using NUnit.Framework.Constraints;
using System;
using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // ��������� ���������� ������:
    public bool isActive = false;

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
    internal Animator animator;

    // �����������:
    public float reloadTime;
    public int magazineSize, bulletsLeft;
    public bool isReloading;

    public enum WeaponModel
    {
        Colt1911,
        AK74
    }
    public WeaponModel thisWeaponModel;

    public enum ShootingMode
    {
        Single,
        Burst,
        Auto
    }

    private ShootingMode[] shootingModes;
    private int currentShootingModeIndex = 0;

    // �����:
    public const KeyCode SHOOT_KEY_CODE = KeyCode.Mouse0;

    // ������������ �� ������� ����:
    public Vector3 positionInFPP;
    public Vector3 rotationInFPP;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;

        // ��������� ������ �������� ��� ������� ������:
        switch (thisWeaponModel)
        {
            case WeaponModel.Colt1911:
                shootingModes = new ShootingMode[] {ShootingMode.Single };
                break;
            case WeaponModel.AK74:
                shootingModes = new ShootingMode[] {ShootingMode.Single, ShootingMode.Burst, ShootingMode.Auto };
                break;
            default:
                print("��� ������� ������ �� ������������� ������ ��������! ��������� ���!");
                break;
        }
    }

    private void Update()
    {
        if (isActive) {
            // �������� ����:
            // Input.GetKey() �����������, ����� ������ ������
            // Input.GetKeyDown() �����������, ����� ������ ������

            // �������� ������ �� � ���� ������ �� ����� ���� ��������:
            GetComponent<Outline>().enabled = false;

            // ���� ������������ �������� ����������, �� � ������ ��� ������, �� ����������� ����:
            if (bulletsLeft <= 0 && isShooting)
            {
                SoundManager.Instance.PlayEmptyMagazineSound(thisWeaponModel);
            }

            // !Hardcoded ����� �������:
            // TODO: ����� ���� ����������� ��� ���� ����� ������:
            // ����� ������ ��������:
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (shootingModes.Length > 1)
                {
                    if (currentShootingModeIndex == (shootingModes.Length - 1))
                    {
                        currentShootingModeIndex = 0;
                    }
                    else
                    {
                        currentShootingModeIndex++;
                    }

                    SoundManager.Instance.PlayChangeShootingMode(thisWeaponModel);
                }
            }

            // ���������� ����� �������� � HUD:
            if (HUDManager.Instance.shootingModeDisplay != null)
            {
                switch (shootingModes[currentShootingModeIndex])
                {
                    case ShootingMode.Single:
                        HUDManager.Instance.shootingModeDisplay.text = "Single";
                        break;
                    case ShootingMode.Burst:
                        HUDManager.Instance.shootingModeDisplay.text = "Burst";
                        break;
                    case ShootingMode.Auto:
                        HUDManager.Instance.shootingModeDisplay.text = "Auto";
                        break;
                    default:
                        HUDManager.Instance.shootingModeDisplay.text = "Unknown";
                        print("��� ������� ������ �������� �� ������������ �����!");
                        break;
                }
            }

            // ��� �������������� ������ �������� ����� ������� ������ �����, ����� ����� �������� ������:
            if (shootingModes[currentShootingModeIndex] == ShootingMode.Auto)
            {
                isShooting = Input.GetKey(SHOOT_KEY_CODE);
            } else if (shootingModes[currentShootingModeIndex] == ShootingMode.Single || shootingModes[currentShootingModeIndex] == ShootingMode.Burst)
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
            if (HUDManager.Instance.ammoCountDisplay != null)
            {
                HUDManager.Instance.ammoCountDisplay.text = $"{bulletsLeft}/{magazineSize}";
            }
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

        // ��������� ���� ��������:
        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

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
        if(shootingModes[currentShootingModeIndex] == ShootingMode.Burst && burstBulletsLeft > 1)
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

        // ��������� ���� �����������:
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);
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

using NUnit.Framework.Constraints;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class Weapon : MonoBehaviour
{
    // Индикатор активности оружия:
    public bool isActive = false;

    // Константы:
    private const string RECOIL_ANIM_TAG = "RECOIL";
    private const string RELOAD_ANIM_TAG = "RELOAD";
    private const string ENTER_ADS_ANIM_TAG = "enterADS";
    private const string EXIT_ADS_ANIM_TAG = "exitADS";
    private const string RECOIL_ADS_ANIM_TAG = "RECOIL_ADS";

    // Стрельба:
    [Header("Shooting")]
    public bool isShooting, readyToShoot;
    bool allowReset = true;
    public float shootingDelay = 2f;

    // Стрельба очередью:
    [Header("Burst")]
    public int bulletsPerBurst = 3;
    public int burstBulletsLeft;

    // Разброс:
    [Header("Spread")]
    public float spreadIntensity;
    public float hipSpreadIntensity;
    public float adsSpreadIntenisty;

    // Пуля:
    [Header("Bullet")]
    public GameObject bulletPrefab;
    public Transform bulletSpawn;
    public float bulletVelocity = 100F;
    public float bulletPrefabLifeTime = 3F;

    // Дульный эффект:
    public GameObject muzzleEffect;

    // Анимации:
    internal Animator animator;

    // Перезарядка:
    [Header("Reloading")]
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

    // Бинды:
    public const KeyCode SHOOT_KEY_CODE = KeyCode.Mouse0;

    // Расположение от первого лица:
    public Vector3 positionInFPP;
    public Vector3 rotationInFPP;

    // В прицеле или нет:
    private bool isADS;

    private void Awake()
    {
        readyToShoot = true;
        burstBulletsLeft = bulletsPerBurst;
        animator = GetComponent<Animator>();

        bulletsLeft = magazineSize;

        spreadIntensity = hipSpreadIntensity;

        // Назначаем режимы стрельбы для данного оружия:
        switch (thisWeaponModel)
        {
            case WeaponModel.Colt1911:
                shootingModes = new ShootingMode[] { ShootingMode.Single };
                break;
            case WeaponModel.AK74:
                shootingModes = new ShootingMode[] { ShootingMode.Single, ShootingMode.Burst, ShootingMode.Auto };
                break;
            default:
                print("Для данного оружия не предусмотрены режимы стрельбы! Исправьте код!");
                break;
        }
    }

    private void Update()
    {
        if (isActive) {
            // Полезная инфа:
            // Input.GetKey() срабатывает, когда кнопка зажата
            // Input.GetKeyDown() срабатывает, когда кнопка нажата

            // Активное оружие ни в коем случае не может быть выделено:
            GetComponent<Outline>().enabled = false;

            // Переходим в ADS:
            if (Input.GetMouseButtonDown(1))
            {
                EnterADS();
            }
            
            // Выходим из ADS:
            if (Input.GetMouseButtonUp(1))
            {
                ExitADS();
            }

            // Если пользователь пытается выстрелить, но в обойме нет патрон, то проигрываем звук:
            if (bulletsLeft <= 0 && isShooting)
            {
                SoundManager.Instance.PlayEmptyMagazineSound(thisWeaponModel);
            }

            // !Hardcoded смена режимов:
            // TODO: может быть неактуально для всех видов оружий:
            // Смена режима стрельбы:
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

            // При автоматическом режиме стрельба будет вестись только тогда, когда игрок зажимает кнопку:
            if (shootingModes[currentShootingModeIndex] == ShootingMode.Auto)
            {
                isShooting = Input.GetKey(SHOOT_KEY_CODE);
            } else if (shootingModes[currentShootingModeIndex] == ShootingMode.Single || shootingModes[currentShootingModeIndex] == ShootingMode.Burst)
            {
                isShooting = Input.GetKeyDown(SHOOT_KEY_CODE);
            }

            /// Триггер перезарядки:
            // Перезарядка по нажатию клавиши:
            if (Input.GetKeyDown(KeyCode.R) && (bulletsLeft < magazineSize) && (isReloading == false) && (WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel) > 0))
            {
                Reload();
            }

            // Перезарядка по окончании патронов в обойме:
            //if (readyToShoot && (isShooting == false) && (isReloading == false) && (bulletsLeft <= 0))
            //{
            //    Reload();
            //}

            // Стрельба:
            if ((readyToShoot && isShooting) && (bulletsLeft > 0))
            {
                burstBulletsLeft = bulletsPerBurst;
                FireWeapon();
            }
        }
    }

    // Вход в прицел:
    private void EnterADS()
    {
        isADS = true;
        HUDManager.Instance.crosshair.SetActive(false);
        animator.SetTrigger(ENTER_ADS_ANIM_TAG);
        spreadIntensity = adsSpreadIntenisty;
    }

    // Выход из прицела:
    private void ExitADS()
    {
        isADS = false;
        HUDManager.Instance.crosshair.SetActive(true);
        animator.SetTrigger(EXIT_ADS_ANIM_TAG);
        spreadIntensity = hipSpreadIntensity;
    }

    public ShootingMode shootingMode
    {
        get { return shootingModes[currentShootingModeIndex]; }
    }

    private void FireWeapon()
    {
        // Уменьшаем количество пуль в обойме:
        bulletsLeft--;

        // Запускаем дульный эффект:
        muzzleEffect.GetComponent<ParticleSystem>().Play();

        // Триггерим анимацию выстрела:
        if (isADS)
        {
            animator.SetTrigger(RECOIL_ADS_ANIM_TAG);
        }
        else
        {
            animator.SetTrigger(RECOIL_ANIM_TAG);
        }

        // Запускаем звук выстрела:
        SoundManager.Instance.PlayShootingSound(thisWeaponModel);

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
        if(shootingModes[currentShootingModeIndex] == ShootingMode.Burst && burstBulletsLeft > 1)
        {
            burstBulletsLeft--;
            Invoke("FireWeapon", shootingDelay);
        }
    }

    // Метод для перезарядки:
    private void Reload()
    {
        isReloading = true;

        // Триггерим анимацию перезарядки:
        animator.SetTrigger(RELOAD_ANIM_TAG);

        // Запускаем звук перезарядки:
        SoundManager.Instance.PlayReloadSound(thisWeaponModel);
        Invoke("ReloadCompleted", reloadTime);
    }

    // Метод для восстановления количества патронов в обойме:
    private void ReloadCompleted()
    {
        // Получаем количество патронов, оставшихся для данного оружия:
        int ammoAmountLeft = WeaponManager.Instance.CheckAmmoLeftFor(thisWeaponModel);
        if(ammoAmountLeft > magazineSize) 
        {
            bulletsLeft = magazineSize;
        }
        else
        {
            bulletsLeft = ammoAmountLeft;
        }

        WeaponManager.Instance.DecreaseTotalAmmoAmount(bulletsLeft, thisWeaponModel);

        isReloading = false;
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

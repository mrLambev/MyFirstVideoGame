using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
    public const KeyCode ThrowableThrowKeyCode = KeyCode.G;
    public static WeaponManager Instance { set; get; }

    // Список всех слотов для оружия:
    public List<GameObject> weaponSlots;

    // Текущий слот для оружия:
    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalPistolAmmoAmount = 0;
    public int totalRifleAmmoAmount = 0;

    // Гранаты (Throwable):
    [Header("Throwables")]
    public int highExplosiveGrenades = 0;
    public float throwForce = 10f; // - сила броска Throwable
    public GameObject highExplosiveGrenadePrefab;
    public GameObject throwableSpawn;
    public float forceMultiplier = 0f; // - множитель силы броска; нужен для того, чтобы увеличивать силу броска по мере подготовки к броску
    public float forceMultiplierLimit = 2f;

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        foreach (GameObject weaponSlot in weaponSlots)
        {
            if (weaponSlot == activeWeaponSlot)
            {
                weaponSlot.SetActive(true);
            }
            else
            {
                weaponSlot.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SwitchActiveSlot(0);
        }
        else if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            SwitchActiveSlot(1);
        }

        // Подготовка к броску гранаты (Throwable):
        if (Input.GetKey(ThrowableThrowKeyCode))
        {
            forceMultiplier += Time.deltaTime;

            // Проверяем на предоление верхнего лимита:
            if (forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }

        // Бросок гранаты (Throwable):
        //! Полезная инфа: метод Input.GetKeyUp() проверяет, поднята ли определенная клавиша
        if (Input.GetKeyUp(ThrowableThrowKeyCode))
        {
            if(highExplosiveGrenades > 0)
            {
                ThrowThrowable();
            }

            forceMultiplier = 0;
        }
    }

    public void PickupWeapon(GameObject pickedUpWeapon)
    {
        AddWeaponIntoActiveSlot(pickedUpWeapon);
    }

    // "Подбирает" оружие в слот:
    private void AddWeaponIntoActiveSlot(GameObject pickedUpWeapon)
    {
        DropCurrentWeapon(pickedUpWeapon);

        pickedUpWeapon.transform.SetParent(activeWeaponSlot.transform, false);

        Weapon weapon = pickedUpWeapon.GetComponent<Weapon>();

        pickedUpWeapon.transform.localPosition = new Vector3(weapon.positionInFPP.x, weapon.positionInFPP.y, weapon.positionInFPP.z);
        pickedUpWeapon.transform.localRotation = Quaternion.Euler(weapon.rotationInFPP.x, weapon.rotationInFPP.y, weapon.rotationInFPP.z);

        weapon.isActive = true;
        weapon.animator.enabled = true;
    }

    // Меняет местами активное оружие с подбираемым оружием:
    private void DropCurrentWeapon(GameObject pickedUpWeapon)
    {
        if (activeWeaponSlot.transform.childCount > 0)
        {
            var weaponToDrop = activeWeaponSlot.transform.GetChild(0).gameObject;

            weaponToDrop.GetComponent<Weapon>().isActive = false;
            weaponToDrop.GetComponent<Weapon>().animator.enabled = false;

            weaponToDrop.transform.SetParent(pickedUpWeapon.transform.parent);
            weaponToDrop.transform.localPosition = pickedUpWeapon.transform.localPosition;
            weaponToDrop.transform.localRotation = pickedUpWeapon.transform.localRotation;
        }
    }

    // Меняет текущий активный слот:
    public void SwitchActiveSlot(int slotNumber)
    {
        // Если в прошлом слоту было оружие, то делаем его неактивным:
        if(activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActive = false;
        }

        // Меняем текущий слот:
        activeWeaponSlot = weaponSlots[slotNumber];

        // Если в новом слоту есть оружие, то делаем его активным:
        if (activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActive = true;
        }
    }

    internal void PickupAmmo(AmmoBox ammo)
    {
        switch (ammo.ammoType)
        {
            case AmmoBox.AmmoType.PistolAmmo:
                totalPistolAmmoAmount += ammo.amount;
                break;
            case AmmoBox.AmmoType.RifleAmmo:
                totalRifleAmmoAmount += ammo.amount;
                break;
        }
    }

    // Возвращает количество патронов, которое осталось для данного оружия:
    public int CheckAmmoLeftFor(Weapon.WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case Weapon.WeaponModel.Colt1911:
                return totalPistolAmmoAmount;
            case Weapon.WeaponModel.AK74:
                return totalRifleAmmoAmount;
            default:
                return 0;
        }
    }

    // Уменьшает количество патронов:
    public void DecreaseTotalAmmoAmount(int amount, Weapon.WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case Weapon.WeaponModel.Colt1911:
                totalPistolAmmoAmount -= amount;
                break;
            case Weapon.WeaponModel.AK74:
                totalRifleAmmoAmount -= amount;
                break;
        }
    }


    #region || ---- Throwables ---- ||
    // Подбирает гранату (Throwable):
    public void PickupThrowable(Throwable throwable)
    {
        switch (throwable.throwableType)
        {
            case Throwable.ThrowableType.HighExplosiveGrenade:
                PickupHighExplosiveGrenade();
                break;
        }
    }

    private void PickupHighExplosiveGrenade()
    {
        highExplosiveGrenades += 1;

        HUDManager.Instance.UpdateThrowables(Throwable.ThrowableType.HighExplosiveGrenade);
    }

    // Бросает гранату (Throwable):
    private void ThrowThrowable()
    {
        GameObject throwablePrefab = highExplosiveGrenadePrefab;
        
        // Создаем Throwable в сцене:
        GameObject throwable = Instantiate(throwablePrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        // Применяем силу к созданному объекту:
        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        highExplosiveGrenades -= 1;

        // Обновляем UI:
        HUDManager.Instance.UpdateThrowables(Throwable.ThrowableType.HighExplosiveGrenade);
    }
    #endregion
}

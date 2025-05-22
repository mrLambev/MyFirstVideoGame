using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
    public const KeyCode WeaponSlot1KeyCode = KeyCode.Alpha1;
    public const KeyCode WeaponSlot2KeyCode = KeyCode.Alpha2;

    public const KeyCode LethalThrowableThrowKeyCode = KeyCode.G;
    public const KeyCode TacticalThrowableThrowKeyCode = KeyCode.T;
    public static WeaponManager Instance { set; get; }

    // ������ ���� ������ ��� ������:
    public List<GameObject> weaponSlots;

    // ������� ���� ��� ������:
    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalPistolAmmoAmount = 0;
    public int totalRifleAmmoAmount = 0;

    // ������� (Throwable):
    [Header("Throwables General")]  
    public float throwForce = 10f; // - ���� ������ Throwable
    public GameObject throwableSpawn;
    public float forceMultiplier = 0f; // - ��������� ���� ������; ����� ��� ����, ����� ����������� ���� ������ �� ���� ���������� � ������
    public float forceMultiplierLimit = 2f;

    [Header("Lethal Throwables")]
    public GameObject highExplosiveGrenadePrefab;

    public int lethalsCount = 0;
    public Throwable.ThrowableType equippedLethalThrowableType;
    public const int MAX_LETHAL_THROWABLES_COUNT = 3;

    [Header("Tactical Throwables")]
    public GameObject smokeGrenadePrefab;

    public int tacticalsCount = 0;
    public Throwable.ThrowableType equippedTacticalThrowableType;
    public const int MAX_TACTICAL_THROWABLES_COUNT = 3;

    private void Start()
    {
        activeWeaponSlot = weaponSlots[0];

        equippedLethalThrowableType = Throwable.ThrowableType.None;
        equippedTacticalThrowableType = Throwable.ThrowableType.None;
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

        if (Input.GetKeyDown(WeaponSlot1KeyCode))
        {
            SwitchActiveSlot(0);
        }
        else if(Input.GetKeyDown(WeaponSlot2KeyCode))
        {
            SwitchActiveSlot(1);
        }

        // ���������� � ������ ������� (Throwable):
        if (Input.GetKey(LethalThrowableThrowKeyCode) || Input.GetKey(TacticalThrowableThrowKeyCode))
        {
            forceMultiplier += Time.deltaTime;

            // ��������� �� ���������� �������� ������:
            if (forceMultiplier > forceMultiplierLimit)
            {
                forceMultiplier = forceMultiplierLimit;
            }
        }

        // ������ ������ ������� (Throwable):
        //! �������� ����: ����� Input.GetKeyUp() ���������, ������� �� ������������ �������
        if (Input.GetKeyUp(LethalThrowableThrowKeyCode))
        {
            if(lethalsCount > 0)
            {
                ThrowLethalThrowable();
            }

            forceMultiplier = 0;
        }

        // ������ ����������� ������� (Throwable):
        if (Input.GetKeyUp(TacticalThrowableThrowKeyCode))
        {
            if (tacticalsCount > 0)
            {
                ThrowTacticalThrowable();
            }

            forceMultiplier = 0;
        }
    }

    public void PickupWeapon(GameObject pickedUpWeapon)
    {
        AddWeaponIntoActiveSlot(pickedUpWeapon);
    }

    // "���������" ������ � ����:
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

    // ������ ������� �������� ������ � ����������� �������:
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

    // ������ ������� �������� ����:
    public void SwitchActiveSlot(int slotNumber)
    {
        // ���� � ������� ����� ���� ������, �� ������ ��� ����������:
        if(activeWeaponSlot.transform.childCount > 0)
        {
            Weapon currentWeapon = activeWeaponSlot.transform.GetChild(0).GetComponent<Weapon>();
            currentWeapon.isActive = false;
        }

        // ������ ������� ����:
        activeWeaponSlot = weaponSlots[slotNumber];

        // ���� � ����� ����� ���� ������, �� ������ ��� ��������:
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

    // ���������� ���������� ��������, ������� �������� ��� ������� ������:
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

    // ��������� ���������� ��������:
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
    // ��������� ������� (Throwable):
    public void PickupThrowable(Throwable throwable)
    {
        switch (throwable.throwableType)
        {
            case Throwable.ThrowableType.HighExplosiveGrenade:
                PickupThrowableAsLethal(throwable.throwableType);
                break;
            case Throwable.ThrowableType.SmokeGrenade:
                PickupThrowableAsTactical(throwable.throwableType);
                break;
        }
    }

    // ����� ��� ������� ������ ������� (Throwable):
    private void PickupThrowableAsLethal(Throwable.ThrowableType throwableType)
    {
        if(equippedLethalThrowableType == throwableType || equippedLethalThrowableType == Throwable.ThrowableType.None)
        {
            equippedLethalThrowableType = throwableType;
            
            // ���������, ������ ���� ���������� �� ������ ���������:
            if (lethalsCount < MAX_LETHAL_THROWABLES_COUNT)
            {
                lethalsCount += 1;
                // ���������� ������ ������� �� �����:
                Destroy(InteractionManager.Instance.throwableWeHoverOver.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("Lethal throwables limit count reached!");
            }
        }
        else
        {
            // TODO: �������� ����������� ������ ��� ������� (Throwable)
        }
    }

    // ����� ��� ������� ����������� ������� (Throwable):
    private void PickupThrowableAsTactical(Throwable.ThrowableType throwableType)
    {
        if (equippedTacticalThrowableType == throwableType || equippedTacticalThrowableType == Throwable.ThrowableType.None)
        {
            equippedTacticalThrowableType = throwableType;

            // ���������, ������ ���� ���������� �� ������ ���������:
            if (tacticalsCount < MAX_TACTICAL_THROWABLES_COUNT)
            {
                tacticalsCount += 1;
                // ���������� ������ ������� �� �����:
                Destroy(InteractionManager.Instance.throwableWeHoverOver.gameObject);
                HUDManager.Instance.UpdateThrowablesUI();
            }
            else
            {
                print("Tactical throwables limit count reached!");
            }
        }
        else
        {
            // TODO: �������� ����������� ������ ��� ������� (Throwable)
        }
    }

    // ������� ������� (Throwable):
    private void ThrowLethalThrowable()
    {
        GameObject throwablePrefab = GetThrowablePrefab(equippedLethalThrowableType);
        
        // ������� Throwable � �����:
        GameObject throwable = Instantiate(throwablePrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        // ��������� ���� � ���������� �������:
        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        lethalsCount -= 1;

        // ���� � ��� ����������� ������ �������, �� ������ ������� ��� ������� �� None:
        if (lethalsCount <= 0)
        {
            equippedLethalThrowableType = Throwable.ThrowableType.None;
        }

        // ��������� UI:
        HUDManager.Instance.UpdateThrowablesUI();
    }

    private void ThrowTacticalThrowable()
    {
        GameObject throwablePrefab = GetThrowablePrefab(equippedTacticalThrowableType);

        // ������� Throwable � �����:
        GameObject throwable = Instantiate(throwablePrefab, throwableSpawn.transform.position, Camera.main.transform.rotation);
        Rigidbody rb = throwable.GetComponent<Rigidbody>();

        // ��������� ���� � ���������� �������:
        rb.AddForce(Camera.main.transform.forward * (throwForce * forceMultiplier), ForceMode.Impulse);

        throwable.GetComponent<Throwable>().hasBeenThrown = true;

        tacticalsCount -= 1;

        // ���� � ��� ����������� ����������� �������, �� ������ ������� ��� ������� �� None:
        if (tacticalsCount <= 0)
        {
            equippedTacticalThrowableType = Throwable.ThrowableType.None;
        }

        // ��������� UI:
        HUDManager.Instance.UpdateThrowablesUI();
    }

    private GameObject GetThrowablePrefab(Throwable.ThrowableType throwableType)
    {
        switch (throwableType)
        {
            case Throwable.ThrowableType.HighExplosiveGrenade:
                return highExplosiveGrenadePrefab;
            case Throwable.ThrowableType.SmokeGrenade:
                return smokeGrenadePrefab;
        }

        return new();
    }

    #endregion
}

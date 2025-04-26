using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static Weapon;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance { set; get; }

    // ������ ���� ������ ��� ������:
    public List<GameObject> weaponSlots;

    // ������� ���� ��� ������:
    public GameObject activeWeaponSlot;

    [Header("Ammo")]
    public int totalPistolAmmoAmount = 0;
    public int totalRifleAmmoAmount = 0;

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
}

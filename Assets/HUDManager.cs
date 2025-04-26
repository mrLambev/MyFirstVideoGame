using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    public const string COLT1911_WEAPON_SPRITE_RESOURCE_NAME = "Colt1911_Weapon";
    public const string AK74_WEAPON_SPRITE_RESOURCE_NAME = "AK74_Weapon";
    public const string PISTOL_AMMO_SPRITE_RESOURCE_NAME = "Pistol_Ammo";
    public const string RIFLE_AMMO_SPRITE_RESOURCE_NAME = "Rifle_Ammo";

    public static HUDManager Instance { set; get; }

    [Header("Ammo")]
    public TextMeshProUGUI magazineAmmoUI;
    public TextMeshProUGUI totalAmmoUI;
    public Image ammoTypeUI;

    [Header("Weapon")]
    public Image activeWeaponUI;
    public Image unActiveWeaponUI;

    [Header("Shooting Mode")]
    public TextMeshProUGUI shootingModeDisplay;

    [Header("Throwables")]
    public Image lethalUI;
    public TextMeshProUGUI lethalAmountUI;

    public Image tacticalUI;
    public TextMeshProUGUI tacticalAmountUI;

    public Sprite emptySlot;

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
        Weapon activeWeapon = WeaponManager.Instance.activeWeaponSlot.GetComponentInChildren<Weapon>();
        Weapon unActiveWeapon = GetUnactiveWeaponSlot().GetComponentInChildren<Weapon>();

        if (activeWeapon)
        {
            magazineAmmoUI.text = $"{activeWeapon.bulletsLeft}";
            totalAmmoUI.text = $"{WeaponManager.Instance.CheckAmmoLeftFor(activeWeapon.thisWeaponModel)}";

            Weapon.WeaponModel model = activeWeapon.thisWeaponModel;
            ammoTypeUI.sprite = GetAmmoSprite(model);

            activeWeaponUI.sprite = GetWeaponSprite(model);

            switch (activeWeapon.shootingMode)
            {
                case Weapon.ShootingMode.Auto:
                    shootingModeDisplay.text = "Auto";
                    break;
                case Weapon.ShootingMode.Burst:
                    shootingModeDisplay.text = "Burst";
                    break;
                case Weapon.ShootingMode.Single:
                    shootingModeDisplay.text = "Single";
                    break;
                default:
                    shootingModeDisplay.text = "Unknown Shooting Mode";
                    break;
            }

            if (unActiveWeapon)
            {
                unActiveWeaponUI.sprite = GetWeaponSprite(unActiveWeapon.thisWeaponModel);
            }
        }
        else
        {
            magazineAmmoUI.text = "";
            totalAmmoUI.text = "";

            ammoTypeUI.sprite = emptySlot;

            shootingModeDisplay.text = "";

            activeWeaponUI.sprite = emptySlot;
            unActiveWeaponUI.sprite = emptySlot;
        }
    }

    private Sprite GetWeaponSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.Colt1911:
                return Instantiate(Resources.Load<GameObject>(COLT1911_WEAPON_SPRITE_RESOURCE_NAME)).GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.AK74:
                return Instantiate(Resources.Load<GameObject>(AK74_WEAPON_SPRITE_RESOURCE_NAME)).GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }

    private Sprite GetAmmoSprite(Weapon.WeaponModel model)
    {
        switch (model)
        {
            case Weapon.WeaponModel.Colt1911:
                return Instantiate(Resources.Load<GameObject>(PISTOL_AMMO_SPRITE_RESOURCE_NAME)).GetComponent<SpriteRenderer>().sprite;
            case Weapon.WeaponModel.AK74:
                return Instantiate(Resources.Load<GameObject>(RIFLE_AMMO_SPRITE_RESOURCE_NAME)).GetComponent<SpriteRenderer>().sprite;
            default:
                return null;
        }
    }

    private GameObject GetUnactiveWeaponSlot()
    {
        foreach (GameObject weaponSlot in WeaponManager.Instance.weaponSlots)
        {
            if (weaponSlot != WeaponManager.Instance.activeWeaponSlot)
            {
                return weaponSlot;
            }
        }

        // Это не должно произойти, но это нужно прописать, а то компилятор выдаст ошибку:
        return null;
    }
}

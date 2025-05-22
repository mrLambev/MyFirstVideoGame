using UnityEditor;
using UnityEngine;
using static Weapon;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { set; get; }

    public AudioSource BeerBottle_Break;

    public AudioSource Bullet_Hit;

    public AudioSource ShootingChannel;

    public AudioClip Colt1911_Shot;
    public AudioSource Colt1911_Reload;
    public AudioSource Colt1911_EmptyMagazine;

    public AudioClip AK74_Shot;
    public AudioSource AK74_Reload;
    public AudioSource AK74_EmptyMagazine;
    public AudioSource AK74_ChangeShootingMode;

    public AudioSource throwablesChannel;
    public AudioClip highExplosiveGrenadeSound;
    public AudioClip smokeGrenadeSound;

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

    public void PlayShootingSound(WeaponModel weaponModel)
    {
        switch(weaponModel) {
            case WeaponModel.Colt1911:
                ShootingChannel.PlayOneShot(Colt1911_Shot);
                break;
            case WeaponModel.AK74:
                ShootingChannel.PlayOneShot(AK74_Shot);
                break;
        }
    }

    public void PlayReloadSound(WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case WeaponModel.Colt1911:
                Colt1911_Reload.Play();
                break;
            case WeaponModel.AK74:
                AK74_Reload.Play();
                break;
        }
    }

    public void PlayEmptyMagazineSound(WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case WeaponModel.Colt1911:
                Colt1911_EmptyMagazine.Play();
                break;
            case WeaponModel.AK74:
                AK74_EmptyMagazine.Play();
                break;
        }
    }

    public void PlayChangeShootingMode(WeaponModel weaponModel)
    {
        switch (weaponModel)
        {
            case WeaponModel.Colt1911:
                print("Для данного оружия не предусмотрен звук смены режима стрельбы!");
                break;
            case WeaponModel.AK74:
                AK74_ChangeShootingMode.Play();
                break;
        }
    }
}

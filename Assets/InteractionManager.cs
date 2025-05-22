using System.Runtime.CompilerServices;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    // �����:
    public const KeyCode PICK_UP_KEYCODE = KeyCode.F;

    public static InteractionManager Instance { set; get; }

    public Weapon weaponWeHoverOver = null;

    public AmmoBox ammoBoxWeHoverOver = null;

    public Throwable throwableWeHoverOver = null;

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
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject gameObjectWeHit = hit.transform.gameObject;

            // ���� �� ������� �� ������ (Weapon):
            if (gameObjectWeHit.GetComponent<Weapon>())
            {
                weaponWeHoverOver = gameObjectWeHit.gameObject.GetComponent<Weapon>();
                weaponWeHoverOver.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(PICK_UP_KEYCODE))
                {
                    WeaponManager.Instance.PickupWeapon(gameObjectWeHit.gameObject);
                }
            }
            else 
            {
                if (weaponWeHoverOver)
                {
                    weaponWeHoverOver.GetComponent<Outline>().enabled = false;
                }
            }

            // ���� �� ������� �� ���� � ������ (AmmoBox):
            if (gameObjectWeHit.GetComponent<AmmoBox>())
            {
                ammoBoxWeHoverOver = gameObjectWeHit.gameObject.GetComponent<AmmoBox>();
                ammoBoxWeHoverOver.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(PICK_UP_KEYCODE))
                {
                    WeaponManager.Instance.PickupAmmo(ammoBoxWeHoverOver);
                    // ���������� ���� � ������� �� �����:
                    Destroy(gameObjectWeHit.gameObject);
                }
            }
            else
            {
                if (ammoBoxWeHoverOver)
                {
                    ammoBoxWeHoverOver.GetComponent<Outline>().enabled = false;
                }
            }

            // ���� �� ������� �� ������� (Throwable):
            if (gameObjectWeHit.GetComponent<Throwable>())
            {
                throwableWeHoverOver = gameObjectWeHit.gameObject.GetComponent<Throwable>();
                throwableWeHoverOver.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(PICK_UP_KEYCODE))
                {
                    WeaponManager.Instance.PickupThrowable(throwableWeHoverOver);
                }
            }
            else
            {
                if (throwableWeHoverOver)
                {
                    throwableWeHoverOver.GetComponent<Outline>().enabled = false;
                }
            }
        }
    }
}

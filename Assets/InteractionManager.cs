using System.Runtime.CompilerServices;
using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    // Бинды:
    public const KeyCode PICK_UP_KEYCODE = KeyCode.F;

    public static InteractionManager Instance { set; get; }

    public Weapon weaponWeHoverOver = null;

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
        }
    }
}

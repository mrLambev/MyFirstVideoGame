using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    public int amount = 200;

    public enum AmmoType
    {
        PistolAmmo,
        RifleAmmo
    }
    public AmmoType ammoType;
}

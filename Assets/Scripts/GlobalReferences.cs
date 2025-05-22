using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    public static GlobalReferences Instance { set; get; }

    public GameObject bulletImpactEffectPrefab;

    public GameObject highExplosionGrenadeEffect;

    public GameObject smokeGrenadeEffect;

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
}

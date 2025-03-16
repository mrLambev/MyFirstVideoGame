using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { set; get; }

    public AudioSource shootingSound_AK47;

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

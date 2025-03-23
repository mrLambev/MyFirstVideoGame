using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { set; get; }

    public AudioSource BeerBottle_Break;

    public AudioSource Bullet_Hit;

    public AudioSource AK47_Shot;
    public AudioSource AK47_Reload;
    public AudioSource AK47_EmptyMagazine;

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

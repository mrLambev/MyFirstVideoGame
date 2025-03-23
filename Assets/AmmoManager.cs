using TMPro;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    public static AmmoManager Instance { set; get; }

    public TextMeshProUGUI ammoCountDisplay;

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

using TMPro;
using UnityEngine;

public class HUDManager : MonoBehaviour {
    public static HUDManager Instance { set; get; }

    public TextMeshProUGUI shootingModeDisplay;
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

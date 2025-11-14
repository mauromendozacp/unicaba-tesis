using UnityEngine;

public class KeysManager : MonoBehaviour
{
    public static KeysManager Instance { get; private set; }

    [Header("UI")]
    [SerializeField] private GameplayUI gameplayUI = null;

    [SerializeField] private int initialKeys = 0;

    private int keys;

    public int Keys => keys;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        keys = initialKeys;
        UpdateUI();
    }

    public void AddKey()
    {
        keys++;
        UpdateUI();
        Debug.Log("[KeysManager] Llave recogida. Total: " + keys);
    }

    public bool TryUseKey()
    {
        if (keys <= 0)
            return false;

        keys--;
        UpdateUI();
        Debug.Log("[KeysManager] Llave usada. Restantes: " + keys);
        return true;
    }

    private void UpdateUI()
    {
        if (gameplayUI != null)
        {
            gameplayUI.UpdateKeysText(keys);
        }
    }
}

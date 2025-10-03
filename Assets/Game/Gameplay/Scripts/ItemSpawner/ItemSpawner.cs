using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour, ISpawner
{
    [Header("Prefab raíz con ItemWorld + Collider (IsTrigger)")]
    [SerializeField] private ItemWorld itemWorldPrefab;

    [Header("Configuración de ítems a spawnear")]
    [SerializeField] private List<ItemSpawnSettings> itemSpawnSettings = new List<ItemSpawnSettings>();

    public List<ItemSpawnSettings> GetSpawnSettings() => itemSpawnSettings;

    private void Start()
    {
        foreach (var settings in itemSpawnSettings)
        {
            if (settings.ItemData == null)
            {
                Debug.LogWarning($"[ItemSpawner] ItemData no asignado para '{settings.Name}'.");
                continue;
            }

            foreach (var pos in settings.SpawnPositions)
            {
                Spawn(settings.ItemData, pos);
            }
        }
    }

    private void Spawn(ItemData data, Vector3 position)
    {
        if (itemWorldPrefab == null)
        {
            Debug.LogWarning("[ItemSpawner] ItemWorldPrefab no asignado.");
            return;
        }

        ItemWorld newItem = Instantiate(itemWorldPrefab, position, Quaternion.identity);
        newItem.SetData(data);
    }
}

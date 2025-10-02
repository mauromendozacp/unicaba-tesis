using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerItemCollector : MonoBehaviour
{
    private PlayerInventory inventory;

    private void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
        if (inventory == null)
        {
            inventory = GetComponentInChildren<PlayerInventory>();
        }

        if (inventory == null)
        {
            Debug.LogError("[PlayerItemCollector] No se encontró PlayerInventory en el jugador o hijos.");
        }
        else
        {
            Debug.Log("[PlayerItemCollector] Inventory encontrado correctamente.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[PlayerItemCollector] Entró en trigger con: {other.name}");

        ItemWorld itemWorld = other.GetComponent<ItemWorld>();
        if (itemWorld == null)
        {
            Debug.Log($"[PlayerItemCollector] {other.name} no tiene ItemWorld.");
            return;
        }

        Debug.Log($"[PlayerItemCollector] ItemWorld detectado: {itemWorld.name}");

        ItemData data = itemWorld.GetItem();
        if (data == null)
        {
            Debug.LogWarning("[PlayerItemCollector] El ItemWorld no tiene ItemData asignado.");
            return;
        }

        Debug.Log($"[PlayerItemCollector] ItemData encontrado: {data.ItemName}");

        if (data is Key keyItem)
        {
            inventory.AddKey(keyItem.DoorId);
            Debug.Log($"[PlayerItemCollector] Llave agregada al inventario: {keyItem.DoorId}");
        }
        else
        {
            inventory.EquipItem(data);
            Debug.Log($"[PlayerItemCollector] Item equipado: {data.ItemName}");
        }

        itemWorld.Release();
        Debug.Log($"[PlayerItemCollector] Item {data.ItemName} liberado (oculto del mundo).");
    }
}

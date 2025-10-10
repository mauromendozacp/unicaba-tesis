using UnityEngine;

[CreateAssetMenu(fileName = "Item_", menuName = "Items/Key")]
public class Key : ItemData
{
    [SerializeField] private string doorId = "";

    public string DoorId => doorId;

    public override void Use(GameObject user)
    {
        PlayerInventory inventory = user.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddKey(doorId);
            Debug.Log($"Llave {doorId} añadida al inventario.");
        }
        else
        {
            Debug.LogWarning("El usuario no tiene PlayerInventory para usar la llave.");
        }
    }
}

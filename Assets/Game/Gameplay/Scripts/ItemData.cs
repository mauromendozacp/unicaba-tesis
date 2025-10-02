using UnityEngine;

public enum ItemType
{
    None,
    Consumable,
    Gun,
    Collectable
}

[CreateAssetMenu(menuName = "Items/ItemBase")]
public abstract class ItemData : ScriptableObject
{
    [SerializeField] private string itemName = string.Empty;
    [SerializeField] private Sprite icon = null;
    [SerializeField] private GameObject prefab = null;
    [SerializeField] private Vector3 positionOffset = Vector3.zero;
    [SerializeField] private Vector3 rotation = Vector3.zero;
    [SerializeField] private ItemType type = ItemType.None;

    public string ItemName => itemName;
    public Sprite Icon => icon;
    public GameObject Prefab => prefab;
    public Vector3 PositionOffset => positionOffset;
    public Vector3 Rotation => rotation;
    public ItemType Type => type;

    public abstract void Use(GameObject user);
}
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemBase")]
public abstract class ItemData : ScriptableObject
{
    [SerializeField] private string itemName = string.Empty;
    [SerializeField] private Sprite icon = null;
    [SerializeField] private int amount = 0;

    public string ItemName => itemName;
    public Sprite Icon => icon;
    public int Amount => amount;

    public abstract void Use(GameObject user);
}
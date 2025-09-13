using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemBase")]
public abstract class ItemData : ScriptableObject
{
    [SerializeField] private string itemName = string.Empty;
    [SerializeField] private Sprite icon = null;
    [SerializeField] private Mesh mesh = null;
    [SerializeField] private Material material = null;
    [SerializeField] private int amount = 0;

    public string ItemName => itemName;
    public Sprite Icon => icon;
    public Mesh Mesh => mesh;
    public Material Material => material;
    public int Amount => amount;

    public abstract void Use(GameObject user);
}
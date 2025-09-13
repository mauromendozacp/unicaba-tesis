using System;
using UnityEngine;

public class ItemWorld : MonoBehaviour, IEquipable
{
    [SerializeField] private ItemData data = null;
    [SerializeField] private MeshRenderer meshRenderer = null;
    [SerializeField] private MeshFilter meshFilter = null;

    public ItemData Data => data;
    public Action<ItemWorld> onRelease = null;

    public void SetData(ItemData data)
    {
        this.data = data;

        meshRenderer.material = data.Material;
        meshFilter.mesh = data.Mesh;
    }

    public void Get()
    {
        gameObject.SetActive(true);
    }

    public void Release()
    {
        gameObject.SetActive(false);
    }

    public ItemData GetItem() => data;

    public virtual void Equip()
    {
        onRelease?.Invoke(this);
    }
}

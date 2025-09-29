using System;
using UnityEngine;

public class ItemWorld : MonoBehaviour, IEquipable
{
    [SerializeField] private ItemData data = null;

    public ItemData Data => data;
    public Action<ItemWorld> onRelease = null;

    private void Start()
    {
        if (data != null)
        {
            SetData(data);
        }
    }

    public void SetData(ItemData data)
    {
        this.data = data;

        GameObject item = Instantiate(data.Prefab, transform);
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

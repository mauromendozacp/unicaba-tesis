using UnityEngine;
using System;

public class ItemWorld : MonoBehaviour, IEquipable
{
    [SerializeField] private ItemData data = null;
    private GameObject visualInstance = null;

    public ItemData Data => data;
    public Action<ItemWorld> onRelease = null;

    private void Start()
    {
        if (data != null)
        {
            SetData(data);
        }
    }

    public void SetData(ItemData newData)
    {
        data = newData;

        // Limpia visual anterior
        if (visualInstance != null)
        {
            Destroy(visualInstance);
        }

        // Instancia el prefab visual del ItemData
        if (data.Prefab != null)
        {
            visualInstance = Instantiate(data.Prefab, transform);
            visualInstance.transform.localPosition = data.PositionOffset;
            visualInstance.transform.localEulerAngles = data.Rotation;
        }
        else
        {
            Debug.LogWarning($"[ItemWorld] ItemData {data.name} no tiene prefab asignado.");
        }
    }

    public void Get()
    {
        gameObject.SetActive(true);
    }

    public void Release()
    {
        gameObject.SetActive(false);
    }

    public ItemData GetItem()
    {
        return data;
    }

    public void Equip()
    {
        onRelease?.Invoke(this);
    }
}

using System.Collections.Generic;
using UnityEngine;

public class PlayerItemDetection : MonoBehaviour
{
    [SerializeField] private LayerMask itemMask = default;

    private List<IEquipable> detectedItems = null;

    private void Start()
    {
        detectedItems = new List<IEquipable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.CheckLayerInMask(itemMask, other.gameObject.layer))
        {
            IEquipable itemEquipable = other.GetComponent<IEquipable>();

            if (itemEquipable != null && itemEquipable.GetItemType() != ItemType.Collectable)
            {
                detectedItems.Add(itemEquipable);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Utils.CheckLayerInMask(itemMask, other.gameObject.layer))
        {
            IEquipable itemEquipable = other.GetComponent<IEquipable>();

            if (itemEquipable != null && itemEquipable.GetItemType() != ItemType.Collectable)
            {
                detectedItems.Remove(itemEquipable);
            }
        }
    }

    public IEquipable GetFirstItemDetection()
    {
        if (detectedItems.Count > 0)
        {
            IEquipable item = detectedItems[0];
            detectedItems.RemoveAt(0);
            return item;
        }

        return null;
    }
}

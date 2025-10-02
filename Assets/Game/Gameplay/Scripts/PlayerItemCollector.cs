using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerItemCollector : MonoBehaviour
{
    [SerializeField] private LayerMask itemMask = default;

    private Action onCollectKey = null;

    public void Init(Action onCollectKey)
    {
        this.onCollectKey = onCollectKey;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Utils.CheckLayerInMask(itemMask, other.gameObject.layer))
        {
            IEquipable itemEquipable = other.GetComponent<IEquipable>();

            if (itemEquipable != null && itemEquipable.GetItemType() == ItemType.Collectable)
            {
                onCollectKey?.Invoke();
                Destroy(other.gameObject);
            }
        }
    }
}

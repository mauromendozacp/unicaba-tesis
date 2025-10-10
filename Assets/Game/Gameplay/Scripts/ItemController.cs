using UnityEngine;
using UnityEngine.Pool;

public class ItemController : MonoBehaviour
{
  [SerializeField] private ItemData[] availableItems = null;
  [SerializeField] private ItemWorld prefab = null;

  private ObjectPool<ItemWorld> pool = null;

  private void Awake()
  {
    pool = new ObjectPool<ItemWorld>(OnCreateItem, OnGetItem, OnReleaseItem, OnDestroyItem);
  }

  private void Start()
  {
    ItemWorld[] items = FindObjectsByType<ItemWorld>(FindObjectsSortMode.None);

    for (int i = 0; i < items.Length; i++)
    {
      items[i].onRelease += ReleaseItem;
    }
  }

  public ItemWorld SpawnItem(ItemData data, Vector3 position)
  {
    ItemWorld item = pool.Get();
    item.SetData(data);
    item.transform.position = position + data.PositionOffset;
    item.transform.eulerAngles = data.Rotation;
    //item.onRelease += ReleaseItem;
    return item;
  }

  public ItemWorld SpawnRandomItem(Vector3 position)
  {
    int randomIndex = Random.Range(0, availableItems.Length);
    ItemData randomItem = availableItems[randomIndex];

    return SpawnItem(randomItem, position);
  }

  public void ReleaseItem(ItemWorld item)
  {
    pool.Release(item);
  }

  private ItemWorld OnCreateItem()
  {
    ItemWorld itemWorld = Instantiate(prefab);
    itemWorld.onRelease += ReleaseItem;
    return itemWorld;
  }

  private void OnGetItem(ItemWorld item)
  {
    item.Get();
  }

  private void OnReleaseItem(ItemWorld item)
  {
    item.Release();
  }

  private void OnDestroyItem(ItemWorld item)
  {
    if (item != null)
    {
      Destroy(item.gameObject);
    }
  }
}

using UnityEngine;
using UnityEngine.Pool;

public class ExplosionPoolManager : MonoBehaviour
{
  public static ExplosionPoolManager Instance { get; private set; }

  [Header("Pool Settings")]
  [SerializeField] private GameObject explosionPrefab;
  [SerializeField] private int defaultCapacity = 10;
  [SerializeField] private int maxSize = 20;

  private IObjectPool<GameObject> explosionPool;
  private Transform poolParent;

  private void Awake()
  {
    if (Instance != null && Instance != this)
    {
      Destroy(gameObject);
    }
    else
    {
      Instance = this;
      poolParent = transform;
      SetupPool();
    }
  }

  private void SetupPool()
  {
    explosionPool = new ObjectPool<GameObject>(
        CreatePooledItem,
        OnTakeFromPool,
        OnReturnedToPool,
        OnDestroyPoolObject,
        true,
        defaultCapacity,
        maxSize
    );
  }

  private GameObject CreatePooledItem()
  {
    GameObject explosionInstance = Instantiate(explosionPrefab, poolParent);

    PoolableItem poolable = explosionInstance.GetComponent<PoolableItem>();
    if (poolable == null)
    {
      poolable = explosionInstance.AddComponent<PoolableItem>();
    }
    poolable.SetPool(explosionPool);

    if (explosionInstance.GetComponent<ExplosionLifetime>() == null)
    {
      Debug.LogWarning("El prefab de explosión no tiene el script ExplosionLifetime");
    }

    explosionInstance.SetActive(false); // Empezar desactivado
    return explosionInstance;
  }

  private void OnReturnedToPool(GameObject explosion)
  {
    explosion.SetActive(false);
    explosion.transform.position = Vector3.zero;
    explosion.transform.rotation = Quaternion.identity;
  }

  private void OnTakeFromPool(GameObject explosion)
  {
    explosion.SetActive(true);
  }

  private void OnDestroyPoolObject(GameObject explosion)
  {
#if UNITY_EDITOR
    DestroyImmediate(explosion);
#else
            Destroy(explosion);
#endif
  }

  public GameObject GetExplosion()
  {
    return explosionPool.Get();
  }
}
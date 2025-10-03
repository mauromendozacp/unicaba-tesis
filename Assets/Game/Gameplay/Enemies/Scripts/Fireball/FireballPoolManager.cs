using UnityEngine;
using UnityEngine.Pool;

public class FireballPoolManager : MonoBehaviour
{
  public static FireballPoolManager Instance { get; private set; }

  [Header("Pool Settings")]
  [SerializeField] private GameObject fireballPrefab;
  [SerializeField] private int defaultCapacity = 10;
  [SerializeField] private int maxSize = 20;

  private IObjectPool<GameObject> fireballPool;
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
    fireballPool = new ObjectPool<GameObject>(
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
    GameObject fireballInstance = Instantiate(fireballPrefab, poolParent);

    // Asegúrate de que el script Fireball sepa a qué pool debe devolverse.
    // Se añade un componente PoolableItem para manejar el 'Release'.
    PoolableItem poolable = fireballInstance.GetComponent<PoolableItem>();
    if (poolable == null)
    {
      poolable = fireballInstance.AddComponent<PoolableItem>();
    }
    poolable.SetPool(fireballPool);

    return fireballInstance;
  }

  private void OnReturnedToPool(GameObject fireball)
  {
    fireball.SetActive(false);
    // Restablece cualquier estado que necesite ser limpiado
    // Por ejemplo, si el Fireball tuviera un Rigidbody, podrías hacer:
    // fireball.GetComponent<Rigidbody>().velocity = Vector3.zero;
    // fireball.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
  }

  private void OnTakeFromPool(GameObject fireball)
  {
    fireball.SetActive(true);
    // El script Fireball deberá encargarse de resetear su posición, rotación, etc.,
    // justo antes de ser disparado.
  }

  private void OnDestroyPoolObject(GameObject fireball)
  {
#if UNITY_EDITOR
    DestroyImmediate(fireball);
#else
    Destroy(fireball);
#endif
  }

  // --- Métodos Públicos para usar la Pool ---

  public GameObject GetFireball()
  {
    return fireballPool.Get();
  }

  // Este método lo usará el script Fireball cuando deba "destruirse"
  public void ReleaseFireball(GameObject fireball)
  {
    fireballPool.Release(fireball);
  }
}
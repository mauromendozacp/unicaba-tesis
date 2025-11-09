using UnityEngine;
using UnityEngine.Pool;

public class PoolableItem : MonoBehaviour
{
  private IObjectPool<GameObject> parentPool;

  public void SetPool(IObjectPool<GameObject> pool)
  {
    parentPool = pool;
  }

  public void ReturnToPool()
  {
    if (parentPool != null)
    {
      parentPool.Release(gameObject);
    }
    else
    {
      Destroy(gameObject);
    }
  }
}
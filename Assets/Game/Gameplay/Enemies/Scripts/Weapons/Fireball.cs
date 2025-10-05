using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour
{
  [SerializeField] private float damage = 20f;
  [SerializeField] private float lifetime = 5f;

  private PoolableItem poolableItem;

  void Awake()
  {
    poolableItem = GetComponent<PoolableItem>();
    if (poolableItem == null)
    {
      Debug.LogError("Fireball is missing PoolableItem component. Add it or ensure it's added by the pool manager.");
    }
  }

  void OnEnable()
  {
    StartCoroutine(LifetimeCoroutine());
  }

  IEnumerator LifetimeCoroutine()
  {
    yield return new WaitForSeconds(lifetime);
    ReturnToPool();
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      IDamageable player = other.GetComponent<IDamageable>();
      if (player != null && player.IsAlive)
      {
        player.TakeDamage(damage);
      }
      ReturnToPool();
    }
  }

  private void ReturnToPool()
  {
    StopAllCoroutines();
    poolableItem?.ReturnToPool();
  }
}
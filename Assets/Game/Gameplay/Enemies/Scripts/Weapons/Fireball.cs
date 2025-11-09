using UnityEngine;
using System.Collections;

public class Fireball : MonoBehaviour
{
  [SerializeField] private float damage = 20f;
  [SerializeField] private float lifetime = 5f;
  //[SerializeField] GameObject ExplosionPrefab;

  private PoolableItem poolableItem;
  Rigidbody rb;

  private bool isAlive;

  void Awake()
  {
    poolableItem = GetComponent<PoolableItem>();
    rb = GetComponent<Rigidbody>();

    if (poolableItem == null)
    {
      Debug.LogError("Fireball is missing PoolableItem component. Add it or ensure it's added by the pool manager.");
    }
    if (rb == null)
    {
      Debug.LogError("Fireball is missing Rigidbody component. A Rigidbody is required for movement.");
    }
  }

  void OnEnable()
  {
    isAlive = true;
    StartCoroutine(LifetimeCoroutine());
  }

  public void SetDamage(float newDamage)
  {
    damage = newDamage;
  }

  public void Launch(Vector3 startPosition, Vector3 direction, float speed)
  {
    if (rb == null)
    {
      Debug.LogError("No se puede lanzar una bola de fuego sin Rigidbody.");
      ReturnToPool();
      return;
    }

    transform.position = startPosition;
    transform.rotation = Quaternion.LookRotation(direction);
    rb.useGravity = false;
    rb.isKinematic = false;
    rb.linearVelocity = direction * speed;
  }

  IEnumerator LifetimeCoroutine()
  {
    yield return new WaitForSeconds(lifetime);
    ReturnToPool();
  }

  /*
    void OnTriggerEnter(Collider other)
    {
      if (!isAlive) return;
      Debug.Log($"Collided with: {other.name}");
      if (other.CompareTag("Player"))
      {
        IDamageable player = other.GetComponent<IDamageable>();
        Debug.Log(player);
        if (player == null) player = other.GetComponentInParent<IDamageable>();
        Debug.Log(player);
        if (player != null && player.IsAlive)
        {
          player.TakeDamage(damage);
        }
      }
      Vector3 explosionPosition = other.ClosestPoint(transform.position);
      ReturnToPool();
      if (ExplosionPrefab != null)
      {
        GameObject attack = Instantiate(ExplosionPrefab, explosionPosition, Quaternion.identity);
        GameObject.Destroy(attack, 2f);
      }
    }
  */
  void OnCollisionEnter(Collision collision)
  {
    if (!isAlive) return;

    ContactPoint contact = collision.contacts[0];
    Vector3 explosionPosition = contact.point;
    Vector3 normal = contact.normal;

    if (collision.gameObject.CompareTag("Player"))
    {
      IDamageable player = collision.gameObject.GetComponent<IDamageable>();
      if (player != null && player.IsAlive)
      {
        player.TakeDamage(damage);
      }
    }

    ReturnToPool();

    /*
    if (ExplosionPrefab != null)
    {
      Quaternion explosionRotation = Quaternion.LookRotation(normal);
      //GameObject attack = Instantiate(ExplosionPrefab, explosionPosition, explosionRotation);
      GameObject attack = ExplosionPoolManager.Instance.GetExplosion();
      attack.transform.position = explosionPosition;
      attack.transform.rotation = explosionRotation;
      //GameObject.Destroy(attack, 2f);
    }
    */
    Quaternion explosionRotation = Quaternion.LookRotation(normal);
    GameObject attack = ExplosionPoolManager.Instance.GetExplosion();
    attack.transform.position = explosionPosition;
    attack.transform.rotation = explosionRotation;
  }

  private void ReturnToPool()
  {
    if (!isAlive) return;
    isAlive = false;
    StopAllCoroutines();

    if (rb != null)
    {
      rb.linearVelocity = Vector3.zero;
      rb.angularVelocity = Vector3.zero;
    }

    poolableItem?.ReturnToPool();
  }
}
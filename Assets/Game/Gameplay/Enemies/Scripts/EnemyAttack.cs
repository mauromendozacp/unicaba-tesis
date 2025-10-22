using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
  float _damage = 0f;
  [SerializeField] Collider _collider;

  void Awake()
  {
    if (_collider == null) _collider = GetComponent<Collider>();
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player") && _damage > 0)
    {
      IDamageable player = other.GetComponent<IDamageable>();
      if (player != null && player.IsAlive)
      {
        player.TakeDamage(_damage);
      }
    }
  }

  public void SetDamage(float damage)
  {
    if (damage > 0)
    {
      _damage = damage;
    }
  }

  public void ToggleCollider(bool enable)
  {
    if (_collider != null) _collider.enabled = enable;
  }
}

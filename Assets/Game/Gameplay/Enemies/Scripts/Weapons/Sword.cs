using UnityEngine;

public class Sword : MonoBehaviour
{
  [SerializeField] private float damage = 30f;
  Collider attackCollider;
  void Awake()
  {
    attackCollider = GetComponent<Collider>();
  }

  public void SetAttackColliderState(bool state)
  {
    attackCollider.enabled = state;
  }

  void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      var playerHealth = other.GetComponent<PlayerHealth>();
      if (playerHealth != null && playerHealth.IsAlive)
      {
        playerHealth.TakeDamage(damage);
      }
    }
  }
}

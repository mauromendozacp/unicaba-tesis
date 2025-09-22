using System;
using UnityEngine;


public class EnemyPrototype : EnemyBase
{
  [SerializeField] GameObject horns;
  [SerializeField] Material idleMaterial;
  [SerializeField] Material chaseMaterial;
  [SerializeField] Material attackMaterial;

  [SerializeField] Material damagedMaterial;
  Material originalMaterial;

  [Header("Attack Settings")]
  [SerializeField] private Collider attackCollider;
  [SerializeField] private float attackDamage = 40f;

  void OnEnable()
  {
    currentHealth = maxHealth;
    ChangeState(new IdleState(this));
    GetComponent<Collider>().enabled = true;
  }

  protected override void Awake()
  {
    base.Awake();
    originalMaterial = GetComponentInChildren<Renderer>().material;
    ChangeState(new IdleState(this));
  }


  void Start()
  {
    if (attackCollider != null)
    {
      attackCollider.enabled = false;
    }
  }

  void OnTriggerEnter(Collider other)
  {
    if (!other.CompareTag("Player")) return;
    IDamageable player = other.GetComponent<IDamageable>();
    if (player != null && !player.IsAlive)
    {
      CurrentTarget = null;
      ChangeState(new IdleState(this));
    }
    if (attackCollider != null && attackCollider.enabled && player != null)
    {
      player.TakeDamage(attackDamage);
    }
  }

  public override void TakeDamage(float damage)
  {
    base.TakeDamage(damage);

    if (currentHealth <= 0)
    {
      ChangeState(new DeathState(this));
    }
    else
    {
      ChangeState(new DamagedState(this));
    }
  }

  public void SetAttackCollider(bool active)
  {
    if (attackCollider != null)
    {
      attackCollider.enabled = active;
    }
  }

  public void StopAllCoroutines()
  {
    StopAllCoroutines();
  }

  public void ChangeMaterial()
  {
    foreach (Transform childTransform in horns.transform)
    {
      GameObject childObject = childTransform.gameObject;
      var renderer = childObject.GetComponent<Renderer>();
      if (renderer != null)
      {
        switch (currentState.State)
        {
          case EnemyState.Idle:
            renderer.material = idleMaterial;
            break;
          case EnemyState.Chase:
            renderer.material = chaseMaterial;
            break;
          case EnemyState.Attack:
            renderer.material = attackMaterial;
            break;
          default:
            renderer.material = idleMaterial;
            break;
        }
      }
    }
    if (currentState.State == EnemyState.Damaged)
    {
      GetComponent<Renderer>().material = damagedMaterial;
    }
    else
    {
      GetComponent<Renderer>().material = originalMaterial;
    }
  }

  public void Die()
  {
    base.Die();
  }
}
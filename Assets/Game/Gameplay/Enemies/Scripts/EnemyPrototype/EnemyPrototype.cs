using System;
using UnityEngine;


public class EnemyPrototype : EnemySoldier
{
  [SerializeField] GameObject horns;
  [SerializeField] Material idleMaterial;
  [SerializeField] Material chaseMaterial;
  [SerializeField] Material attackMaterial;

  //[SerializeField] Material damagedMaterial;
  //Material originalMaterial;


  void OnEnable()
  {
    currentHealth = maxHealth;
    ChangeState(new IdleState(this));
    //GetComponent<Collider>().enabled = true;
    EnableMovementAndCollisions();
  }

  protected override void Awake()
  {
    base.Awake();
    //originalMaterial = GetComponentInChildren<Renderer>().material;
  }


  protected override void Start()
  {
    base.Start();
    //ChangeState(new IdleState(this));
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
    if (!IsAlive) return;
    base.TakeDamage(damage);
    ChangeState(new DamagedState(this));
  }


  /*public void StopAllCoroutines()
  {
    StopAllCoroutines();
  }*/

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

  public override void Die()
  {
    base.Die();
  }

  public override void Kill()
  {
    ChangeState(new DeathState(this));
  }
}
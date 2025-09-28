using System.Collections;
using UnityEngine;

public class EnemyPrototype2 : EnemyBase
{
  [SerializeField] GameObject horns;
  [SerializeField] Material idleMaterial;
  [SerializeField] Material chaseMaterial;
  [SerializeField] Material attackMaterial;

  [SerializeField] Material damagedMaterial;
  Material originalMaterial;

  [Header("Long Range Attack Settings")]
  [SerializeField] GameObject fireBallPrefab;
  [SerializeField] Transform firePoint;
  [SerializeField] float fireBallSpeed = 10f;
  [SerializeField] float attackDistance = 8f; // Distancia a la que se posiciona para atacar
  public float AttackDistance => attackDistance;

  void OnEnable()
  {
    currentHealth = maxHealth;
    ChangeState(new Prototype2IdleState(this));
    GetComponent<Collider>().enabled = true;
  }

  protected override void Awake()
  {
    base.Awake();
    originalMaterial = GetComponentInChildren<Renderer>().material;
    ChangeState(new Prototype2IdleState(this));
  }

  public void ShootFireBall()
  {
    if (fireBallPrefab == null || firePoint == null)
    {
      Debug.LogError("FireBallPrefab or FirePoint not assigned in EnemyPrototype2.");
      return;
    }

    GameObject fireBall = Instantiate(fireBallPrefab, firePoint.position, firePoint.rotation);
    if (CurrentTarget != null)
    {
      Vector3 direction = (CurrentTarget.position - firePoint.position).normalized;
      fireBall.GetComponent<Rigidbody>().linearVelocity = direction * fireBallSpeed;
    }
  }

  public override void TakeDamage(float damage)
  {
    base.TakeDamage(damage);

    if (currentHealth <= 0)
    {
      ChangeState(new Prototype2DeathState(this));
    }
    else
    {
      ChangeState(new Prototype2DamagedState(this));
    }
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

  public override void Die()
  {
    base.Die();
  }
}
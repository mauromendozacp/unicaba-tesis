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
    //GetComponent<Collider>().enabled = true;
    EnableMovementAndCollisions();
  }

  protected override void Awake()
  {
    base.Awake();
    originalMaterial = GetComponentInChildren<Renderer>().material;
    ChangeState(new Prototype2IdleState(this));
  }

  public void ShootFireBall()
  {
    if (FireballPoolManager.Instance == null || firePoint == null)
    {
      Debug.LogError("FireballPoolManager.Instance or FirePoint not assigned/available.");
      return;
    }

    GameObject fireBall = FireballPoolManager.Instance.GetFireball();
    fireBall.transform.position = firePoint.position;
    fireBall.transform.rotation = firePoint.rotation;

    Rigidbody rb = fireBall.GetComponent<Rigidbody>();
    if (rb == null)
    {
      Debug.LogError("Fireball prefab is missing a Rigidbody component.");
      FireballPoolManager.Instance.ReleaseFireball(fireBall);
      return;
    }

    if (CurrentTarget != null)
    {
      Vector3 direction = (CurrentTarget.position - firePoint.position).normalized;
      rb.linearVelocity = direction * fireBallSpeed;
    }
    else
    {
      rb.linearVelocity = firePoint.forward * fireBallSpeed;
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

  public override void Kill()
  {
    ChangeState(new Prototype2DeathState(this));
  }
}
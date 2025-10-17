using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.AI;

public enum EnemyState
{
  Idle,
  Chase,
  Attack,
  Retreat,
  Damaged,
  Death
}

public abstract class EnemySoldier : EnemyBase
{
  [Header("Movement Settings")]
  [SerializeField] protected bool isTerritorial = false;
  public bool IsTerritorial => isTerritorial;
  [SerializeField] float moveSpeed = 4f;
  protected Vector3 initialPosition;
  public float currentSpeed;
  public float CurrentSpeed => currentSpeed;
  protected IEnemyState currentState;

  protected override void Start()
  {
    base.Start();
    currentSpeed = moveSpeed * UnityEngine.Random.Range(0.75f, 1.0f);
    initialPosition = transform.position;
    if (agent != null)
    {
      agent.speed = currentSpeed;
      agent.stoppingDistance = 0.1f;
      //agent.stoppingDistance = attackRange * 0.8f;
      agent.updateRotation = true; // El agente maneja la rotación
    }
  }

  protected virtual void Update()
  {
    currentState?.Update();
  }

  public void ChangeState(IEnemyState newState)
  {

    currentState?.Exit();
    StopAllCoroutines();
    currentState = newState;
    currentState.Enter();
  }



  public Vector3 GetInitialPosition()
  {
    return initialPosition;
  }

  public bool IsTooFarFromOrigin(float margin = 0f)
  {
    if (!isTerritorial || CurrentTarget == null) return false;

    float effectiveMaxDistance = maxChaseDistance + margin;
    if (effectiveMaxDistance < 0) effectiveMaxDistance = 0;

    float distanceSqr = (CurrentTarget.position - initialPosition).sqrMagnitude;
    float maxDistanceSqr = effectiveMaxDistance * effectiveMaxDistance;

    return distanceSqr > maxDistanceSqr;
  }

  protected virtual void OnDrawGizmosSelected()
  {
    // Visualización del Radio de Persecución (ChaseRadius)
    Gizmos.color = Color.yellow;
    Gizmos.DrawWireSphere(transform.position, chaseRadius);

    // Visualización del Límite Territorial (MaxChaseDistance)
    if (isTerritorial)
    {
      Gizmos.color = Color.red;
      Gizmos.DrawWireSphere(initialPosition, maxChaseDistance);
    }
  }
}
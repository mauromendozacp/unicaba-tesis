// PlayerHealth.cs
using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable, IRevivable
{
  [SerializeField] private float maxHealth = 100f;
  [SerializeField] private float invincibilityTime = 0.5f;
  //[SerializeField] private Material damagedMaterial;

  private float currentHealth;
  public float Health => currentHealth;

  private bool isInvincible = false;
  //private Material originalMaterial;
  //private Renderer playerRenderer;
  private PlayerInputController inputController;

  public bool IsAlive => currentHealth > 0;
  public bool IsDowned { get; private set; }
  public bool IsRevivable => IsDowned;

  public event Action<float, IDamageable> OnDamaged;
  public event Action<float, float> OnUpdateLife;
  public event Action<IDamageable> OnDeath;
  public event Action<IDamageable> OnRevived;

  void Awake()
  {
    currentHealth = maxHealth;
    //playerRenderer = GetComponentInChildren<Renderer>();
    //originalMaterial = playerRenderer.material;
    inputController = GetComponent<PlayerInputController>();
    IsDowned = false;
  }

  public void TakeDamage(float damage)
  {
    if (isInvincible || IsDowned) return;

    currentHealth -= damage;
    if (currentHealth < 0) currentHealth = 0;
    OnDamaged?.Invoke(damage, this);
    OnUpdateLife?.Invoke(currentHealth, maxHealth);

    StartCoroutine(DamageFeedbackCoroutine());

    if (currentHealth <= 0)
    {
      Die();
    }
  }

  private void Die()
  {
    if (inputController != null)
    {
      inputController.enabled = false;
    }

    IsDowned = true;
    OnDeath?.Invoke(this);
  }

  public void Revive(float healthRestored)
  {
    currentHealth = healthRestored;
    if (inputController != null)
    {
      inputController.enabled = true;
    }
    transform.rotation = Quaternion.identity; // Devuelve la rotación normal
    IsDowned = false;
    OnRevived?.Invoke(this);
    OnUpdateLife?.Invoke(currentHealth, maxHealth);
  }

  private IEnumerator DamageFeedbackCoroutine()
  {
    isInvincible = true;
    //playerRenderer.material = damagedMaterial;

    yield return new WaitForSeconds(invincibilityTime);

    //playerRenderer.material = originalMaterial;
    isInvincible = false;
  }

  public Vector3 GetPosition()
  {
    return transform.position;
  }

  public void IncreaseHealth(float increaseHealth)
  {
    currentHealth = Mathf.Clamp(currentHealth + increaseHealth, 0, maxHealth);
    OnUpdateLife?.Invoke(currentHealth, maxHealth);
  }
}

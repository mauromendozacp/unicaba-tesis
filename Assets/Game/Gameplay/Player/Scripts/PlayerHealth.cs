// PlayerHealth.cs
using UnityEngine;
using System.Collections;
using System;

public class PlayerHealth : MonoBehaviour, IDamageable
{
  [SerializeField] private float maxHealth = 100f;
  [SerializeField] private float invincibilityTime = 0.5f;
  [SerializeField] private Material damagedMaterial;

  private float currentHealth;
  public float Health => currentHealth;

  private bool isInvincible = false;
  private Material originalMaterial;
  private Renderer playerRenderer;
  private CharacterController characterController;

  public bool IsAlive => currentHealth > 0;

  public event Action<float, IDamageable> OnDamaged;
  public event Action<float, float> OnUpdateLife;
  public event Action<IDamageable> OnDeath;

  void Awake()
  {
    currentHealth = maxHealth;
    playerRenderer = GetComponentInChildren<Renderer>();
    originalMaterial = playerRenderer.material;
    characterController = GetComponent<CharacterController>();
  }

  public void TakeDamage(float damage)
  {
    if (isInvincible) return;

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
    // Desactivar CharacterController
    if (characterController != null)
    {
      characterController.enabled = false;
    }

    // Simular que el personaje está acostado
    transform.rotation = Quaternion.Euler(90, transform.rotation.y, transform.rotation.z);
    OnDeath?.Invoke(this);
  }

  private IEnumerator DamageFeedbackCoroutine()
  {
    isInvincible = true;
    playerRenderer.material = damagedMaterial;

    yield return new WaitForSeconds(invincibilityTime);

    playerRenderer.material = originalMaterial;
    isInvincible = false;
  }
}

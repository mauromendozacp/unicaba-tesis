// PlayerHealth.cs
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour, IDamageable, IRevivable
{
  [SerializeField] private float invincibilityTime = 0.5f;
  [SerializeField] private Material goldenMaterial;

  private float maxHealth = 0f;
  private float currentHealth = 0f;
  private bool isInvincible = false;
  private bool isGoldenEffectActive = false;

  public float Health => currentHealth;

  private Material[] originalMaterials;
  private Renderer[] playerRenderers;
  private PlayerInputGameplayController inputController;

  //[SerializeField] float knockbackForceMultiplier = 0.5f;
  [SerializeField] protected Material damagedMaterial;
  protected Material originalMaterial;

  public bool IsAlive => currentHealth > 0;
  public bool IsDowned { get; private set; }
  public bool IsRevivable => IsDowned;

  public event Action<float, IDamageable> OnDamaged;
  public event Action<float, float> OnUpdateLife;
  public event Action<IDamageable> OnDeath;
  public event Action<IDamageable> OnRevived;

  void Awake()
  {
    playerRenderers = GetComponentsInChildren<Renderer>();
    originalMaterials = new Material[playerRenderers.Length];
    for (int i = 0; i < playerRenderers.Length; i++)
    {
      originalMaterials[i] = playerRenderers[i].material;
    }
    inputController = GetComponent<PlayerInputGameplayController>();
    IsDowned = false;
  }

  public void ToggleDamageMaterial(bool active)
  {
    if (playerRenderers != null && damagedMaterial != null && originalMaterials != null)
    {
      for (int i = 0; i < playerRenderers.Length; i++)
      {
        if (playerRenderers[i] != null)
        {
          playerRenderers[i].material = active ? damagedMaterial : originalMaterials[i];
        }
      }
    }
    else
    {
      Debug.LogWarning("PlayerHealth: Renderers or materials not properly assigned.");
    }
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
      inputController.PlayerMap.Disable();
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
    ToggleDamageMaterial(true);
    yield return new WaitForSeconds(invincibilityTime);
    ToggleDamageMaterial(false);
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

  public void SetInitialData(float maxHealth)
  {
    this.maxHealth = maxHealth;
    currentHealth = maxHealth;
  }

  public void SetInvincibility(bool invincible)
  {
    isInvincible = invincible;

    if (invincible)
      EnableGoldenEffect();
    else
      DisableGoldenEffect();
  }

  private void EnableGoldenEffect()
  {
    if (isGoldenEffectActive || goldenMaterial == null) return;

    RefreshRenderers();

    isGoldenEffectActive = true;
    for (int i = 0; i < playerRenderers.Length; i++)
    {
      if (playerRenderers[i] != null)
      {
        playerRenderers[i].material = goldenMaterial;
      }
    }
  }

  private void DisableGoldenEffect()
  {
    if (!isGoldenEffectActive) return;

    isGoldenEffectActive = false;
    for (int i = 0; i < playerRenderers.Length; i++)
    {
      if (playerRenderers[i] != null && i < originalMaterials.Length && originalMaterials[i] != null)
      {
        playerRenderers[i].material = originalMaterials[i];
      }
    }
  }

  private void RefreshRenderers()
  {
    playerRenderers = GetComponentsInChildren<Renderer>();

    if (originalMaterials == null || originalMaterials.Length != playerRenderers.Length)
    {
      originalMaterials = new Material[playerRenderers.Length];
    }

    for (int i = 0; i < playerRenderers.Length; i++)
    {
      if (playerRenderers[i] != null && originalMaterials[i] == null)
      {
        originalMaterials[i] = playerRenderers[i].material;
      }
    }
  }

  [ContextMenu("LLenar Vida")]
  public void FillHealth()
  {
    currentHealth = maxHealth;
    OnUpdateLife?.Invoke(currentHealth, maxHealth);
  }
}

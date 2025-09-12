// PlayerHealth.cs
using UnityEngine;
using UnityEngine.InputSystem; // Necesario para el nuevo sistema de input
using System.Collections;

public class PlayerHealth : MonoBehaviour, IDamageable
{
  [SerializeField] private float maxHealth = 100f;
  [SerializeField] private float invincibilityTime = 0.5f;
  [SerializeField] private Material damagedMaterial;

  private float currentHealth;
  private bool isInvincible = false;
  private Material originalMaterial;
  private Renderer playerRenderer;
  private CharacterController characterController;
  private PlayerInput playerInput;

  public bool IsAlive => currentHealth > 0;

  void Awake()
  {
    currentHealth = maxHealth;
    playerRenderer = GetComponentInChildren<Renderer>();
    originalMaterial = playerRenderer.material;
    characterController = GetComponent<CharacterController>();
    playerInput = GetComponent<PlayerInput>();
  }

  public void TakeDamage(float damage)
  {
    if (isInvincible) return;

    currentHealth -= damage;
    if (currentHealth < 0) currentHealth = 0;

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

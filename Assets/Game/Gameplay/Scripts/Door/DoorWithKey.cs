using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorWithKey : MonoBehaviour
{
  [Header("Animación")]
  [SerializeField] private Animator doorAnimator = null;

  [Tooltip("Si está activo, la puerta solo se abre una vez.")]
  [SerializeField] private bool openOnce = true;

  [Header("Bloqueo de movimiento")]
  [Tooltip("Tiempo en segundos que el jugador queda bloqueado mientras se abre la puerta.")]
  [SerializeField] private float lockDuration = 0.5f;

  [SerializeField] AudioEvent doorOpeningSound = null;
  [SerializeField] AudioEvent doorClosedSound = null;

  private bool isOpen = false;

  private void Reset()
  {
    // Este collider (en el hijo) debe ser trigger
    Collider col = GetComponent<Collider>();
    if (col != null)
      col.isTrigger = true;

    // Buscamos el animator en el padre (la puerta)
    if (doorAnimator == null)
      doorAnimator = GetComponentInParent<Animator>();
  }

  private void Awake()
  {
    if (doorAnimator == null)
      doorAnimator = GetComponentInParent<Animator>();
  }

  private void OnTriggerEnter(Collider other)
  {
    if (openOnce && isOpen)
      return;

    // Solo reaccionamos si lo que entra es (o tiene) un PlayerController
    PlayerController player = other.GetComponent<PlayerController>();
    if (player == null)
      player = other.GetComponentInParent<PlayerController>();

    if (player == null)
      return;

    if (KeysManager.Instance == null)
    {
      Debug.LogWarning("[DoorWithKey] No hay KeysManager en la escena.");
      return;
    }

    if (KeysManager.Instance.TryUseKey())
    {
      StartCoroutine(OpenDoorSequence(player));
    }
    else
    {
      Debug.Log("[DoorWithKey] No hay llaves suficientes para abrir la puerta.");
      if (doorClosedSound != null)
      {
        GameManager.Instance.AudioManager.PlayAudio(doorClosedSound);
      }
    }
  }

  private IEnumerator OpenDoorSequence(PlayerController player)
  {
    if (player != null)
    {
      // Bloqueamos input de gameplay mientras se abre
      player.ToggleGameplayInputs(false);
    }

    OpenDoorInternal();

    if (lockDuration > 0f)
      yield return new WaitForSeconds(lockDuration);

    if (player != null)
    {
      player.ToggleGameplayInputs(true);
    }
  }

  private void OpenDoorInternal()
  {
    if (isOpen && openOnce)
      return;

    isOpen = true;
    if (doorOpeningSound != null)
    {
      GameManager.Instance.AudioManager.PlayAudio(doorOpeningSound);
    }


    if (doorAnimator == null)
      doorAnimator = GetComponentInParent<Animator>();

    if (doorAnimator != null)
    {
      // Tu caso: el Animator ya está en la puerta, solo hay que activarlo
      doorAnimator.enabled = true;
    }
    else
    {
      // Fallback por si no hay Animator: abre "teleport" girando algo
      Transform doorRoot = transform.parent != null ? transform.parent : transform;
      doorRoot.Rotate(0f, 90f, 0f);
    }
  }
}

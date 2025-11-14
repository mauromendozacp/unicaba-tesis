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

    private bool isOpen = false;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        if (doorAnimator == null)
            doorAnimator = GetComponentInChildren<Animator>();
    }

    private void Awake()
    {
        if (doorAnimator == null)
            doorAnimator = GetComponentInChildren<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (openOnce && isOpen)
            return;

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
        }
    }

    private IEnumerator OpenDoorSequence(PlayerController player)
    {
        if (player != null)
        {
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

        if (doorAnimator == null)
            doorAnimator = GetComponentInChildren<Animator>();

        if (doorAnimator != null)
        {
            doorAnimator.enabled = true;
        }
        else
        {
            transform.Rotate(0f, 90f, 0f);
        }
    }
}

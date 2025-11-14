using UnityEngine;

[RequireComponent(typeof(Collider))]
public class DoorWithKey : MonoBehaviour
{
    [Header("Animación")]
    [SerializeField] private Animator doorAnimator = null;

    [Tooltip("Nombre del trigger en el Animator que abre la puerta")]
    [SerializeField] private string openTriggerName = "Open";

    [Header("Opciones")]
    [Tooltip("Si está activo, la puerta solo se abre una vez.")]
    [SerializeField] private bool openOnce = true;

    private bool isOpen = false;

    private void Reset()
    {
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;

        if (doorAnimator == null)
            doorAnimator = GetComponentInChildren<Animator>();
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
            OpenDoor();
        }
        else
        {
            Debug.Log("[DoorWithKey] No hay llaves suficientes para abrir la puerta.");
        }
    }

    private void OpenDoor()
    {
        if (isOpen && openOnce)
            return;

        isOpen = true;

        if (doorAnimator != null && !string.IsNullOrEmpty(openTriggerName))
        {
            doorAnimator.SetTrigger(openTriggerName);
        }
        else
        {
            transform.Rotate(0f, 90f, 0f);
        }
    }
}

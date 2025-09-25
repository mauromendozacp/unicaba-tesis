using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ReviveController : MonoBehaviour
{
  [Header("Revive Settings")]
  [SerializeField] private float reviveTime = 10f;
  [SerializeField] private float reviveDetectRange = 2f;
  [SerializeField] private float reviveAmount = 50f;

  [Header("Revive Feedback")]
  [SerializeField] private GameObject reviveEffectSphere;
  [SerializeField] private TextMeshProUGUI reviveTimerText;

  private Coroutine reviveCoroutine = null;
  private IRevivable targetToRevive = null;

  public bool IsReviving { get; private set; } = false;

  private PlayerHealth playerHealth = null;

  // Eventos para notificar a otras clases sobre el estado de revivir
  public event Action OnReviveStart;
  public event Action OnReviveCancel;
  public event Action<IRevivable> OnReviveComplete;

  private void Awake()
  {
    playerHealth = GetComponent<PlayerHealth>();
    reviveTimerText.text = ((int)(reviveTime)).ToString();
    if (reviveEffectSphere != null) reviveEffectSphere.SetActive(false);
    if (reviveTimerText != null) reviveTimerText.gameObject.SetActive(false);
  }

  public bool TryStartRevive()
  {
    if (IsReviving) return false;

    targetToRevive = FindRevivableTarget();
    if (targetToRevive != null)
    {
      IsReviving = true;
      OnReviveStart?.Invoke();
      reviveCoroutine = StartCoroutine(ReviveTeammateCoroutine());
      return true;
    }

    return false;
  }

  public void CancelRevive()
  {
    if (reviveCoroutine != null)
    {
      StopCoroutine(reviveCoroutine);
      reviveCoroutine = null;
    }
    targetToRevive = null;
    if (reviveEffectSphere != null) reviveEffectSphere.SetActive(false);
    if (reviveTimerText != null) reviveTimerText.gameObject.SetActive(false);
    IsReviving = false;
    OnReviveCancel?.Invoke();
  }

  private IRevivable FindRevivableTarget()
  {
    /*
    IRevivable[] allRevivables = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None) as IRevivable[];
    if (allRevivables == null || allRevivables.Length == 0) return null;
    Debug.Log("Found " + allRevivables.Length + " revivable entities in the scene.");

    foreach (var revivable in allRevivables)
    {
      if (revivable.IsRevivable && Vector3.Distance(transform.position, revivable.GetPosition()) <= reviveDetectRange)
      {
        return revivable;
      }
    }

    return null;
    */

    Collider[] hitColliders = Physics.OverlapSphere(transform.position, reviveDetectRange);

    foreach (var hitCollider in hitColliders)
    {
      IRevivable revivable = hitCollider.GetComponent<IRevivable>();
      if (revivable != null && revivable.IsRevivable)
      {
        return revivable;
      }
    }
    return null;
  }

  private IEnumerator ReviveTeammateCoroutine()
  {
    if (reviveEffectSphere != null) reviveEffectSphere.SetActive(true);
    if (reviveTimerText != null) reviveTimerText.gameObject.SetActive(true);

    float timer = reviveTime;
    int previousTimerInt = (int)Mathf.Ceil(timer);
    while (timer > 0f)
    {
      int currentTimerInt = (int)Mathf.Ceil(timer);
      if (reviveTimerText != null && currentTimerInt != previousTimerInt)
      {
        reviveTimerText.text = currentTimerInt.ToString();
        previousTimerInt = currentTimerInt;
      }

      // Comprobación de interrupción:
      if (targetToRevive == null || !targetToRevive.IsRevivable || Vector3.Distance(transform.position, targetToRevive.GetPosition()) > reviveDetectRange || playerHealth.IsDowned)
      {
        CancelRevive();
        yield break;
      }

      timer -= Time.deltaTime;
      yield return null;
    }

    if (targetToRevive != null)
    {
      targetToRevive.Revive(reviveAmount);
      OnReviveComplete?.Invoke(targetToRevive);
    }

    if (reviveTimerText != null)
    {
      reviveTimerText.text = "0";
    }

    CancelRevive();
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.green;
    Gizmos.DrawWireSphere(transform.position, reviveDetectRange);
  }
}

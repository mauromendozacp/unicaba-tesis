using UnityEngine;

public class Sawblade : MonoBehaviour
{
  [Tooltip("Cantidad de daño que infligirá la sierra.")]
  [SerializeField] private float damage = 100f;

  private Animator sawAnimator;
  private Collider sawCollider;
  private const string SawSpeedParam = "SawSpeed"; // Nombre de tu parámetro de velocidad en el Animator

  private void Awake()
  {
    // Obtener los componentes necesarios al inicio.
    sawAnimator = GetComponent<Animator>();
    sawCollider = GetComponent<Collider>();
  }

  private void Start()
  {
    float randomSpeed = Random.Range(0.4f, 0.7f);
    sawAnimator.speed = randomSpeed;
    float randomStartTime = Random.Range(0f, 1f);
    sawAnimator.Play("Sierra", 0, randomStartTime);
  }

  private void OnTriggerEnter(Collider other)
  {
    IDamageable damageable = other.GetComponent<IDamageable>();
    if (damageable != null && damageable.IsAlive)
    {
      BreakSawblade();
      damageable.TakeDamage(damage);
    }
  }

  private void BreakSawblade()
  {
    if (sawAnimator != null)
    {
      sawAnimator.enabled = false;
    }

    if (sawCollider != null)
    {
      sawCollider.enabled = false;
    }
  }
}
using UnityEngine;

public class SpiderEnemy : EnemyBase
{
  [Header("Attack Settings")]
  [SerializeField] private Collider attackCollider;
  [SerializeField] private float attackDamage = 40f;

  SpiderAnimationController animator;
  public SpiderAnimationController Animator => animator;


  void OnEnable()
  {
    currentHealth = maxHealth;
    ChangeState(new SpiderIdleState(this));

    if (selfCollider != null) selfCollider.enabled = true;
    EnableMovementAndCollisions();
  }

  protected override void Awake()
  {
    animator = GetComponent<SpiderAnimationController>();
    base.Awake();
    ChangeState(new SpiderIdleState(this));
  }

  void Start()
  {
    if (attackCollider != null)
    {
      attackCollider.enabled = false;
    }
  }

  void OnTriggerEnter(Collider other)
  {
    if (!other.CompareTag("Player")) return;
    IDamageable player = other.GetComponent<IDamageable>();
    if (player != null && !player.IsAlive)
    {
      CurrentTarget = null;
      ChangeState(new SpiderIdleState(this));
    }

    if (attackCollider != null && attackCollider.enabled && player != null)
    {
      player.TakeDamage(attackDamage);
    }
  }


  public override void TakeDamage(float damage)
  {
    base.TakeDamage(damage);

    if (currentHealth <= 0)
    {
      ChangeState(new SpiderDeathState(this));
    }
    else
    {
      ChangeState(new SpiderDamagedState(this));
    }
  }

  public void SetAttackCollider(bool active)
  {
    if (attackCollider != null)
    {
      attackCollider.enabled = active;
    }
  }
}
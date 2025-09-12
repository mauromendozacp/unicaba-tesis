// IDamageable.cs
public interface IDamageable
{
  bool IsAlive { get; }
  void TakeDamage(float damage);
}

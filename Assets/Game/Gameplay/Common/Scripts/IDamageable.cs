// IDamageable.cs
public interface IDamageable
{
  float Health { get; }
  bool IsAlive { get; }
  void TakeDamage(float damage);
}

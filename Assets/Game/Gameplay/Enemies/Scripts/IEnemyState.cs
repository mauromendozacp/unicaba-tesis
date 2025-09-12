public interface IEnemyState
{
  public EnemyState State { get; }
  void Enter();
  void Update();
  void Exit();
}
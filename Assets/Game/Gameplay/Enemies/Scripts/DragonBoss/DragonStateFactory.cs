public class DragonStateFactory
{
  private DragonBossController _context;

  public DragonStateFactory(DragonBossController context)
  {
    _context = context;
  }

  // --- Estados Terrestres (Vulnerables) ---
  public IState GroundIdle() => new DragonGroundIdleState(_context, this);
  public IState GroundMove() => new DragonGroundMoveState(_context, this);
  public IState GroundAttack(string attackAnim) => new DragonGroundAttackState(_context, this, attackAnim);

  // --- Estados de Transición (Invulnerabilidad) ---
  public IState TransitionTakeoff() => new DragonTransitionTakeoffState(_context, this);
  public IState TransitionLanding() => new DragonTransitionLandingState(_context, this);

  // --- Estados Aéreos (Invulnerables) ---
  public IState AirHoverAttack() => new DragonAirHoverAttackState(_context, this);
  public IState AirFlyReposition() => new DragonAirFlyRepositionState(_context, this);

  // --- Estado Final ---
  public IState Die() => new DragonDieState(_context, this);
}
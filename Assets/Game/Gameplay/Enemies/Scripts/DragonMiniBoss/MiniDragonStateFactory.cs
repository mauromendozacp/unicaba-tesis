public class MiniDragonStateFactory
{
  private MiniDragonController _context;

  public MiniDragonStateFactory(MiniDragonController context)
  {
    _context = context;
  }

  // --- Estados Iniciales ---
  public IState Sleeping() => new MiniDragonSleepingState(_context, this);
  public IState InitialTravel() => new MiniDragonInitialTravelState(_context, this);

  // --- Estados Terrestres (Vulnerables) ---
  public IState GroundIdle() => new MiniDragonGroundIdleState(_context, this);
  public IState GroundMove() => new MiniDragonGroundMoveState(_context, this);
  public IState MeleeAttack(string animName) => new MiniDragonMeleeAttackState(_context, this, animName);
  public IState RangedAttack() => new MiniDragonRangedAttackState(_context, this);

  // --- Estados Aéreos (Bajo Vuelo) ---
  public IState TransitionTakeoff() => new MiniDragonTransitionTakeoffState(_context, this);
  public IState AirReposition() => new MiniDragonAirRepositionState(_context, this);
  public IState AirRangedAttack() => new MiniDragonAirRangedAttackState(_context, this);
  public IState TransitionLanding() => new MiniDragonTransitionLandingState(_context, this);

  // --- Estado Final ---
  public IState Die() => new MiniDragonDieState(_context, this);
}
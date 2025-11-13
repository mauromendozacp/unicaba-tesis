using UnityEngine;

public class DragonDoor : MonoBehaviour
{
  Animator _anim;
  [SerializeField] EnemyManager _enemyManager;

  void Awake()
  {
    _anim = GetComponent<Animator>();
    _anim.enabled = false;
  }

  void Start()
  {
    if (_enemyManager != null)
    {
      _enemyManager.OnWavesEnd += Open;
    }
    else
    {
      Debug.LogWarning("EnemyManager no asignado en DragonDoor.");
    }
  }

  public void Open()
  {
    _anim.enabled = true;
  }
}

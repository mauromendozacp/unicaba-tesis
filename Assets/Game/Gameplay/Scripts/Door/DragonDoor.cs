using UnityEngine;

public class DragonDoor : MonoBehaviour
{
  Animator _anim;
  [SerializeField] EnemyManager _enemyManager;
  [SerializeField] AudioEvent doorOpeningSound = null;

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
    if (doorOpeningSound != null)
    {
      GameManager.Instance.AudioManager.PlayAudio(doorOpeningSound);
    }
    _anim.enabled = true;

  }
}

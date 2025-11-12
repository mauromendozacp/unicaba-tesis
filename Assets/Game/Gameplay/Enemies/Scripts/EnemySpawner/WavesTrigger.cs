using UnityEngine;

public class WavesTrigger : MonoBehaviour
{
  [SerializeField] EnemyManager enemyManager;
  [SerializeField] WavesTrigger twinTrigger;

  private void OnTriggerEnter(Collider other)
  {
    if (other.CompareTag("Player"))
    {
      enemyManager.StartEnemyWaves();
      gameObject.SetActive(false);
    }
  }

  private void OnDisable()
  {
    if (twinTrigger != null)
    {
      twinTrigger.gameObject.SetActive(false);
    }
  }
}

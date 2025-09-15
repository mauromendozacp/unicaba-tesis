using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class EnemyManager : MonoBehaviour
{
  [SerializeField] List<GameObject> enemyPrefabs;
  [SerializeField] int poolInitialSize = 10;
  [SerializeField] int poolMaxSize = 20;
  [SerializeField] List<Transform> spawnCenters;
  [SerializeField] float maxSpawnRadius = 5f;
  [SerializeField] int initialEnemiesCount = 2;
  [SerializeField] int enemiesToAddPerWave = 1;
  [SerializeField] float waveCooldownTime = 2f;

  private IObjectPool<GameObject> enemyPool;
  private int currentEnemiesCount;
  private int enemiesAlive;

  // Propiedad Pública (para que los enemigos notifiquen su muerte)
  public static EnemyManager Instance;


  private void Awake()
  {
    if (Instance == null)
    {
      Instance = this;
    }
    else
    {
      Destroy(gameObject);
    }
  }

  void Start()
  {
    InitializePool();
    currentEnemiesCount = initialEnemiesCount;
    StartCoroutine(SpawnWave());
  }

  void InitializePool()
  {
    enemyPool = new ObjectPool<GameObject>(
        createFunc: CreatePooledItem,
        actionOnGet: OnTakeFromPool,
        actionOnRelease: OnReturnToPool,
        actionOnDestroy: OnDestroyPoolObject,
        collectionCheck: false,
        defaultCapacity: poolInitialSize,
        maxSize: poolMaxSize
    );

    // Pre-poblar el pool con algunos enemigos.
    for (int i = 0; i < poolInitialSize; i++)
    {
      enemyPool.Release(CreatePooledItem());
    }
  }

  private GameObject CreatePooledItem()
  {
    GameObject prefabToInstantiate = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
    GameObject enemy = Instantiate(prefabToInstantiate);

    // Agregar una referencia al pool en el script del enemigo para que pueda ser devuelto.
    EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
    if (enemyScript != null)
    {
      enemyScript.SetPool(enemyPool);
    }

    return enemy;
  }

  private void OnTakeFromPool(GameObject enemy)
  {
    // Esto se llama cada vez que se toma un enemigo del pool.
    enemy.SetActive(true);
  }

  private void OnReturnToPool(GameObject enemy)
  {
    // Esto se llama cada vez que un enemigo es devuelto al pool.
    enemy.SetActive(false);
  }

  private void OnDestroyPoolObject(GameObject enemy)
  {
    // Esto se llama cuando el pool destruye un objeto (por ejemplo, al exceder el max size).
    Destroy(enemy);
  }

  IEnumerator SpawnWave()
  {
    yield return new WaitForSeconds(waveCooldownTime); // Esperar entre oleadas
    enemiesAlive = currentEnemiesCount;

    for (int i = 0; i < currentEnemiesCount; i++)
    {
      GameObject enemyToSpawn = enemyPool.Get();
      if (enemyToSpawn != null)
      {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        enemyToSpawn.transform.position = spawnPosition;
      }
    }
  }

  Vector3 GetRandomSpawnPosition()
  {
    Transform selectedCenter = spawnCenters[Random.Range(0, spawnCenters.Count)];
    Vector2 randomCircle = Random.insideUnitCircle * maxSpawnRadius;
    return selectedCenter.position + new Vector3(randomCircle.x, 1, randomCircle.y);
  }

  // Método para que los enemigos lo llamen al morir
  public void OnEnemyKilled()
  {
    enemiesAlive--;
    if (enemiesAlive <= 0)
    {
      Debug.Log("¡Oleada completada! Preparando la siguiente...");
      currentEnemiesCount += enemiesToAddPerWave;
      StartCoroutine(SpawnWave());
    }
  }
}
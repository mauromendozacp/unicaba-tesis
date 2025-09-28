using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using System;

public class EnemyManager : MonoBehaviour
{
  public event Action<int> OnWavesStart;   // Pasa el número total de oleadas
  public event Action OnWaveStart;
  public event Action OnWavesEnd;

  [Header("General")]
  [Tooltip("Lista general de todos los prefabs de enemigos.")]
  public List<GameObject> enemyPrefabs;

  [Header("Configuración de Oleadas")]
  [Tooltip("Lista de todas las oleadas de enemigos.")]
  public List<Wave> waves;
  [Tooltip("El tamaño máximo que puede alcanzar el pool.")]
  public int poolMaxSize = 50;

  [Header("Puntos de Spawn")]
  [Tooltip("Lista de Transform de los puntos centrales para el spawn.")]
  public List<Transform> spawnCenters;
  [Tooltip("El radio máximo desde un centro donde se puede instanciar un enemigo.")]
  public float maxSpawnRadius = 5f;

  [Header("Controller to drop items")]
  [SerializeField] ItemController itemController;

  private Dictionary<string, GameObject> enemyPrefabDict;
  //private IObjectPool<GameObject> enemyPool;
  private Dictionary<string, IObjectPool<GameObject>> enemyPoolDict;
  private int currentWaveIndex = 0;
  private int enemiesToSpawnInCurrentWave;
  private int enemiesAlive;
  private List<int> dropIndices;

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

    enemyPrefabDict = new Dictionary<string, GameObject>();
    foreach (GameObject prefab in enemyPrefabs)
    {
      if (prefab != null)
      {
        enemyPrefabDict[prefab.name] = prefab;
      }
    }
    enemyPoolDict = new Dictionary<string, IObjectPool<GameObject>>();
  }

  void Start()
  {
    if (itemController == null)
    {
      itemController = FindFirstObjectByType<ItemController>();
    }
    InitializePool();
    StartCoroutine(StartWaves());
  }

  void InitializePool()
  {
    /*
    enemyPool = new ObjectPool<GameObject>(
        createFunc: CreatePooledItem,
        actionOnGet: OnTakeFromPool,
        actionOnRelease: OnReturnToPool,
        actionOnDestroy: OnDestroyPoolObject,
        collectionCheck: false,
        defaultCapacity: 20,
        maxSize: poolMaxSize
    );
    */
    foreach (GameObject prefab in enemyPrefabs)
    {
      if (!enemyPoolDict.ContainsKey(prefab.name))
      {
        // Crea un pool para cada prefab de enemigo.
        var pool = new ObjectPool<GameObject>(
            createFunc: () => CreatePooledItem(prefab), // Se pasa el prefab a la función
            actionOnGet: OnTakeFromPool,
            actionOnRelease: OnReturnToPool,
            actionOnDestroy: OnDestroyPoolObject,
            collectionCheck: false,
            defaultCapacity: 20,
            maxSize: poolMaxSize
        );
        enemyPoolDict.Add(prefab.name, pool);
      }
    }
  }

  /*
    private GameObject CreatePooledItem()
    {
      if (enemyPrefabs.Count == 0)
      {
        Debug.LogError("No hay prefabs de enemigos en la lista general.");
        return null;
      }

      GameObject prefabToInstantiate = enemyPrefabs[UnityEngine.Random.Range(0, enemyPrefabs.Count)];
      GameObject enemy = Instantiate(prefabToInstantiate);

      EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
      if (enemyScript != null)
      {
        enemyScript.SetPool(enemyPool);
      }
      return enemy;
    }
  */

  private GameObject CreatePooledItem(GameObject prefabToInstantiate)
  {
    GameObject enemy = Instantiate(prefabToInstantiate);

    EnemyBase enemyScript = enemy.GetComponent<EnemyBase>();
    if (enemyScript != null)
    {
      // Se modifica para que use el pool correspondiente al tipo de enemigo.
      enemyScript.SetPool(enemyPoolDict[prefabToInstantiate.name]);
    }
    return enemy;
  }
  private void OnTakeFromPool(GameObject enemy)
  {
    enemy.SetActive(true);
  }

  private void OnReturnToPool(GameObject enemy)
  {
    enemy.SetActive(false);
  }

  private void OnDestroyPoolObject(GameObject enemy)
  {
    //Destroy(enemy);
    DestroyImmediate(enemy);
  }

  IEnumerator StartWaves()
  {
    OnWavesStart?.Invoke(waves.Count);

    for (currentWaveIndex = 0; currentWaveIndex < waves.Count; currentWaveIndex++)
    {
      OnWaveStart?.Invoke();

      yield return StartCoroutine(SpawnCurrentWave());

      yield return new WaitForSeconds(waves[currentWaveIndex].cooldownTime);
    }

    OnWavesEnd?.Invoke();
  }

  IEnumerator SpawnCurrentWave()
  {
    if (waves.Count == 0 || currentWaveIndex >= waves.Count)
    {
      yield break;
    }

    Wave currentWave = waves[currentWaveIndex];
    enemiesToSpawnInCurrentWave = 0;

    foreach (var waveContent in currentWave.enemiesInWave)
    {
      enemiesToSpawnInCurrentWave += waveContent.count;
    }

    enemiesAlive = enemiesToSpawnInCurrentWave;

    dropIndices = new List<int>();
    int itemsToDrop = currentWave.itemsToDropInWave;

    if (itemsToDrop > 0 && enemiesToSpawnInCurrentWave > 0)
    {
      int actualDrops = Mathf.Min(itemsToDrop, enemiesToSpawnInCurrentWave);
      List<int> allIndices = new List<int>();
      for (int i = 1; i <= enemiesToSpawnInCurrentWave; i++)
      {
        allIndices.Add(i);
      }
      for (int i = 0; i < actualDrops; i++)
      {
        int randomIndex = UnityEngine.Random.Range(0, allIndices.Count);
        dropIndices.Add(allIndices[randomIndex]);
        allIndices.RemoveAt(randomIndex);
      }
      dropIndices.Sort();
    }

    foreach (var enemySpawn in currentWave.enemiesInWave)
    {
      /*
      for (int i = 0; i < enemySpawn.count; i++)
      {
        GameObject enemyToSpawn = enemyPool.Get();
        if (enemyToSpawn != null)
        {
          // Reiniciar la posición y otros estados del enemigo
          enemyToSpawn.transform.position = GetRandomSpawnPosition();
          enemyToSpawn.SetActive(true);
        }
      }
      */
      string enemyPrefabName = enemySpawn.enemyPrefab.name;
      if (enemyPoolDict.ContainsKey(enemyPrefabName))
      {
        var specificEnemyPool = enemyPoolDict[enemyPrefabName];
        for (int i = 0; i < enemySpawn.count; i++)
        {
          GameObject enemyToSpawn = specificEnemyPool.Get();
          if (enemyToSpawn != null)
          {
            enemyToSpawn.transform.position = GetRandomSpawnPosition();
            enemyToSpawn.SetActive(true);
          }
        }
      }
      else
      {
        Debug.LogError($"Pool para el enemigo '{enemyPrefabName}' no encontrado. Asegúrate de que el prefab está en la lista general de prefabs del EnemyManager.");
      }
    }

    while (enemiesAlive > 0)
    {
      yield return null;
    }
  }

  Vector3 GetRandomSpawnPosition()
  {
    Vector2 randomCircle = UnityEngine.Random.insideUnitCircle * maxSpawnRadius; ;
    if (spawnCenters.Count == 0)
    {
      Debug.LogWarning("No hay centros de spawn definidos. Usando la posición del EnemyManager como fallback.");
      return transform.position + new Vector3(randomCircle.x, 1.2f, randomCircle.y);
    }

    Transform selectedCenter = spawnCenters[UnityEngine.Random.Range(0, spawnCenters.Count)];
    return selectedCenter.position + new Vector3(randomCircle.x, 1.2f, randomCircle.y);
  }

  public void OnEnemyKilled(Vector3 enemyPosition)
  {
    enemiesAlive--;
    if (enemiesAlive >= 0)
    {
      int currentKillIndex = enemiesToSpawnInCurrentWave - enemiesAlive;
      if (dropIndices != null && dropIndices.Contains(currentKillIndex))
      {
        // dropIndices.Remove(currentKillIndex); 
        if (itemController != null)
        {
          Vector3 itemPosition = new Vector3(enemyPosition.x, 1, enemyPosition.z);
          itemController.SpawnRandomItem(itemPosition);
          //Debug.Log($"Item dropped at kill index {currentKillIndex} at position {itemPosition} and wave {currentWaveIndex + 1}");
        }
      }
    }
  }
}
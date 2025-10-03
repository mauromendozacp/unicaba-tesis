using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class Spawner<TSettings, TController> : MonoBehaviour
    where TSettings : ItemSpawnSettings
{
    private Dictionary<int, Coroutine> respawnCoroutines = new Dictionary<int, Coroutine>();

    protected abstract List<TSettings> GetSpawnSettings();
    protected abstract void SpawnAtPosition(TSettings settings, int spawnIndex);
    protected abstract void OnObjectCollectedOrDestroyed(TSettings settings, int spawnIndex);

    private void Start()
    {
        foreach (var settings in GetSpawnSettings())
        {
            for (int i = 0; i < settings.SpawnPositions.Count; i++)
            {
                SpawnAtPosition(settings, i);
            }
        }
    }

    protected void StartRespawnCoroutine(TSettings settings, int spawnIndex)
    {
        if (respawnCoroutines.ContainsKey(spawnIndex))
            StopCoroutine(respawnCoroutines[spawnIndex]);

        respawnCoroutines[spawnIndex] = StartCoroutine(RespawnObject(settings, spawnIndex));
    }

    private IEnumerator RespawnObject(TSettings settings, int spawnIndex)
    {
        yield return new WaitForSeconds(settings.RespawnTime);
        SpawnAtPosition(settings, spawnIndex);
    }
}

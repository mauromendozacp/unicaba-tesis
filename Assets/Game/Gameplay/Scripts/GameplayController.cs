using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
  [SerializeField] private GameplayUI gameplayUI = null;
  [SerializeField] private PlayerSpawn playerSpawn = null;
  [SerializeField] private EnemyManager enemyManager = null;
  [SerializeField] private int maxKeysToWin = 0;

  private int keys = 0;
  private bool lastWaveToWin = false;

  private void Awake()
  {
    playerSpawn.Init(gameplayUI.GetPlayerUI, () =>
    {
      TogglePause(true);
      gameplayUI.TogglePause(true);
    },
    OnPlayerDeath, AddKeys, gameplayUI.OnJoinPlayers);

    enemyManager.OnWaveStart += () => { gameplayUI.ToggleWave(true); };
    enemyManager.OnWavesStart += gameplayUI.OnUpdateWave;
    enemyManager.OnWavesStart += CheckLastWave;
    enemyManager.OnWavesEnd += () => { gameplayUI.ToggleWave(false); };
    enemyManager.OnWavesEnd += CheckVictory;
    gameplayUI.ToggleWave(false);
  }

  private void CheckLastWave(int currentWave, int totalWaves)
  {
    lastWaveToWin = currentWave == totalWaves;
  }

  private void CheckVictory()
  {
    if (lastWaveToWin)
    {
      gameplayUI.OpenWinPanel();

      List<PlayerController> players = playerSpawn.GetPlayers();
      for (int i = 0; i < players.Count; i++)
      {
        players[i].ToggleInput(false);
      }

      //eliminar todos los enemigos
      enemyManager.KillAllEnemies();
    }
  }

  private void Start()
  {
    gameplayUI.Init(() => { TogglePause(false); }, GoToMenu, RetryLevel, NextLevel);
  }

  private void OnPlayerDeath()
  {
    List<PlayerController> players = playerSpawn.GetPlayers();
    for (int i = 0; i < players.Count; i++)
    {
      if (players[i].PlayerHealth.IsAlive)
      {
        return;
      }
    }

    gameplayUI.OpenLosePanel();
  }

  private void TogglePause(bool status)
  {
    Time.timeScale = status ? 0f : 1f;
  }

  private void GoToMenu()
  {
    TogglePause(false);
    GameManager.Instance.ChangeScene(SceneGame.Menu);
  }

  private void AddKeys()
  {
    keys++;
    gameplayUI.UpdateKeysText(keys);

    if (keys >= maxKeysToWin)
    {
      enemyManager.StartEnemyWaves();

    }
  }

  private void RetryLevel()
  {
    GameManager.Instance.ChangeScene(SceneGame.Gameplay);
  }

  private void NextLevel()
  {
    GameManager.Instance.ChangeScene(SceneGame.Gameplay);
  }
}

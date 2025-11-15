using System.Collections.Generic;
using UnityEngine;

public class GameplayController : MonoBehaviour
{
  [SerializeField] private GameplayUI gameplayUI = null;
  [SerializeField] private PlayerSpawn playerSpawn = null;
  [SerializeField] private EnemyManager enemyManager = null;
  [SerializeField] private int maxKeysToWin = 0;
  [SerializeField] private SceneGame nextLevel = default;

  [SerializeField] private MiniDragonController miniBoss;
  [SerializeField] private DragonBossController finalBoss;
  bool finalBossIsDead = false;

  private int keys = 0;
  private bool lastWaveToWin = false;

  private void Awake()
  {
    playerSpawn.Init(gameplayUI.GetPlayerUI, OnPause, OnPlayerDeath, AddKeys, gameplayUI.OnJoinPlayers);

    enemyManager.OnWaveStart += () => { gameplayUI.ToggleWave(true); };
    enemyManager.OnWavesStart += gameplayUI.OnUpdateWave;
    enemyManager.OnWavesStart += CheckLastWave;
    enemyManager.OnWavesEnd += () => { gameplayUI.ToggleWave(false); };
    //enemyManager.OnWavesEnd += CheckVictory;
    enemyManager.OnWavesEnd += WavesHasEnded;
    miniBoss.OnDragonMiniBossDeath += Victory;
    if (finalBoss != null)
    {
      finalBoss.OnDragonBossDeath += () =>
    {
      finalBossIsDead = true;
      Victory();
    };
    }
    gameplayUI.ToggleWave(false);
  }

  private void CheckLastWave(int currentWave, int totalWaves)
  {
    lastWaveToWin = currentWave == totalWaves;
  }

  private void WavesHasEnded()
  {
    gameplayUI.ToggleWave(false);
    //gameplayUI.ToggleMiniBoss(true);
    miniBoss.TriggerWakeUp();
  }

  private void Victory()
  {
    if (nextLevel == SceneGame.Menu && finalBoss != null && !finalBossIsDead)
    {
      finalBoss.gameObject.SetActive(true);
      return;
    }
    gameplayUI.OpenWinPanel();

    List<PlayerController> players = playerSpawn.GetPlayers();
    for (int i = 0; i < players.Count; i++)
    {
      players[i].ToggleInput(false);
    }
    enemyManager.KillAllEnemies();
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
    gameplayUI.Init(OnResume, GoToMenu, OnRestart, RetryLevel, NextLevel, ExitGame);
    if (finalBoss != null)
    {
      Debug.Log("Deshabilitando al jefe final al iniciar el nivel.");
      finalBoss.gameObject.SetActive(false);
    }
  }

  private void OnResume()
  {
    TogglePause(false);

    if (playerSpawn.PlayerInputs[0].currentControlScheme == "Gamepad")
    {
      GameManager.Instance.CursorManager.ToggleCursor(0, false);
    }

    List<PlayerController> players = playerSpawn.GetPlayers();
    for (int i = 0; i < players.Count; i++)
    {
      players[i].ToggleGameplayInputs(true);
    }
  }

  private void OnPause()
  {
    TogglePause(true);
    gameplayUI.TogglePause(true);

    if (playerSpawn.PlayerInputs[0].currentControlScheme == "Gamepad")
    {
      GameManager.Instance.CursorManager.ToggleCursor(0, true);
    }

    List<PlayerController> players = playerSpawn.GetPlayers();
    for (int i = 0; i < players.Count; i++)
    {
      players[i].ToggleGameplayInputs(false);
    }
  }

  private void OnRestart()
  {
    TogglePause(false);
    GameManager.Instance.ChangeScene(SceneGame.Gameplay_1);
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
    GameManager.Instance.GameDataManager.playersData.Clear();

    TogglePause(false);
    GameManager.Instance.ChangeScene(SceneGame.Menu);
  }

  private void ExitGame()
  {
    Application.Quit();
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
    GameManager.Instance.ChangeScene(SceneGame.Gameplay_1);
  }

  private void NextLevel()
  {
    GameManager.Instance.ChangeScene(nextLevel);
  }
}

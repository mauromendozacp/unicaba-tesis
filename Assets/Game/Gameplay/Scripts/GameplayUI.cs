using System;
using TMPro;
using Unity.VisualScripting.FullSerializer.Internal;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private PlayerUI[] players = null;

    [SerializeField] private GameObject pausePanel = null;
    [SerializeField] private Button resumeBtn = null;
    [SerializeField] private Button optionsBtn = null;
    [SerializeField] private Button restartBtn = null;
    [SerializeField] private Button menuBtn = null;

    [SerializeField] private GameObject wavePanel = null;
    [SerializeField] private TMP_Text waveText = null;

    [SerializeField] private GameObject hudPanel = null;
    [SerializeField] private GameObject optionsPanel = null;
    [SerializeField] private OptionSound musicOption = null;
    [SerializeField] private OptionSound sfxOption = null;
    [SerializeField] private GameObject losePanel = null;
    [SerializeField] private GameObject winPanel = null;

    [SerializeField] private Button retryBtn = null;
    [SerializeField] private Button loseMenuBtn = null;
    [SerializeField] private Button exitBtn = null;
    [SerializeField] private Button nextLevelButton = null;
    [SerializeField] private Button exitOptionsButton = null;

    [Header("Contador de llaves")]
    [SerializeField] private TMP_Text keysText = null;

    public void Init(Action onResume, Action onMenu, Action onRestart, Action onRetry, Action onNextLevel, Action onQuitGame)
    {
        resumeBtn.onClick.AddListener(() =>
        {
            onResume?.Invoke();
            TogglePause(false);
        });
        optionsBtn.onClick.AddListener(() => 
        {
            hudPanel.SetActive(false);
            optionsPanel.SetActive(true);
        });
        restartBtn.onClick.AddListener(onRestart.Invoke);
        menuBtn.onClick.AddListener(onMenu.Invoke);

        retryBtn.onClick.AddListener(onRetry.Invoke);
        loseMenuBtn.onClick.AddListener(onMenu.Invoke);
        nextLevelButton.onClick.AddListener(onNextLevel.Invoke);
        exitOptionsButton.onClick.AddListener(() =>
        {
            hudPanel.SetActive(true);
            optionsPanel.SetActive(false);
        });

        exitBtn.onClick.AddListener(onQuitGame.Invoke);

        AudioManager audioManager = GameManager.Instance.AudioManager;
        musicOption.Init(audioManager.MusicEnabled, audioManager.MusicVolume, audioManager.UpdateMusicVolume, audioManager.ToggleMusic);
        sfxOption.Init(audioManager.SfxEnabled, audioManager.SfxVolume, audioManager.UpdateSfxVolume, audioManager.ToggleSFX);
    }

    public void TogglePause(bool status)
    {
        pausePanel.SetActive(status);
    }

    public void ToggleWave(bool status)
    {
        wavePanel.SetActive(status);
    }

    public void OnUpdateWave(int currentWave, int maxWave)
    {
        waveText.text = "Wave " + currentWave + "/" + maxWave;
    }

    public PlayerUI GetPlayerUI(int index)
    {
        if (index >= 0 && index < players.Length)
        {
            return players[index];
        }

        return null;
    }

    public void OnJoinPlayers(int playersCount)
    {
        for (int i = 0; i < playersCount; i++)
        {
            if (i < players.Length)
            {
                players[i].gameObject.SetActive(true);
            }
        }
    }

    public void OpenLosePanel()
    {
        hudPanel.SetActive(false);
        losePanel.SetActive(true);
    }

    public void OpenWinPanel()
    {
        hudPanel.SetActive(false);
        winPanel.SetActive(true);
    }

    public void UpdateKeysText(int keys)
    {
        keysText.text = "x " + keys;
    }
}

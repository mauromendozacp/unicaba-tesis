using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private PlayerUI[] players = null;

    [SerializeField] private GameObject pausePanel = null;
    [SerializeField] private Button resumeBtn = null;
    [SerializeField] private Button menuBtn = null;

    [SerializeField] private GameObject wavePanel = null;
    [SerializeField] private TMP_Text waveText = null;

    public void Init(Action onResume, Action onMenu)
    {
        resumeBtn.onClick.AddListener(() =>
        {
            onResume?.Invoke();
            TogglePause(false);
        });

        menuBtn.onClick.AddListener(onMenu.Invoke);
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
}

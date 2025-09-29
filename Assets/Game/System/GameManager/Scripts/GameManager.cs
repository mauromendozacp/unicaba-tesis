using System;
using UnityEngine;

public enum SceneGame
{
    Menu,
    Loading,
    Gameplay
}

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [Header("Manager References")]
    [SerializeField] private LoadingManager loadingManager = null;
    [SerializeField] private AudioManager audioManager = null;
    [SerializeField] private GameDataManager gameDataManager = null;

    public LoadingManager LoadingManager => loadingManager;
    public AudioManager AudioManager => audioManager;
    public GameDataManager GameDataManager => gameDataManager;

    public override void Awake()
    {
        if (instance == null)
        {
            loadingManager.LoadingScene(SceneGame.Loading);
            audioManager.Init();
        }

        base.Awake();
    }

    public void ChangeScene(SceneGame nextScene, Action onComplete = null)
    {
        audioManager.StopCurrentMusic();
        loadingManager.TransitionScene(nextScene, onComplete);
    }
}
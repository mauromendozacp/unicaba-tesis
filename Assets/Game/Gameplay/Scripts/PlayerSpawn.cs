using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private Transform[] spawnLocations = null;
    [SerializeField] private CinemachineTargetGroup targetGroup = null;
    [SerializeField] private PlayerInputManager playerInputManager = null;
    [SerializeField] private Sprite[] playerIcons = null;

    private Func<int, PlayerUI> onGetPlayerUI = null;
    private Action onPause = null;
    private Action onDeath = null;
    private Action onCollectKey = null;
    private Action<int> onJoinPlayerUI = null;

    private PlayerData[] playersData = new PlayerData[4];
    private readonly List<PlayerInput> players = new List<PlayerInput>();

    public void Init(Func<int, PlayerUI> onGetPlayerUI, Action onPause, Action onDeath, Action onCollectKey, Action<int> onJoinPlayerUI)
    {
        this.onGetPlayerUI = onGetPlayerUI;
        this.onPause = onPause;
        this.onDeath = onDeath;
        this.onCollectKey = onCollectKey;
        this.onJoinPlayerUI = onJoinPlayerUI;

        List<PlayerSelectionData> selectionPlayers = GameManager.Instance.GameDataManager.playersData;
        for (int i = 0; i < selectionPlayers.Count; i++)
        {
            var data = selectionPlayers[i];
            playersData[i] = data.playerData;
            playerInputManager.JoinPlayer(i, -1, data.controlScheme, data.device);
        }
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        targetGroup.AddMember(playerInput.transform, 1f, 1f);

        int index = players.Count;
        PlayerUI playerUI = onGetPlayerUI.Invoke(index);
        if (playerUI != null)
        {
            PlayerController playerController = playerInput.GetComponent<PlayerController>();
            PlayerData data = playersData[index];

            playerController.Init(playerUI, data, playerIcons[players.Count], onPause, onDeath, onCollectKey);

            Transform playerTransform = spawnLocations[index];
            playerController.transform.SetPositionAndRotation(playerTransform.position, playerTransform.rotation);
        }

        players.Add(playerInput);

        onJoinPlayerUI?.Invoke(players.Count);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        targetGroup.RemoveMember(playerInput.transform);
        players.Remove(playerInput);
    }

    public List<PlayerController> GetPlayers()
    {
        List<PlayerController> playerControllers = new List<PlayerController>();
        for (int i = 0; i < players.Count; i++)
        {
            playerControllers.Add(players[i].GetComponent<PlayerController>());
        }

        return playerControllers;
    }
}

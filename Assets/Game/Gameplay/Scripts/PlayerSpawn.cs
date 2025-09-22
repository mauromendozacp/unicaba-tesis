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

    private Func<int, PlayerUI> onGetPlayerUI = null;
    private Action onPause = null;

    private PlayerData[] playersData = new PlayerData[4];
    private readonly List<PlayerInput> players = new List<PlayerInput>();

    public void Init(Func<int, PlayerUI> onGetPlayerUI, Action onPause)
    {
        this.onGetPlayerUI = onGetPlayerUI;
        this.onPause = onPause;

        List<PlayerSelectionData> selectionPlayers = GameManager.Instance.GameDataManager.playersData;
        for (int i = 0; i < selectionPlayers.Count; i++)
        {
            var data = selectionPlayers[i];
            playerInputManager.JoinPlayer(i, -1, data.controlScheme, data.device);
            playersData[i] = data.playerData;
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

            playerController.Init(playerUI, data, onPause);

            Transform playerTransform = spawnLocations[index];
            playerController.transform.SetPositionAndRotation(playerTransform.position, playerTransform.rotation);
        }

        players.Add(playerInput);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        targetGroup.RemoveMember(playerInput.transform);
        players.Remove(playerInput);
    }

    public List<PlayerInput> GetPlayers()
    {
        return players;
    }
}

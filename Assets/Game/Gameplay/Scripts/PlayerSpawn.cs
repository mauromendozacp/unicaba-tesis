using System;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private Transform[] spawnLocations = null;
    [SerializeField] public CinemachineTargetGroup targetGroup = null;

    private Func<int, PlayerUI> onGetPlayerUI = null;
    private Action onPause = null;

    private readonly List<PlayerInput> players = new List<PlayerInput>();

    public void Init(Func<int, PlayerUI> onGetPlayerUI, Action onPause)
    {
        this.onGetPlayerUI = onGetPlayerUI;
        this.onPause = onPause;
    }

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        targetGroup.AddMember(playerInput.transform, 1f, 1f);

        PlayerUI playerUI = onGetPlayerUI.Invoke(players.Count);
        if (playerUI != null)
        {
            PlayerController playerController = playerInput.GetComponent<PlayerController>();
            playerController.Init(playerUI, onPause);
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

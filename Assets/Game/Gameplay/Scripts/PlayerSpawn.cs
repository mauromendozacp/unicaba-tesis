using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private Transform[] spawnLocations = null;
    [SerializeField] public CinemachineTargetGroup targetGroup = null;
    [SerializeField] private PlayerInputManager playerInputManager = null;

    private readonly List<PlayerInput> players = new List<PlayerInput>();

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        targetGroup.AddMember(playerInput.transform, 1f, 1f);
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

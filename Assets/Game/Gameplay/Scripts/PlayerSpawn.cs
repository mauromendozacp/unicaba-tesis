using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] public CinemachineTargetGroup targetGroup = null;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        targetGroup.AddMember(playerInput.transform, 1f, 1f);
    }

    public void OnPlayerLeft(PlayerInput playerInput)
    {
        targetGroup.RemoveMember(playerInput.transform);
    }
}

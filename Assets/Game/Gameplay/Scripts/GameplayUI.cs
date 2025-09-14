using UnityEngine;

public class GameplayUI : MonoBehaviour
{
    [SerializeField] private PlayerUI[] players = null;

    public PlayerUI GetPlayerUI(int index)
    {
        if (index >= 0 && index < players.Length)
        {
            return players[index];
        }

        return null;
    }
}

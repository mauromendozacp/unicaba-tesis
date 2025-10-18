using UnityEngine;
using UnityEngine.InputSystem;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private JoystickCursor[] cursors;

    public void InitCursor(int index, PlayerInput playerInput)
    {
        cursors[index].Init(playerInput, index);
    }

    public void ToggleCursor(int index, bool status)
    {
        cursors[index].Toggle(status);
    }

    public void ToggleAllCursors(bool status)
    {
        for (int i = 0; i < cursors.Length; i++)
        {
            ToggleCursor(i, status);
        }
    }
}

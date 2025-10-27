using UnityEngine;

public enum PANEL_TYPE
{
    NONE,
    MENU,
    SELECTION,
    OPTIONS,
    TUTORIAL,
    CONTROLS,
    CREDITS
}

public class SectionPanel : MonoBehaviour
{
    [SerializeField] private PANEL_TYPE type = default;

    public PANEL_TYPE Type => type;

    public void Toggle(bool status)
    {
        gameObject.SetActive(status);
    }
}

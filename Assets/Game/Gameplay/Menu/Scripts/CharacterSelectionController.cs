using System;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelectionController : MonoBehaviour
{
    [SerializeField] private PlayerData[] playersData = null;
    [SerializeField] private SlotPlayer[] slots = null;
    [SerializeField] private Button startBtn = null;
    [SerializeField] private Button backBtn = null;

    public Action onStart = null;
    public Action onBack = null;

    public PlayerData[] PlayerDatas => playersData;
    public SlotPlayer[] Slots => slots;

    public void Init(Action onStart, Action onBack)
    {
        this.onStart = onStart;
        this.onStart += ToggleOffInteractions;

        this.onBack = onBack;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].Init(GetCharacterSpriteByIndex, playersData.Length);
        }
    }

    public void InitButtons()
    {
        startBtn.onClick.AddListener(onStart.Invoke);
        backBtn.onClick.AddListener(onBack.Invoke);
    }

    public void Toggle(bool status)
    {
        gameObject.SetActive(status);
    }

    public SlotPlayer GetSlotByIndex(int index)
    {
        return index >= 0 && index < slots.Length ? slots[index] : null;
    }

    private Sprite GetCharacterSpriteByIndex(int index)
    {
        return index >= 0 && index < playersData.Length ? playersData[index].Icon : null;
    }

    private void ToggleOffInteractions()
    {
        backBtn.interactable = false;

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].ToggleChangeCharacters(false);
        }
    }
}

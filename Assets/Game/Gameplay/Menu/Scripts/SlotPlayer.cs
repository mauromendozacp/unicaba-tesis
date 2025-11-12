using System;
using UnityEngine;
using UnityEngine.UI;

public class SlotPlayer : MonoBehaviour
{
    [SerializeField] private Image slotCharacter = null;
    [SerializeField] private Button nextBtn = null;
    [SerializeField] private Button previuosBtn = null;

    private int currentCharacterIndex = 0;
    private int maxCharacters = 0;
    private bool isConfirm = false;

    private Func<int, Sprite> onGetCharacterSpriteByIndex = null;

    public int CurrentCharacterIndex => currentCharacterIndex;
    public bool IsConfirm => isConfirm;

    public void Init(Func<int, Sprite> onGetCharacterSpriteByIndex, int maxCharacters)
    {
        this.onGetCharacterSpriteByIndex = onGetCharacterSpriteByIndex;
        this.maxCharacters = maxCharacters;
    }

    public void InitButtons()
    {
        nextBtn.onClick.AddListener(OnNextCharacter);
        previuosBtn.onClick.AddListener(OnPreviousCharacter);
    }

    public void OnJoinPlayer()
    {
        SetCharacter();
        ToggleChangeCharacters(true);
    }

    public void OnNextCharacter()
    {
        currentCharacterIndex++;
        if (currentCharacterIndex >= maxCharacters)
        {
            currentCharacterIndex = 0;
        }

        SetCharacter();
    }

    public void OnPreviousCharacter()
    {
        currentCharacterIndex--;
        if (currentCharacterIndex < 0)
        {
            currentCharacterIndex = maxCharacters - 1;
        }

        SetCharacter();
    }

    private void SetCharacter()
    {
        Sprite characterSprite = onGetCharacterSpriteByIndex.Invoke(currentCharacterIndex);
        if (characterSprite)
        {
            slotCharacter.sprite = characterSprite;
        }
    }

    public void ToggleChangeCharacters(bool status)
    {
        nextBtn.interactable = status;
        previuosBtn.interactable = status;
    }
}

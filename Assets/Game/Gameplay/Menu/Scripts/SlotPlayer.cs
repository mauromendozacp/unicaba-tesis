using System;
using UnityEngine;
using UnityEngine.UI;

public class SlotPlayer : MonoBehaviour
{
    [SerializeField] private Image slotCharacter = null;
    [SerializeField] private Button nextBtn = null;
    [SerializeField] private Button previuosBtn = null;
    [SerializeField] private Button confirmBtn = null;

    private int currentCharacterIndex = 0;
    private int maxCharacters = 0;
    private bool isConfirm = false;

    private Action onUpdateConfirm = null;
    private Func<int, Sprite> onGetCharacterSpriteByIndex = null;

    public bool IsConfirm => isConfirm;

    public void Init(Action onUpdateConfirm, Func<int, Sprite> onGetCharacterSpriteByIndex)
    {
        this.onUpdateConfirm = onUpdateConfirm;
        this.onGetCharacterSpriteByIndex = onGetCharacterSpriteByIndex;
    }

    public void InitButtons()
    {
        nextBtn.onClick.AddListener(OnNextCharacter);
        previuosBtn.onClick.AddListener(OnPreviousCharacter);
        confirmBtn.onClick.AddListener(OnConfirm);
    }

    public void OnJoinPlayer()
    {
        SetCharacter();
        ToggleChangeCharacters(true);
        confirmBtn.interactable = true;
        onUpdateConfirm?.Invoke();
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

    public void OnConfirm()
    {
        isConfirm = !isConfirm;
        ToggleChangeCharacters(!isConfirm);

        onUpdateConfirm?.Invoke();
    }

    private void SetCharacter()
    {
        Sprite characterSprite = onGetCharacterSpriteByIndex.Invoke(currentCharacterIndex);
        if (characterSprite)
        {
            slotCharacter.sprite = characterSprite;
        }
    }

    private void ToggleChangeCharacters(bool status)
    {
        nextBtn.interactable = status;
        previuosBtn.interactable = status;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private AudioEvent clickSound = null;

    private void Start()
    {
        Button button = GetComponent<Button>();

        button.onClick.AddListener(() =>
        {
            GameManager.Instance.AudioManager.PlayAudio(clickSound);
        });
    }
}

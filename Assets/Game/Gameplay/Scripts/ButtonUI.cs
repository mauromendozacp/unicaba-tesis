using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    [SerializeField] private int playerOwnerIndex = 0;
    [SerializeField] private AudioEvent clickSound = null;

    private Button button = null;

    public Button Button => button;
    public int PlayerOwnerIndex => playerOwnerIndex;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void Start()
    {
        button.onClick.AddListener(() =>
        {
            GameManager.Instance.AudioManager.PlayAudio(clickSound);
        });
    }
}

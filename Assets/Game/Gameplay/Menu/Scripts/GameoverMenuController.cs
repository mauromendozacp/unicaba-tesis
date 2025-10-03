using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameoverMenuController : MonoBehaviour
{
  [SerializeField] private Button RetryBtn = null;
  [SerializeField] private Button MenuBtn = null;


  private void Start()
  {
    RetryBtn.onClick.AddListener(OnRetry);
    MenuBtn.onClick.AddListener(OnMenu);
  }

  void OnRetry()
  {
    GameManager.Instance.ChangeScene(SceneGame.Gameplay);
  }

  void OnMenu()
  {
    GameManager.Instance.ChangeScene(SceneGame.Menu);
  }
}

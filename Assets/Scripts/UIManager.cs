using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
public class UIManager : MonoBehaviour {
  [SerializeField]
  private GameObject pauseUI;
  [SerializeField]
  private GameObject winScreenUI;

  private void OnEnable() {
    GameManager gameManager = GameManager.Instance();
    gameManager.pauseHandler += SetPauseUI;
    gameManager.winGameHandler += SetWinUI;
  }
  private void OnDisable() {
    GameManager gameManager = GameManager.Instance();
    gameManager.pauseHandler -= SetPauseUI;
    gameManager.winGameHandler -= SetWinUI;
  }
  public void SetWinUI(bool active) { winScreenUI.SetActive(active); }
  public void SetPauseUI(bool active) { pauseUI.SetActive(active); }
}
}

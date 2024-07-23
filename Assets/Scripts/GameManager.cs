using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Level;
using UI;

public class GameManager : MonoBehaviour {
  // TODO: abstract the level interface so we don't need game managers for each
  // level
  // TODO: Consider if we should find the level instead of having developers
  // drag and drop it to the inspector
  [SerializeField]
  private FirstLevel firstLevel;
  [SerializeField]
  private UIManager uiManager;
  [SerializeField]
  private bool isPaused;
  private bool didWin = false;

  private void OnEnable() {
    firstLevel.winGameHandler += OnWinGame;
    firstLevel.pauseHandler += OnPauseGame;
  }
  private void OnDisable() {
    firstLevel.winGameHandler -= OnWinGame;
    firstLevel.pauseHandler -= OnPauseGame;
  }

  private void OnWinGame() {
    // TODO: Display UI
    // Likely we will hand it to the UI manager so we can reuse GameManager for
    // other levels
    Debug.Log("[UI Display, GameManager] You Win!");
    if (didWin)
      return;
    didWin = true;
    uiManager.SetWinUI(didWin);
  }

  private void OnPauseGame() {
    if (didWin)
      return;
    isPaused = !isPaused;
    if (isPaused) {
      Time.timeScale = 0;
    } else {
      Time.timeScale = 1;
    }
    uiManager.SetPauseUI(isPaused);
  }
}

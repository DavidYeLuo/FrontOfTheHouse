using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Level;
using UI;

public delegate void BoolHandler(bool value);
public class GameManager : MonoBehaviour {
  private static GameManager instance;
  public static GameManager Instance() { return instance; } // Singleton Pattern
  public event BoolHandler winGameHandler;
  public event BoolHandler pauseHandler;
  [SerializeField]
  private bool isPaused;
  private bool didWin = false;

  private void Awake() {
    if (instance == null) {
      instance = this;
    } else {
      Destroy(this);
    }
  }

  public bool IsPaused() { return isPaused; }
  public bool DidWin() { return didWin; }

  public void WinGame() {
    Debug.Log("[UI Display, GameManager] You Win!");
    if (didWin)
      return;
    didWin = true;
    // uiManager.SetWinUI(didWin);
    winGameHandler?.Invoke(didWin);
  }

  public void TogglePause() {
    if (didWin)
      return;
    isPaused = !isPaused;
    if (isPaused) {
      Time.timeScale = 0;
    } else {
      Time.timeScale = 1;
    }
    // uiManager.SetPauseUI(isPaused);
    pauseHandler?.Invoke(isPaused);
  }
}

using UnityEngine;
using UnityEngine.SceneManagement;

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
      Debug.Log("Game Manager already Exist, destroying self.");
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
    winGameHandler?.Invoke(didWin);
  }

  public void SetPause(bool isPaused) {
    if (didWin)
      return;
    if (isPaused) {
      Time.timeScale = 0;
    } else {
      Time.timeScale = 1;
    }
    pauseHandler?.Invoke(isPaused);
  }
  public void TogglePause() {
    isPaused = !isPaused;
    SetPause(isPaused);
  }

  public void TransitionScene(string scene) { SceneManager.LoadScene(scene); }
}

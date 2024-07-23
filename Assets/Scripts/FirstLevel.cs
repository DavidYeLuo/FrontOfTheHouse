using UnityEngine;
using Interactable;

namespace Level {
public delegate void WinGameHandler();
public class FirstLevel : MonoBehaviour {
  public event WinGameHandler winGameHandler;
  public event Player.PauseHandler pauseHandler;
  [SerializeField]
  private Player.Player player;
  [SerializeField]
  private bool didWin = false; // Prevents winning multiple times
  [SerializeField]
  private UtensilBox utensilBox;

  public bool didPlayerWin() { return didWin; }

  private void WinWhenUtensilBoxIsSorted(bool isSorted) {
    if (!isSorted)
      return;
    Debug.Log("[Event invoke, FirstLevel] UtensilBox sorted! You Win!");
    didWin = true;
    winGameHandler?.Invoke();
  }

  // TODO: Might consider improving the system
  // Currently the player emits the event for the FirstLevel,
  // then the FirstLevel emits the event for the GameManager
  private void Pause() { pauseHandler?.Invoke(); }

  private void OnEnable() {
    utensilBox.isUtensilBoxSortedHandler += WinWhenUtensilBoxIsSorted;
    player.pauseHandler += Pause;
  }

  private void OnDisable() {
    utensilBox.isUtensilBoxSortedHandler -= WinWhenUtensilBoxIsSorted;
    player.pauseHandler -= Pause;
  }
}
}

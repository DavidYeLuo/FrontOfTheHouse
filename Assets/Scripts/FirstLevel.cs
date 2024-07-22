using UnityEngine;
using Interactable;

namespace Level {
public delegate void WinGameHandler();
public class FirstLevel : MonoBehaviour {
  public event WinGameHandler winGameHandler;
  [SerializeField]
  private bool didWin = false; // Prevents winning multiple times
  [SerializeField]
  private UtensilBox utensilBox;

  public bool didPlayerWin() { return didWin; }

  private void WinWhenUtensilBoxIsSorted(bool isSorted) {
    // didWin prevents winning multiple times
    if (didWin || !isSorted)
      return;
    Debug.Log("[Event invoke, FirstLevel] UtensilBox sorted! You Win!");
    didWin = true;
    winGameHandler?.Invoke();
  }

  private void OnEnable() {
    utensilBox.isUtensilBoxSortedHandler += WinWhenUtensilBoxIsSorted;
  }

  private void OnDisable() {
    utensilBox.isUtensilBoxSortedHandler -= WinWhenUtensilBoxIsSorted;
  }
}
}

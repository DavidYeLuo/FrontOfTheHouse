using UnityEngine;
using Interactable;

namespace Level {
public class FirstLevel : MonoBehaviour {
  [SerializeField]
  private Player.Player player;
  [SerializeField]
  private UtensilBox utensilBox;
  private GameManager gameManager;

  private void Start() { gameManager = GameManager.Instance(); }

  private void WinWhenUtensilBoxIsSorted(bool isSorted) {
    if (!isSorted)
      return;
    Debug.Log("[Event invoke, FirstLevel] UtensilBox sorted! You Win!");
    gameManager.WinGame();
  }

  private void Pause() { gameManager.TogglePause(); }

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

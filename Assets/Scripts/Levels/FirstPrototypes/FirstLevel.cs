using UnityEngine;
using Interactable;
using System.Collections;

namespace Level {
public class FirstLevel : MonoBehaviour {
  [SerializeField]
  private Level levelHelper;
  [SerializeField]
  private UtensilBox utensilBox;

  private void Awake() { levelHelper.Init(); }
  private void Start() {
    StartCoroutine(levelHelper.ZoomInToPlayerTransition(
        Level.DEFAULT_CAMERA_OFFSET, Level.DEFAULT_TRANSITION_TIME));
  }

  private void WinWhenUtensilBoxIsSorted(bool isSorted) {
    if (!isSorted)
      return;
    Debug.Log("[Event invoke, FirstLevel] UtensilBox sorted! You Win!");
    levelHelper.gameManager.WinGame();
  }

  private void OnEnable() {
    utensilBox.isUtensilBoxSortedHandler += WinWhenUtensilBoxIsSorted;
    levelHelper.player.pauseHandler += levelHelper.gameManager.TogglePause;
  }

  private void OnDisable() {
    utensilBox.isUtensilBoxSortedHandler -= WinWhenUtensilBoxIsSorted;
    levelHelper.player.pauseHandler -= levelHelper.gameManager.TogglePause;
  }
}
}

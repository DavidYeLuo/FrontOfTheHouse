using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Level;

public class GameManager : MonoBehaviour {
  // TODO: abstract the level interface so we don't need game managers for each
  // level
  // TODO: Consider if we should find the level instead of having developers
  // drag and drop it to the inspector
  [SerializeField]
  private FirstLevel firstLevel;

  private void OnEnable() { firstLevel.winGameHandler += OnWinGame; }
  private void OnDisable() { firstLevel.winGameHandler -= OnWinGame; }

  private void OnWinGame() {
    // TODO: Display UI
    // Likely we will hand it to the UI manager so we can reuse GameManager for
    // other levels
    Debug.Log("[UI Display, GameManager] You Win!");
  }
}

using UnityEngine;
using Interactable;
using System.Collections;

namespace Level {
public class LevelNewHire : MonoBehaviour {

  [SerializeField]
  private Level levelHelper;
  [SerializeField]
  private SpeedRack speedrack;

  private void Awake() { levelHelper.Init(); }
  private void Start() {
    StartCoroutine(levelHelper.ZoomInToPlayerTransition(
        Level.DEFAULT_CAMERA_OFFSET, Level.DEFAULT_TRANSITION_TIME));
  }
  private void WinWhenSpeedRackHasMuffinTrays(ItemHolderState state) {
    if (state != ItemHolderState.FILLED)
      return;

    var food = speedrack.PeekTopItem();
    if (food != null && food is FoodTray &&
        food.GetState() == ItemHolderState.FILLED)
      levelHelper.gameManager.WinGame();
    else
      Debug.Log("TODO: Empty foodtray not implemented.");
  }
  private void OnEnable() {
    speedrack.foodHolderHandler += WinWhenSpeedRackHasMuffinTrays;
    foreach (Player.Player player in levelHelper.players) {
      player.pauseHandler += levelHelper.gameManager.TogglePause;
    }
  }

  private void OnDisable() {
    speedrack.foodHolderHandler -= WinWhenSpeedRackHasMuffinTrays;
    foreach (Player.Player player in levelHelper.players) {
      player.pauseHandler -= levelHelper.gameManager.TogglePause;
    }
  }
}
}

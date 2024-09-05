using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Food;
using Interactable;

namespace Level {
public class SecondLevel : MonoBehaviour {
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
    levelHelper.player.pauseHandler += levelHelper.gameManager.TogglePause;
  }

  private void OnDisable() {
    speedrack.foodHolderHandler -= WinWhenSpeedRackHasMuffinTrays;
    levelHelper.player.pauseHandler -= levelHelper.gameManager.TogglePause;
  }
}
}

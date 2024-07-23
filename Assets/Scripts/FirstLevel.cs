using UnityEngine;
using Interactable;
using System.Collections;

namespace Level {
public class FirstLevel : MonoBehaviour {
  [Header("Camera/Scenematics")]
  [SerializeField]
  private float cameraOffset;
  [SerializeField]
  private float transitionTime;

  [Header("Dependencies")]
  [SerializeField]
  private Player.Player player;
  [SerializeField]
  private UtensilBox utensilBox;

  private Camera cam;
  private GameManager gameManager;

  // TODO: Instead of the level handling the animation cutscene, let's offload
  // it to another gameobject
  private IEnumerator Start() {
    gameManager = GameManager.Instance();
    cam = Camera.main;

    Vector3 temp =
        cam.transform
            .position; // For returning the camera where it needs to be in case

    cam.transform.position -= cameraOffset * cam.transform.forward;

    // Start Cutscene
    // BUG: when disabling playerscript, it prevents the user to pause in mid
    // cutscene
    player.enabled = false; // Disables input while in cutscene
    const float INTERVAL = 60;
    float wait = transitionTime / INTERVAL;
    float step = cameraOffset / INTERVAL;
    // BUG: Waiting for a second is too long
    // We should find a different way to animate because the only time that is
    // smooth is when wait = 0
    // TODO: Research for custom lerp implementation
    for (int i = 0; i < INTERVAL; i++) {
      cam.transform.position += step * cam.transform.forward;
      yield return new WaitForSeconds(wait);
    } // End of Cutscene
    cam.transform.position = temp; // Return the camera where it original was in
                                   // case it gets lost after the animation.
                                   // Might not be needed.
    player.enabled = true;         // Enables input while in cutscene

    yield return null;
  }

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

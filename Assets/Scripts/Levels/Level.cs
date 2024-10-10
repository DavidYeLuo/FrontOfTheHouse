using UnityEngine;
using Interactable;
using System.Collections;
using System;
using UnityEngine.Assertions;
using System.Collections.Generic;
using UI;
using CustomTimer;

namespace Level {
// When using this class, we should Init
[Serializable]
public class Level {
  // Input for SetLockPlayerInput
  public const bool INPUT_LOCK_ON = true;
  public const bool INPUT_LOCK_OFF = false;
  // Default Zoom Options
  public const float DEFAULT_CAMERA_OFFSET = 10.0f;
  public const float DEFAULT_TRANSITION_TIME = 0.5f;

  [Header("Dependencies")]
  [SerializeField]
  public List<Player.Player> players;
  public TimelineAPI timelineAPI;

  public Camera cam;
  public GameManager gameManager;

  // Default Init
  public void Init() {
    gameManager = GameManager.Instance();
    cam = Camera.main;
    Assert.IsNotNull(players);
    Assert.IsNotNull(cam);
    Assert.IsNotNull(gameManager);
  }

  public void AdjustTimeline(Timer timer, float secondsBeforeBreakfast,
                             float secondsBeforeLunch,
                             float secondsBeforeEndOfService) {
    timelineAPI.TimelineTimer = timer;
    timelineAPI.TimelineDisplay.Adjust(
        secondsBeforeBreakfast, secondsBeforeLunch, secondsBeforeEndOfService);
  }

  public void SetLockPlayerInput(bool isLocked) {
    foreach (Player.Player player in players) {
      player.enabled = !isLocked; // Enables/Disables obj
    }
  }

  private IEnumerator SetLockPlayerInputCoroutine(bool isLocked) {
    SetLockPlayerInput(isLocked);
    yield return null;
  }

  public IEnumerator ZoomInToPlayerTransition(float cameraOffset,
                                              float transitionTime) {
    Vector3 temp =
        cam.transform
            .position; // For returning the camera where it needs to be in case

    cam.transform.position -= cameraOffset * cam.transform.forward;

    // Start Cutscene
    // BUG: when disabling playerscript, it prevents the user to pause in mid
    // cutscene
    // player.enabled = false; // Disables input while in cutscene
    yield return SetLockPlayerInputCoroutine(
        true); // Learned that calling a regular method in coroutine doesn't
               // work properly
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
    // player.enabled = true;         // Disables input while in cutscene
    yield return SetLockPlayerInputCoroutine(false);

    yield return null;
  }

  public struct TimeDiffFreq {
    public List<int> freqList;
    public List<int> timeDiffList;
  }
  // Creates a list containing the number of spawn after a certain time frame
  public TimeDiffFreq GetDiscreteSampleBaseOnAnimCurve(int numOfSamples,
                                                       AnimationCurve curve) {
    List<int> freqList = new List<int>();
    List<int> timeDiffList = new List<int>();
    TimeDiffFreq res = new TimeDiffFreq();
    res.freqList = freqList;
    res.timeDiffList = timeDiffList;

    int spawnCount = 0;
    int prevCount = 0;

    int timeFrame = 0;
    int prevFrame = 0;
    for (int i = 0; i <= 100; i++) {
      prevCount = spawnCount;
      spawnCount = (int)(curve.Evaluate(0.01f * i) * numOfSamples);
      int difference = spawnCount - prevCount;
      timeFrame++;
      if (difference == 0)
        continue;
      freqList.Add(difference);

      timeDiffList.Add(timeFrame - prevFrame);
      prevFrame = timeFrame;
    }
    return res;
  }
}
}

using UnityEngine;
using Interactable;
using System.Collections;
using System.Collections.Generic;
using NPC;
using PlayerAction;
using ObjectPool;
using CustomTimer;
using UnityEngine.UI;
using UI;
using Level;

namespace LevelTest {
public class LevelWaveSpawnTester : MonoBehaviour {
  [Header("Settings")]
  // TODO: Use a list of the new scriptable object instead see the
  // TimelineSizeAdjuster class
  // NOTE: Might be more useful if we extend the scriptable object to also
  // accomodate with the spawner We can try using an interface for this added
  // functionality
  [SerializeField]
  private float secondsBeforeBreakfast;
  [SerializeField]
  private float secondsBeforeLunch;
  [SerializeField]
  private float secondsBeforeEndOfService;

  // TODO: Add number of spawns for each service
  [SerializeField]
  private AnimationCurve breakfastSpawnFreq;
  [SerializeField]
  private AnimationCurve lunchSpawnFreq;

  [Header("Dependencies")]

  [SerializeField]
  private Level.Level levelHelper;
  public Timer timelineTimer;

  [Space]

  [SerializeField]
  private Guest guestPrefab;
  [SerializeField]
  private Vector3 spawnPoint;
  [SerializeField]
  private List<GameObject> objectOfInterests;
  [SerializeField]
  private List<Elevator> exits;

  public int maxGuestPoolSize = 1;
  Pooler guestPooler;

  private bool areGuestsSatisfied = false;
  private WaitForSeconds waitUntilBreakfast;
  private WaitForSeconds waitUntilLunch;
  private WaitForSeconds waitUntilServiceEnd;

  private void Awake() {
    levelHelper.Init();
    levelHelper.AdjustTimeline(timelineTimer, secondsBeforeBreakfast,
                               secondsBeforeLunch, secondsBeforeEndOfService);
  }
  private IEnumerator Start() {
    StartCoroutine(levelHelper.ZoomInToPlayerTransition(
        Level.Level.DEFAULT_CAMERA_OFFSET,
        Level.Level.DEFAULT_TRANSITION_TIME));
    guestPooler = new Pooler(maxGuestPoolSize, guestPrefab);

    waitUntilBreakfast = new WaitForSeconds(secondsBeforeBreakfast);
    waitUntilLunch = new WaitForSeconds(secondsBeforeLunch);
    waitUntilServiceEnd = new WaitForSeconds(secondsBeforeEndOfService);

    levelHelper.GetDiscreteSampleBaseOnAnimCurve(10, breakfastSpawnFreq);

    timelineTimer.WaitForSeconds(secondsBeforeBreakfast + secondsBeforeLunch +
                                 secondsBeforeEndOfService);
    List<Guest> spawnedGuests = new List<Guest>();
    TimeDiffFreq timeDiffFreq =
        levelHelper.GetDiscreteSampleBaseOnAnimCurve(10, breakfastSpawnFreq);
    Guest currentSpawnedGuest;
    int numGuestsWhoLeft = 0;
    void incrementCounter(Guest _guest) { numGuestsWhoLeft++; }
    for (int i = 0; i < maxGuestPoolSize; i++) {
      currentSpawnedGuest = SpawnGuest();
      spawnedGuests.Add(currentSpawnedGuest);
      currentSpawnedGuest.OnGuestLeave += incrementCounter;
      currentSpawnedGuest.gameObject.SetActive(false);
    }
    int numSpawns = 0;
    float FRAC = 1 / 100.0f;
    for (int i = 0; i < timeDiffFreq.freqList.Count; i++) {
      Debug.Log(timeDiffFreq.timeDiffList[i]);
      yield return new WaitForSeconds(timeDiffFreq.timeDiffList[i] * FRAC *
                                      secondsBeforeBreakfast);
      for (int freq = 0; freq < timeDiffFreq.freqList[i]; freq++) {
        spawnedGuests[numSpawns].gameObject.SetActive(true);
        numSpawns++;
      }
    }

    // Spawns for the lunch
    timeDiffFreq =
        levelHelper.GetDiscreteSampleBaseOnAnimCurve(10, lunchSpawnFreq);
    for (int i = 0; i < timeDiffFreq.freqList.Count; i++) {
      Debug.Log(timeDiffFreq.timeDiffList[i]);
      yield return new WaitForSeconds(timeDiffFreq.timeDiffList[i] * FRAC *
                                      secondsBeforeBreakfast);
      for (int freq = 0; freq < timeDiffFreq.freqList[i]; freq++) {
        spawnedGuests[numSpawns].gameObject.SetActive(true);
        numSpawns++;
      }
    }

    // yield return waitUntilBreakfast;
    // currentSpawnedGuest = SpawnGuest();
    // currentSpawnedGuest.OnGuestLeave += incrementCounter;
    // spawnedGuests.Add(currentSpawnedGuest);
    // yield return waitUntilLunch;
    // currentSpawnedGuest = SpawnGuest();
    // currentSpawnedGuest.OnGuestLeave += incrementCounter;
    // spawnedGuests.Add(currentSpawnedGuest);
    // yield return waitUntilServiceEnd;
    // areGuestsSatisfied = numGuestsWhoLeft == 2 ? true : false;
    if (areGuestsSatisfied)
      Debug.Log("You Win!");
    else
      Debug.Log("You Lose!");
    spawnedGuests.ForEach((_guest) => _guest.OnGuestLeave -= incrementCounter);
    yield return null;
  }
  private Guest SpawnGuest() {
    Debug.Log("Spawning Guest");
    Guest firstGuest = guestPooler.Spawn() as Guest;
    firstGuest.gameObject.transform.position = spawnPoint;
    firstGuest.gameObject.transform.rotation = Quaternion.identity;

    firstGuest.Init(GuestGoal.EXPLORE, objectOfInterests, exits);
    return firstGuest;
  }
  private void OnEnable() {
    foreach (Player.Player player in levelHelper.players) {
      player.pauseHandler += levelHelper.gameManager.TogglePause;
    }
  }

  private void OnDisable() {
    foreach (Player.Player player in levelHelper.players) {
      player.pauseHandler -= levelHelper.gameManager.TogglePause;
    }
  }
}
}

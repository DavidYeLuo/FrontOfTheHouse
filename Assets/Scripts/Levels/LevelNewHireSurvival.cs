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

namespace Level {
public class LevelNewHireSurvival : MonoBehaviour {
  [Header("Settings")]
  [SerializeField]
  private float secondsBeforeBreakfast;
  [SerializeField]
  private float secondsBeforeLunch;
  [SerializeField]
  private float secondsBeforeEndOfService;

  [Header("Dependencies")]

  [SerializeField]
  private Level levelHelper;
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
        Level.DEFAULT_CAMERA_OFFSET, Level.DEFAULT_TRANSITION_TIME));
    guestPooler = new Pooler(maxGuestPoolSize, guestPrefab);
    // Guest guest = Instantiate(guestPrefab, spawnPoint, Quaternion.identity);

    waitUntilBreakfast = new WaitForSeconds(secondsBeforeBreakfast);
    waitUntilLunch = new WaitForSeconds(secondsBeforeLunch);
    waitUntilServiceEnd = new WaitForSeconds(secondsBeforeEndOfService);

    timelineTimer.WaitForSeconds(secondsBeforeBreakfast + secondsBeforeLunch +
                                 secondsBeforeEndOfService);
    List<Guest> spawnedGuests = new List<Guest>();
    Guest currentSpawnedGuest;
    int numGuestsWhoLeft = 0;
    void incrementCounter(Guest _guest) { numGuestsWhoLeft++; }
    yield return waitUntilBreakfast;
    currentSpawnedGuest = SpawnGuest();
    currentSpawnedGuest.OnGuestLeave += incrementCounter;
    spawnedGuests.Add(currentSpawnedGuest);
    yield return waitUntilLunch;
    currentSpawnedGuest = SpawnGuest();
    currentSpawnedGuest.OnGuestLeave += incrementCounter;
    spawnedGuests.Add(currentSpawnedGuest);
    yield return waitUntilServiceEnd;
    areGuestsSatisfied = numGuestsWhoLeft == 2 ? true : false;
    if (areGuestsSatisfied)
      Debug.Log("You Win!");
    else
      Debug.Log("You Lose!");
    spawnedGuests.ForEach((_guest) => _guest.OnGuestLeave -= incrementCounter);
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

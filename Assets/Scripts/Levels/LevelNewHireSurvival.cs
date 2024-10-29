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
  [Header("Prefabs")]
  [SerializeField]
  private WaveManager waveManager;
  private WaveManager _waveManager;

  private void Awake() { levelHelper.Init(); }
  private IEnumerator Start() {
    StartCoroutine(levelHelper.ZoomInToPlayerTransition(
        Level.DEFAULT_CAMERA_OFFSET, Level.DEFAULT_TRANSITION_TIME));
    guestPooler = new Pooler(maxGuestPoolSize, guestPrefab);
    List<Guest> spawnedGuests = new List<Guest>();
    Guest currentSpawnedGuest;
    _waveManager = Instantiate(waveManager);
    _waveManager.onWaveBeat += SpawnNGuest;
    // StartCoroutine(_waveManager.Begin());
    _waveManager.Begin();
    return null;
  }
  private Guest SpawnGuest() {
    Debug.Log("Spawning Guest");
    Guest firstGuest = guestPooler.Spawn() as Guest;
    firstGuest.gameObject.transform.position = spawnPoint;
    firstGuest.gameObject.transform.rotation = Quaternion.identity;

    firstGuest.Init(GuestGoal.EXPLORE, objectOfInterests, exits);
    return firstGuest;
  }
  private void SpawnNGuest(int n) {
    for (int i = 0; i < n; i++) {
      SpawnGuest();
    }
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

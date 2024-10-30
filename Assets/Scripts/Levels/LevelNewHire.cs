using UnityEngine;
using Interactable;
using System.Collections;
using System.Collections.Generic;
using NPC;
using PlayerAction;
using ObjectPool;
using CustomTimer;

namespace Level {
public class LevelNewHire : MonoBehaviour {

  [SerializeField]
  private Level levelHelper;
  [SerializeField]
  private SpeedRack speedrack;

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

  Timer guestSpawnTimer;

  [SerializeField]
  private WaveManager waveManager;
  private WaveManager _waveManager;

  private void Awake() {
    levelHelper.Init();
    guestSpawnTimer = gameObject.AddComponent<Timer>();
  }
  private IEnumerator Start() {
    StartCoroutine(levelHelper.ZoomInToPlayerTransition(
        Level.DEFAULT_CAMERA_OFFSET, Level.DEFAULT_TRANSITION_TIME));
    _waveManager = Instantiate(waveManager);
    guestPooler = new Pooler(10, guestPrefab);
    void SpawnNGuest(int n) {
      Debug.Log(n);
      for (int i = 0; i < n; i++)
        SpawnGuest();
    }
    _waveManager.onWaveBeat += SpawnNGuest;
    _waveManager.Begin();

    return null;
  }

  private void SpawnGuest() {
    Debug.Log("Spawning Guest");
    Guest firstGuest = guestPooler.Spawn() as Guest;
    firstGuest.gameObject.transform.position = spawnPoint;
    firstGuest.gameObject.transform.rotation = Quaternion.identity;

    firstGuest.Init(GuestGoal.EXPLORE, objectOfInterests, exits);
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

    // guestSpawnTimer.timeUpEvent += SpawnGuest;
  }

  private void OnDisable() {
    speedrack.foodHolderHandler -= WinWhenSpeedRackHasMuffinTrays;
    foreach (Player.Player player in levelHelper.players) {
      player.pauseHandler -= levelHelper.gameManager.TogglePause;
    }

    // guestSpawnTimer.timeUpEvent -= SpawnGuest;
  }
}
}

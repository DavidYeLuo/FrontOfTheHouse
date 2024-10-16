using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CustomTimer {
/// This class represents the visual of a timer.
public class TimerVisual : MonoBehaviour {
  public event TimeUpEvent timeUpEvent;
  [field:SerializeField]
  public float CurrentTime { get; private set; }
  public float PercentTime {
    get { return CurrentTime / stopTime; }
  }

  [Header("Dependencies")]
  public Timer timer;
  public Image image;

  private float stopTime;

  private bool isStarted;

  private void Awake() {
    CurrentTime = 0.0f;
    stopTime = 1.0f;
    isStarted = false;
  }

  private void OnEnable() { timer.timeStartEvent += StartTimer; }
  private void OnDisable() { timer.timeStartEvent -= StartTimer; }
  public void StartTimer(float durationInSeconds) {
    Debug.Log("TimerVisual has started!");
    CurrentTime = 0;
    isStarted = true;
    stopTime = durationInSeconds;
  }
  private void Update() {
    if (!isStarted)
      return;
    CurrentTime += Time.deltaTime;
    image.fillAmount = PercentTime;

    if (isStarted && CurrentTime > stopTime) {
      isStarted = false;
      this.timeUpEvent?.Invoke();
    }
  }
}
}

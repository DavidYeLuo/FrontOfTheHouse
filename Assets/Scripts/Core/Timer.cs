using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CustomTimer {
public delegate void TimeUpEvent();
public delegate void TimeStartEvent(float durationInSeconds);
public class Timer : MonoBehaviour {
  public event TimeUpEvent timeUpEvent;
  public event TimeStartEvent timeStartEvent;

  private Timing timing;
  private WorkingTimer workingTimer;
  private static NullTiming nullTiming;

  public Timer() {
    workingTimer = new WorkingTimer(this);
    if (nullTiming == null)
      nullTiming = new NullTiming();
    timing = nullTiming;

    // Internal clock
    timeUpEvent += OnTimeUpSwitchToNullTiming;
  }

  public void WaitForSeconds(float seconds) {
    workingTimer.SetTimer(seconds);
    timing = workingTimer;
    timeStartEvent?.Invoke(seconds);
  }

  // NOTE: switch to nulltiming AND invokes the timeUpEvent
  private void OnTimeUpSwitchToNullTiming() {
    Debug.Log("Nulltiming");
    timing = nullTiming;
  }
  private void FixedUpdate() { timing.Tick(Time.deltaTime); }

  [Serializable]
  private abstract class Timing {
    public abstract void Tick(float deltaTime);
  }
  [Serializable]
  private class NullTiming : Timing {
    public override void Tick(float deltaTime) {}
  }
  [Serializable]
  private class WorkingTimer : Timing {
    private Timer timer;
    private float targetTime = 0.0f;

    public WorkingTimer(Timer timer) { this.timer = timer; }

    public void SetTimer(float targetTime) { this.targetTime = targetTime; }
    public override void Tick(float deltaTime) {
      if (targetTime < 0.0f) {
        timer.timeUpEvent?.Invoke();
      }
      targetTime -= deltaTime;
    }
  }
}
}

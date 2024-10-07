using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using CustomTimer;
using UnityEngine.Assertions;

namespace UI {
public class TimelineAPI : MonoBehaviour {
  [SerializeField]
  private TimerVisual timerVisual;
  public Timer TimelineTimer {
    get { return timerVisual.timer; }
    set { timerVisual.timer = value; }
  }
  [field:SerializeField]
  public TimelineSizeAdjuster TimelineDisplay {
    get; private set;
  }

  private void Start() {
    Assert.IsNotNull(timerVisual);
    Assert.IsNotNull(TimelineDisplay);
  }
}
}

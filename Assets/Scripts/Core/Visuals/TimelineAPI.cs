using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;
using CustomTimer;
using UnityEngine.Assertions;

namespace UI {
/// This class is a class that allows modification toward its subcomponents
/// NOTE: Currently doesn't follow OOP but can be changed to a facade pattern
/// Not sure if it is a good idea to switch to facade since there isn't much
/// complexity yet
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

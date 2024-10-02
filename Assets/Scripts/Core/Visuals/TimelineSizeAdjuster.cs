using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UI {
public class TimelineSizeAdjuster : MonoBehaviour {
  [SerializeField]
  private bool selfInit = true;
  [Space]
  [Header("Only if selfInit is true")]
  public float secondsBeforeBreakfast;
  public float secondsBeforeLunch;
  public float secondsBeforeEndOfService;

  [Space]
  [Header("Dependencies")]
  public Image timeline;
  public Image preBreakfastTimeline;
  public Image preLunchTimeline;
  public Image preEndOfServiceTimeline;

  public void Adjust() {
    float baseHeight = timeline.rectTransform.sizeDelta.y;
    float baseWidth = timeline.rectTransform.sizeDelta.x;
    float totalTime =
        secondsBeforeBreakfast + secondsBeforeLunch + secondsBeforeEndOfService;
    float preBreakfastRatio = secondsBeforeBreakfast / totalTime;
    float preLunchRatio = secondsBeforeLunch / totalTime;
    float preEndOfServiceRatio = secondsBeforeEndOfService / totalTime;

    preBreakfastTimeline.rectTransform.sizeDelta =
        new Vector2(preBreakfastRatio * baseWidth, baseHeight);
    preLunchTimeline.rectTransform.sizeDelta =
        new Vector2(preLunchRatio * baseWidth, baseHeight);

    preEndOfServiceTimeline.rectTransform.sizeDelta =
        new Vector2(preEndOfServiceRatio * baseWidth, baseHeight);
  }

  public void Adjust(float secondsBeforeBreakfast, float secondsBeforeLunch,
                     float secondsBeforeEndOfService) {
    float baseHeight = timeline.rectTransform.sizeDelta.y;
    float baseWidth = timeline.rectTransform.sizeDelta.x;
    float totalTime =
        secondsBeforeBreakfast + secondsBeforeLunch + secondsBeforeEndOfService;
    float preBreakfastRatio = secondsBeforeBreakfast / totalTime;
    float preLunchRatio = secondsBeforeLunch / totalTime;
    float preEndOfServiceRatio = secondsBeforeEndOfService / totalTime;

    preBreakfastTimeline.rectTransform.sizeDelta =
        new Vector2(preBreakfastRatio * baseWidth, baseHeight);
    preLunchTimeline.rectTransform.sizeDelta =
        new Vector2(preLunchRatio * baseWidth, baseHeight);

    preEndOfServiceTimeline.rectTransform.sizeDelta =
        new Vector2(preEndOfServiceRatio * baseWidth, baseHeight);
  }

  private void Start() {
    if (selfInit)
      Adjust();
  }
}
}

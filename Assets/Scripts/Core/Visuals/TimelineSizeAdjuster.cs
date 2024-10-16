using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace UI {
/// This class adjusts the image proportion to stages of the game service
/// TODO: In the future, it will be dynamically adjusted for easier modification
public class TimelineSizeAdjuster : MonoBehaviour {
  [SerializeField]
  private bool selfInit = true;
  // TODO: Refactor to using list instead
  [Space]
  [Header("Only if selfInit is true")]
  // TODO: We can use a ScriptableObject that holds seconds(float) and
  // color(Color)
  // NOTE: Because we are going to have additional data to that gameboject, we
  // should interact with it via an interface
  public float secondsBeforeBreakfast;
  public float secondsBeforeLunch;
  public float secondsBeforeEndOfService;

  [Space]
  [Header("Dependencies")]
  public Image timeline;
  // TODO: Use a single image and spawn as needed and modify it in start
  public Image preBreakfastTimeline;
  public Image preLunchTimeline;
  public Image preEndOfServiceTimeline;

  // TODO: We the vlayout gameobject that we can add Images UI to it

  // TODO: Refactor this with using a list instead
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

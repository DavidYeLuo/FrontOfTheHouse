using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

namespace UI {
public delegate void FinishProgressHandler();
public delegate void BeginProgressBarHandler(float targetSeconds);
public delegate void CancelProgressBarHandler();
public class ProgressBar : MonoBehaviour {
  private static List<ProgressBar> instances = new List<ProgressBar>();
  public event FinishProgressHandler finishProgressBarHandler;
  public event BeginProgressBarHandler beginProgressBarHandler;
  public event CancelProgressBarHandler cancelProgressBarHandler;

  private float targetSeconds;
  private float progressSeconds;

  [MenuItem("UI/ProgressBarUI/CancelAllTasks")]
  public static void CancelAllTasks() {
    Debug.Log("Cancelling all tasks");
    foreach (ProgressBar item in instances) {
      item.CancelTask();
    }
  }
  public void BeginTask(float targetSeconds) {
    this.targetSeconds = targetSeconds;
    beginProgressBarHandler?.Invoke(targetSeconds);
    progressSeconds = 0;
  }
  public void CancelTask() {
    Debug.Log("Task cancelled");
    finishProgressBarHandler = null;
    cancelProgressBarHandler?.Invoke();
  }
  private void Start() { instances.Add(this); }
  private void Update() {
    if (progressSeconds > targetSeconds) {
      finishProgressBarHandler?.Invoke();
      finishProgressBarHandler = null;
    }
    progressSeconds += Time.deltaTime;
  }
}

}

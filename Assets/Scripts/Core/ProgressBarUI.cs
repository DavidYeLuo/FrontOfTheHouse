using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;

namespace UI {
public delegate void FinishProgressHandler();
// This class can be split into two:
// A progress backend and a UI frontend
public class ProgressBarUI : MonoBehaviour {
  private static List<ProgressBarUI> instances = new List<ProgressBarUI>();
  public event FinishProgressHandler finishProgressBarHandler;
  private Player.Player player;

  [Header("Dependencies")]
  [SerializeField]
  private Image
      progressBar; // NOTE: turning this off will leave the background intact so
                   // we should turn off a group of gameobject instead
  [SerializeField]
  private GameObject components; // GameObject that represents the progress bar
                                 // TODO: use a game object instead

  private float targetSeconds;
  private float progressSeconds;

  [MenuItem("UI/ProgressBarUI/CancelAllTasks")]
  public static void CancelAllTasks() {
    Debug.Log("Cancelling all tasks");
    foreach (ProgressBarUI item in instances) {
      item.CancelTask();
    }
  }
  public void BeginTask(GameObject player, float targetSeconds) {
    this.targetSeconds = targetSeconds;
    progressSeconds = 0;
    components.gameObject.SetActive(true);
  }
  public void CancelTask() {
    Debug.Log("Task cancelled");
    components.gameObject.SetActive(false);
    finishProgressBarHandler = null;
  }
  private void Start() { instances.Add(this); }
  private void Update() {
    if (progressSeconds > targetSeconds) {
      progressBar.fillAmount = 1.0f;
      finishProgressBarHandler?.Invoke();
      finishProgressBarHandler = null;
      components.gameObject.SetActive(false);
    }
    progressBar.fillAmount = progressSeconds / targetSeconds;
    progressSeconds += Time.deltaTime;
  }
}
}

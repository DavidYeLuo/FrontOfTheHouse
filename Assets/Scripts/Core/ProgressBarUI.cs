using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using UI;

namespace UI {
// View class for the progress bar
public class ProgressBarUI : MonoBehaviour {
  [Header("Models")]
  [SerializeField]
  private ProgressBar _progressBar;

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

  private void OnEnable() {
    _progressBar.beginProgressBarHandler += BeginTask;
    _progressBar.cancelProgressBarHandler += CancelTask;
  }
  private void OnDisable() {
    _progressBar.beginProgressBarHandler -= BeginTask;
    _progressBar.cancelProgressBarHandler -= CancelTask;
  }

  public void BeginTask(float targetSeconds) {
    this.targetSeconds = targetSeconds;
    progressSeconds = 0;
    components.gameObject.SetActive(true);
  }
  public void CancelTask() {
    Debug.Log("Task cancelled");
    components.gameObject.SetActive(false);
  }
  private void Update() {
    if (progressSeconds > targetSeconds) {
      progressBar.fillAmount = 1.0f;
      components.gameObject.SetActive(false);
    }
    progressBar.fillAmount = progressSeconds / targetSeconds;
    progressSeconds += Time.deltaTime;
  }
}
}

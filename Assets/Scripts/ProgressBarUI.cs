using UnityEngine;
using UnityEngine.UI;

namespace UI {
public delegate void FinishProgressHandler();
public class ProgressBarUI : MonoBehaviour {
  public event FinishProgressHandler finishProgressBarHandler;

  [Header("Dependencies")]
  [SerializeField]
  private Image
      progressBar; // NOTE: turning this off will leave the background intact so
                   // we should turn off a group of gameobject insteadj
  [SerializeField]
  private Canvas canvas; // GameObject that represents the progress bar TODO:
                         // use a game object instead

  private float targetSeconds;
  private float progressSeconds;

  public void BeginTask(float targetSeconds) {
    this.targetSeconds = targetSeconds;
    progressSeconds = 0;
    canvas.gameObject.SetActive(true);
  }
  public void CancelTask() {}
  // private void Start() { progressBar = GetComponent<Image>(); }
  private void Update() {
    Debug.Log(progressSeconds);
    if (progressSeconds > targetSeconds) {
      progressBar.fillAmount = 1.0f;
      finishProgressBarHandler?.Invoke();
      canvas.gameObject.SetActive(false);
    }
    progressBar.fillAmount = progressSeconds / targetSeconds;
    progressSeconds += Time.deltaTime;
  }
}
}

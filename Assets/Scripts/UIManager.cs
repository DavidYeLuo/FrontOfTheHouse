using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
public class UIManager : MonoBehaviour {
  [SerializeField]
  private GameObject pauseUI;
  [SerializeField]
  private GameObject winScreenUI;

  public void SetWinUI(bool active) { winScreenUI.SetActive(active); }
  public void SetPauseUI(bool active) { pauseUI.SetActive(active); }
}
}

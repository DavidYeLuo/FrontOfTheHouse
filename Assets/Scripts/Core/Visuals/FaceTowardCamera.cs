using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
public class FaceTowardCamera : MonoBehaviour {
  [SerializeField]
  RectTransform rect;
  private void LateUpdate() {
    rect.rotation = Quaternion.LookRotation(Camera.main.transform.forward,
                                            Camera.main.transform.up);
  }
}
}

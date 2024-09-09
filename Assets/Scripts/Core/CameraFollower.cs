using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraFollower : MonoBehaviour {
  public List<GameObject> targets;
  public Vector3 offset = Vector3.zero;
  public float smoothTime = 0.25f;
  private Vector3 previousPosition = Vector3.zero;
  private Vector3 velocity = Vector3.zero;

  public void Init(List<GameObject> targets, Vector3 offset, float smoothTime) {
    Assert.IsTrue(targets.Count > 0);
    this.targets = targets;
    this.offset = offset;
    this.smoothTime = smoothTime;
    for (int i = 0; i < targets.Count; i++) {
      previousPosition += targets[i].transform.position;
    }
    previousPosition /= targets.Count;
  }

  private void Start() {
    Assert.IsTrue(targets.Count > 0);

    for (int i = 0; i < targets.Count; i++) {
      previousPosition += targets[i].transform.position;
    }
    previousPosition /= targets.Count;
  }

  private void LateUpdate() {
    Vector3 averagePosition = Vector3.zero;
    for (int i = 0; i < targets.Count; i++) {
      averagePosition += targets[i].transform.position;
    }
    averagePosition /= targets.Count;

    // Smoothly move the camera to the middle of the players
    Vector3 smoothedPosition = Vector3.SmoothDamp(
        previousPosition, averagePosition, ref velocity, smoothTime);
    transform.position = smoothedPosition + offset;
    previousPosition = smoothedPosition;
  }
}

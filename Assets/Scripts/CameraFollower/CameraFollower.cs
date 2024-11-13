using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace CameraEffects {
public enum CameraOffsetStrategy {
  STATIC_OFFSET,
  LINEAR_OFFSET,
  ROOT_OFFSET,
  INVERSE_PROPORTIONAL_OFFSET
}
public class CameraFollower : MonoBehaviour {
  public List<GameObject> targets;
  public Vector3 offset = Vector3.zero;
  public float smoothTime = 0.25f;
  public CameraOffsetStrategy offsetStrategy =
      CameraOffsetStrategy
          .LINEAR_OFFSET; // Based on quick tests, Linear offset is the only one
                          // that keeps all objects in view regardless of
                          // distance
  private CameraOffset cameraOffset;
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
    cameraOffset = InitOffsetStrategy(offsetStrategy);
    Assert.IsNotNull(cameraOffset);
  }

  private void Start() {
    Assert.IsTrue(targets.Count > 0);

    for (int i = 0; i < targets.Count; i++) {
      previousPosition += targets[i].transform.position;
    }
    previousPosition /= targets.Count;
    cameraOffset = InitOffsetStrategy(offsetStrategy);
    Assert.IsNotNull(cameraOffset);
  }
  private CameraOffset InitOffsetStrategy(CameraOffsetStrategy strategy) {
    switch (strategy) {
    case CameraOffsetStrategy.STATIC_OFFSET:
      return new StaticOffset(targets, offset);
    case CameraOffsetStrategy.LINEAR_OFFSET:
      return new LinearOffset(targets, offset);
    case CameraOffsetStrategy.ROOT_OFFSET:
      return new RootOffset(targets, offset);
    case CameraOffsetStrategy.INVERSE_PROPORTIONAL_OFFSET:
      return new InverseProportionalOffset(targets, offset);
    default:
      return new LinearOffset(targets, offset);
    }
  }

  protected void LateUpdate() {
    Vector3 averagePosition = Vector3.zero;
    for (int i = 0; i < targets.Count; i++) {
      averagePosition += targets[i].transform.position;
    }
    averagePosition /= targets.Count;

    // Smoothly move the camera to the middle of the players
    Vector3 smoothedPosition = Vector3.SmoothDamp(
        previousPosition, averagePosition, ref velocity, smoothTime);
    transform.position =
        smoothedPosition + cameraOffset.CalculateOffset(averagePosition);
    previousPosition = smoothedPosition;
  }
  private abstract class CameraOffset {
    public List<GameObject> targets;
    public CameraOffset(List<GameObject> targets) { this.targets = targets; }
    public abstract Vector3 CalculateOffset(Vector3 midpoint);
  }
  private class StaticOffset : CameraOffset {
    public Vector3 staticOffset;
    public StaticOffset(List<GameObject> targets, Vector3 staticOffset)
        : base(targets) {
      this.staticOffset = staticOffset;
    }

    public override Vector3 CalculateOffset(Vector3 midpoint) {
      return staticOffset; // TODO: Fix
    }
  }
  private class LinearOffset : CameraOffset {
    public Vector3 staticOffset;
    private Vector3 staticOffsetNormalized;
    public LinearOffset(List<GameObject> targets) : base(targets) {
      staticOffset = Vector3.zero;
      staticOffsetNormalized = Vector3.zero;
    }
    public LinearOffset(List<GameObject> targets, Vector3 staticOffset)
        : base(targets) {
      this.staticOffset = staticOffset;
      staticOffsetNormalized = staticOffset.normalized;
    }

    public override Vector3 CalculateOffset(Vector3 midpoint) {
      float midDistance =
          Vector3.Distance(midpoint, targets[0].transform.position);
      return midDistance * staticOffsetNormalized + staticOffset;
    }
  }
  private class RootOffset : CameraOffset {
    public Vector3 staticOffset;
    private Vector3 staticOffsetNormalized;
    public RootOffset(List<GameObject> targets) : base(targets) {
      staticOffset = Vector3.zero;
      staticOffsetNormalized = Vector3.zero;
    }
    public RootOffset(List<GameObject> targets, Vector3 staticOffset)
        : base(targets) {
      this.staticOffset = staticOffset;
      staticOffsetNormalized = staticOffset.normalized;
    }

    public override Vector3 CalculateOffset(Vector3 midpoint) {
      float midDistance =
          Mathf.Sqrt(Vector3.Distance(midpoint, targets[0].transform.position));
      return midDistance * staticOffsetNormalized + staticOffset;
    }
  }
  private class InverseProportionalOffset : CameraOffset {
    public Vector3 staticOffset;
    private Vector3 staticOffsetNormalized;
    public InverseProportionalOffset(List<GameObject> targets) : base(targets) {
      staticOffset = Vector3.zero;
      staticOffsetNormalized = Vector3.zero;
    }
    public InverseProportionalOffset(List<GameObject> targets,
                                     Vector3 staticOffset)
        : base(targets) {
      this.staticOffset = staticOffset;
      staticOffsetNormalized = staticOffset.normalized;
    }

    public override Vector3 CalculateOffset(Vector3 midpoint) {
      float midDistance =
          Vector3.Distance(midpoint, targets[0].transform.position);
      return 1 / midDistance * staticOffsetNormalized + staticOffset;
    }
  }
}
}

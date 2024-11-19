using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;
using PlayerAction;
using UnityEngine.Assertions;

namespace Interactable {
public class ItemSnapper : MonoBehaviour {
  [SerializeField]
  private List<GameObject> snapPoints;
  private void Awake() {
    Assert.IsNotNull(snapPoints);
    Assert.IsTrue(snapPoints.Count > 0);
  }
  private void OnCollisionEnter(Collision c) {
    IItemSnap item;
    if (c.gameObject.TryGetComponent<IItemSnap>(out item)) {
      float shortestSqrMagnitude = Vector3.SqrMagnitude(
          c.transform.position - snapPoints[0].transform.position);
      GameObject closestSnapPoint = snapPoints[0];
      for (int i = 1; i < snapPoints.Count; i++) {
        float sqrMagnitude = Vector3.SqrMagnitude(
            snapPoints[i].transform.position - c.transform.position);
        if (sqrMagnitude > shortestSqrMagnitude)
          continue;
        shortestSqrMagnitude = sqrMagnitude;
        closestSnapPoint = snapPoints[i];
      }
      c.transform.position = closestSnapPoint.transform.position;
    }
  }
}
}

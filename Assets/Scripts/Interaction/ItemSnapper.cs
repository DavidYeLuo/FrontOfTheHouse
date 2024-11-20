using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;
using PlayerAction;
using UnityEngine.Assertions;
using System.Linq;

namespace Interactable {
public class ItemSnapper : MonoBehaviour {
  public float threshHold = 2.0f;
  public List<IItemSnap.SnapCategories> matchSnapCategories;
  [SerializeField]
  private List<GameObject> snapPoints;
  private void Awake() {
    Assert.IsNotNull(snapPoints);
    Assert.IsTrue(snapPoints.Count > 0);
  }
  private void OnCollisionEnter(Collision c) {
    IItemSnap item;
    if (c.gameObject.TryGetComponent<IItemSnap>(out item)) {
      // Don't include if item category doesn't match
      List<IItemSnap.SnapCategories> itemsCategory = item.GetCategories();
      bool hasCategoryMatch = false;
      foreach (var matchCategory in matchSnapCategories) {
        foreach (var category in itemsCategory) {
          if (matchCategory == category)
            hasCategoryMatch = true;
        }
      }
      if (!hasCategoryMatch)
        return;

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
      if (Vector3.Distance(closestSnapPoint.transform.position,
                           c.transform.position) > threshHold)
        return;

      // Places the object slightly higher to reduce bounciness
      c.transform.position = closestSnapPoint.transform.position -
                             item.GetBottomSnapPoint() +
                             IItemSnap.GLOBAL_OFFSET;
    }
  }
}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactable {
public interface IItemSnap {
  public enum SnapCategories { FoodTray, MuffinBox }
  // Places the object slightly higher to reduce bounciness due to collision
  // when placing the object
  private static Vector3 _globalOffset = new Vector3(0, 0.2f, 0);
  public static Vector3 GLOBAL_OFFSET {
    get { return _globalOffset; }
  }

  /// Define where the object's bottom is
  public Vector3 GetBottomSnapPoint() { return Vector3.zero; }
  /// Define the object's category
  /// Some snapper filters which object to snap
  public List<SnapCategories> GetCategories();
}
}

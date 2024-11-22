using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAction;
using System;
using UnityEngine.Assertions;
using Interactable;
using Entity;

namespace Interactable {
public class FoodTray : ItemHolder<Food>,
                        IInteractable,
                        IDropItem,
                        IPickupItem,
                        IItemSnap {
  [Header("Item Snapping")]
  public Vector3 bottomSnapPoint;
  public List<IItemSnap.SnapCategories> snapCategories;
  public void Accept(IInteractor interactor) { interactor.Interact(this); }

  public int CountFoodCategories(List<FoodCategory> matchCategories) {
    int count = 0;
    List<Food> foodList = this.ItemList;
    foreach (var food in foodList) {
      if (food.ContainsCategory(matchCategories)) {
        count++;
      }
    }
    return count;
  }

  public override GameObject DropItem() {
    thisCollider.enabled = true;
    rb.isKinematic = false;
    return this.gameObject;
  }
  public GameObject PickupItem() {
    rb.isKinematic = true;
    Itemize();
    return this.gameObject;
  }
  public Vector3 GetBottomSnapPoint() { return bottomSnapPoint; }
  public List<IItemSnap.SnapCategories> GetCategories() {
    return snapCategories;
  }
  private void OnDrawGizmosSelected() {
    Gizmos.color = Color.green;
    Vector3 direction = transform.TransformDirection(bottomSnapPoint);
    Gizmos.DrawRay(transform.position, direction);
    Gizmos.DrawWireSphere(transform.position + bottomSnapPoint, 0.1f);
  }
}
}

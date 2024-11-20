using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAction;
using System;
using UnityEngine.Assertions;
using Interactable;
using Food;
using Entity;

namespace Interactable {
public class FoodTray : ItemHolder<Muffin>,
                        IInteractable,
                        IDropItem,
                        IPickupItem,
                        IItemSnap {
  [Header("Item Snapping")]
  public Vector3 bottomSnapPoint;
  public List<IItemSnap.SnapCategories> snapCategories;
  public void Accept(IInteractor interactor) { interactor.Interact(this); }

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

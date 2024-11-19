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
}
}

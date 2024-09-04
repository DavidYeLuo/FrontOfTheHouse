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
                        IDroppable,
                        IPickupItem {
  public void Accept(IInteractor interactor) { interactor.Interact(this); }

  public GameObject GetGameObject() {
    Itemize();
    return this.gameObject;
  }
}
}

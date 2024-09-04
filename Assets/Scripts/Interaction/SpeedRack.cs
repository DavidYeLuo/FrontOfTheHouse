using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;
using PlayerAction;
using Entity;

// Implementation is pretty much like foodtray.
// TODO:
// We should use generics to split the classes to hold whichever items
namespace Interactable {
public class SpeedRack : ItemHolder<FoodTray>,
                         IInteractable,
                         IDroppable,
                         IPickupItem {
  public void Accept(IInteractor interactor) { interactor.Interact(this); }
  public GameObject GetGameObject() { return this.gameObject; }
}
}

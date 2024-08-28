using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;
using PlayerAction;

// Implementation is pretty much like foodtray.
// TODO:
// We should use generics to split the classes to hold whichever items
namespace Interactable {
public class SpeedRack : ItemHolder<FoodTray>, IInteractable, IDroppable {
  public void Accept(IInteractor interactor) { interactor.Interact(this); }
}
}

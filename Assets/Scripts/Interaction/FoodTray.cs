using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAction;
using System;
using UnityEngine.Assertions;
using Interactable;
using Food;

namespace Interactable {
public class FoodTray : ItemHolder<Muffin>, IInteractable {
  public void Accept(IInteractor interactor) { interactor.Interact(this); }
}
}

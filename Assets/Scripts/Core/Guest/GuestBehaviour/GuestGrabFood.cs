using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Food;
using Interactable;

namespace GuestBehaviour {
public class GuestGrabFood<T> : IGuestTick
    where T : Component {
  public void Tick() {}
}
}

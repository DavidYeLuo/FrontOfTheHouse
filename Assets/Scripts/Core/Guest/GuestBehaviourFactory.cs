using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuestBehaviour {
public class GuestBehaviourFactory {
  public IGuestTick GetGuestPathTowardGameObject(GameObject movingObject,
                                                 GameObject objectToReach,
                                                 float moveSpeed) {
    return new GuestPathTowardGameObject(movingObject, objectToReach,
                                         moveSpeed);
  }
  public IGuestTick GetGuestIdle() { return new GuestIdle(); }
}
}

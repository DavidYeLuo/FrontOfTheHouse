using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuestBehaviour {
public class GuestBehaviourFactory {
  public IGuestTick GetGuestPathTowardGameObject(GameObject movingObject,
                                                 GameObject objectToReach,
                                                 float moveSpeed,
                                                 float stopDistance) {
    return new GuestPathTowardGameObject(movingObject, objectToReach, moveSpeed,
                                         stopDistance);
  }
  public IGuestTick GetGuestIdle() { return new GuestIdle(); }
}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;
using Food;

namespace GuestBehaviour {
public class GuestBehaviourFactory {
  public IGuestTick GetGuestPathTowardGameObject(GameObject movingObject,
                                                 GameObject objectToReach,
                                                 float moveSpeed,
                                                 float stopDistance) {
    return new GuestPathTowardGameObject(movingObject, objectToReach, moveSpeed,
                                         stopDistance);
  }
  public IGuestTick GetGuestPathTowardNavmesh(GameObject movingObject,
                                              GameObject objectToReach,
                                              float moveSpeed,
                                              float stopDistance) {
    return new GuestPathTowardNavmesh(movingObject, objectToReach, moveSpeed,
                                      stopDistance);
  }
  public IGuestTick GetGuestGrabFood() { return new GuestGrabMuffin(); }
  public IGuestTick GetGuestIdle() { return new GuestIdle(); }
}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuestBehaviour {
public class GuestPathTowardGameObject : IGuestTick {
  protected GameObject movingGuest;
  protected GameObject objectToReach;
  protected float speed;
  protected float stopDistance;
  public GuestPathTowardGameObject(GameObject movingObject,
                                   GameObject objectToReach, float speed,
                                   float stopDistance) {
    this.movingGuest = movingObject;
    this.objectToReach = objectToReach;
    this.speed = speed;
    this.stopDistance = stopDistance;
  }
  public virtual void Tick() {
    float distance = Vector3.Distance(movingGuest.transform.position,
                                      objectToReach.transform.position);
    if (distance < stopDistance)
      return;
    movingGuest.transform.position = Vector3.MoveTowards(
        movingGuest.transform.position, objectToReach.transform.position,
        Time.fixedDeltaTime * speed);
  }
}
}

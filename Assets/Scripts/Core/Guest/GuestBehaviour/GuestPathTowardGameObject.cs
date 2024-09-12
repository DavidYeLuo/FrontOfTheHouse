using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuestBehaviour {
public class GuestPathTowardGameObject : IGuestTick {
  private GameObject movingGuest;
  private GameObject objectToReach;
  private float speed;
  private float stopDistance;
  public GuestPathTowardGameObject(GameObject movingObject,
                                   GameObject objectToReach, float speed,
                                   float stopDistance) {
    this.movingGuest = movingObject;
    this.objectToReach = objectToReach;
    this.speed = speed;
    this.stopDistance = stopDistance;
  }
  public void Tick() {
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

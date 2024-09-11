using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GuestBehaviour {
public class GuestPathTowardGameObject : IGuestTick {
  private GameObject movingGuest;
  private GameObject objectToReach;
  private float speed;
  public GuestPathTowardGameObject(GameObject movingObject,
                                   GameObject objectToReach, float speed) {
    this.movingGuest = movingObject;
    this.objectToReach = objectToReach;
    this.speed = speed;
  }
  public void Tick() {
    movingGuest.transform.position = Vector3.MoveTowards(
        movingGuest.transform.position, objectToReach.transform.position,
        Time.fixedDeltaTime * speed);
  }
}
}

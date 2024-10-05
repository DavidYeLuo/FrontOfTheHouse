using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace GuestBehaviour {
public class GuestPathTowardNavmesh : GuestPathTowardGameObject {
  public NavMeshAgent agent;
  public GuestPathTowardNavmesh(GameObject movingObject,
                                GameObject objectToReach, float speed,
                                float stopDistance)
      : base(movingObject, objectToReach, speed, stopDistance) {
    agent = movingObject.GetComponent<NavMeshAgent>();
  }
  public override void Tick() {
    agent.SetDestination(objectToReach.transform.position);
  }
}
}

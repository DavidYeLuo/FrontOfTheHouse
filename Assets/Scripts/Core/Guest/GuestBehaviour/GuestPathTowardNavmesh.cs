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
    // agent.Move(objectToReach.transform.position);
    NavMeshPath path = new NavMeshPath();
    agent.SetDestination(objectToReach.transform.position);
  }
  public override void Tick() {}
}
}

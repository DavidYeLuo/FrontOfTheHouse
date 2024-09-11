using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuestBehaviour;
using UnityEngine.Assertions;

namespace NPC {
public enum GuestGoal { EXPLORE, TALK, HUNGRY }
public class Guest : MonoBehaviour {
  public static GuestGoal GetDefaultGoal() { return GuestGoal.EXPLORE; }
  private IGuestTick GetDefaultBehaviour() {
    return behaviourFactory.GetGuestIdle();
  }
  private GuestGoal goal;
  private IGuestTick behaviour;
  private GuestBehaviourFactory behaviourFactory = new GuestBehaviourFactory();

  public float moveSpeed;

  private List<GameObject> listOfInterest = new List<GameObject>();

  public void Init(GuestGoal goal) { SetGoal(goal); }
  public void Init(GuestGoal goal, List<GameObject> listOfInterest) {
    listOfInterest.ForEach((obj) => this.listOfInterest.Add(obj));
    SetGoal(goal);
  }
  public void AddListOfInterests(List<GameObject> listOfInterest) {
    listOfInterest.ForEach((obj) => this.listOfInterest.Add(obj));
    // Update State
    SetGoal(goal);
  }
  private void FixedUpdate() { behaviour.Tick(); }

  public void SetGoal(GuestGoal goal) {
    this.goal = goal;
    switch (goal) {
    case GuestGoal.EXPLORE:
      // Nothing interesting in the level? Just start talking
      if (listOfInterest.Count == 0) {
        SetGoal(GuestGoal.TALK);
        break;
      }

      int rng = (int)Random.Range(0, listOfInterest.Count);
      Assert.AreNotEqual(rng, listOfInterest.Count);
      GameObject objectOfInterest = listOfInterest[rng];
      behaviour = behaviourFactory.GetGuestPathTowardGameObject(
          this.gameObject, objectOfInterest, moveSpeed);
      break;
    case GuestGoal.TALK:
      break;
    case GuestGoal.HUNGRY:
      break;
    }
  }
}
}

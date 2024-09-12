using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuestBehaviour;
using UnityEngine.Assertions;
using PlayerAction; // needed for Elevator

namespace NPC {
public enum GuestGoal { EXPLORE, TALK, HUNGRY, LEAVE }
public class Guest : MonoBehaviour {
  public static GuestGoal GetDefaultGoal() { return GuestGoal.EXPLORE; }
  private const float CONDITION_MARGIN_OF_ERROR = 0.25f;
  private IGuestTick GetDefaultBehaviour() {
    return behaviourFactory.GetGuestIdle();
  }
  private GuestGoal goal;
  private IGuestTick behaviour;
  private GuestBehaviourFactory behaviourFactory = new GuestBehaviourFactory();

  public float moveSpeed = 3.0f;
  [Tooltip("When moving toward gameobject, this is how far it will stop")]
  public float stopDistance = 3.0f;

  private List<GameObject> listOfInterest = new List<GameObject>();
  private List<Elevator> listOfExits = new List<Elevator>();

  private AbstractTransitionFactory transitionFactory =
      new AbstractTransitionFactory();
  private Transition currentTransition;

  public void Init(GuestGoal goal) { SetGoal(goal); }
  public void Init(GuestGoal goal, List<GameObject> listOfInterest,
                   List<Elevator> listOfExits) {
    listOfInterest.ForEach((obj) => this.listOfInterest.Add(obj));
    listOfExits.ForEach((obj) => this.listOfExits.Add(obj));
    SetGoal(goal);
  }
  public void AddListOfInterests(List<GameObject> listOfInterest) {
    listOfInterest.ForEach((obj) => this.listOfInterest.Add(obj));
    // Update State
    SetGoal(goal);
  }
  private void FixedUpdate() {
    behaviour.Tick();
    if (currentTransition.IsMet()) {
      SetGoal(currentTransition.nextGoal);
    }
  }

  public void SetGoal(GuestGoal goal) {
    this.goal = goal;
    int rng;
    switch (goal) {
    case GuestGoal.EXPLORE:
      // Nothing interesting in the level? Just start talking
      if (listOfInterest.Count == 0) {
        SetGoal(GuestGoal.LEAVE);
        break;
      }

      rng = (int)Random.Range(0, listOfInterest.Count);
      Assert.AreNotEqual(rng, listOfInterest.Count);
      GameObject objectOfInterest = listOfInterest[rng];
      behaviour = behaviourFactory.GetGuestPathTowardGameObject(
          this.gameObject, objectOfInterest, moveSpeed, stopDistance);
      currentTransition = transitionFactory.GetWhenCloseDistance(
          this, GuestGoal.LEAVE, objectOfInterest.transform.position,
          stopDistance + CONDITION_MARGIN_OF_ERROR);

      // TODO: interact with the object
      break;
    case GuestGoal.TALK:
      break;
    case GuestGoal.LEAVE:
      rng = (int)Random.Range(0, listOfExits.Count);
      Elevator elevator = listOfExits[rng];
      behaviour = behaviourFactory.GetGuestPathTowardGameObject(
          this.gameObject, elevator.gameObject, moveSpeed, stopDistance);
      currentTransition = transitionFactory.GetWhenCloseDistance(
          this, GuestGoal.LEAVE, elevator.transform.position,
          stopDistance + CONDITION_MARGIN_OF_ERROR);

      // TODO: interact with the object
      break;
    case GuestGoal.HUNGRY:
      break;
    }
  }
  private abstract class Transition {
    public Guest guest;
    public GuestGoal nextGoal;
    public Transition(Guest guest, GuestGoal goal) {
      this.guest = guest;
      this.nextGoal = goal;
    }
    public abstract bool IsMet();
  }
  private class AbstractTransitionFactory {
    public Transition GetWhenCloseDistance(Guest guest,
                                           GuestGoal goalTransition,
                                           Vector3 targetPoint,
                                           float distance) {
      return new TransitionWhenCloseDistance(guest, goalTransition, targetPoint,
                                             distance);
    }
  }
  private class TransitionWhenCloseDistance : Transition {
    private float distance;
    private Vector3 targetPoint;
    public TransitionWhenCloseDistance(Guest guest, GuestGoal goalTransition,
                                       Vector3 targetPoint, float distance)
        : base(guest, goalTransition) {
      this.distance = distance;
      this.targetPoint = targetPoint;
    }
    public override bool IsMet() {
      return Vector3.Distance(guest.transform.position, targetPoint) < distance;
    }
  }
}
}

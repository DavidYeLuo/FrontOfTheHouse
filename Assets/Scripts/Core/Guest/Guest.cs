using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GuestBehaviour;
using UnityEngine.Assertions;
using PlayerAction;
using Interactable; // needed for Elevator
using Food;
using ObjectPool;

namespace NPC {
public enum GuestGoal { EXPLORE, TALK, HUNGRY, LEAVE, GRAB_FOOD, AT_ELEVATOR }
public delegate void GuestLeaveEvent(Guest guest);
public class Guest : PoolObject {
  public event GuestLeaveEvent OnGuestLeave;

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
  public GameObject itemSlot;

  private List<GameObject> listOfInterests = new List<GameObject>();
  private List<Elevator> listOfExits = new List<Elevator>();

  private List<FoodTray> listOfFoodTrays = new List<FoodTray>();
  private AbstractTransitionFactory transitionFactory =
      new AbstractTransitionFactory();
  private Transition currentTransition;

  private FoodTray foodTrayOfInterest = null;

  public void Init(GuestGoal goal) {
    SetGoal(goal);
    listOfInterests.ForEach((interest) => {
      FoodTray foodTray = interest.GetComponent<FoodTray>();
      if (foodTray != null)
        listOfFoodTrays.Add(foodTray);
    });
  }
  public void Init(GuestGoal goal, List<GameObject> listOfInterest,
                   List<Elevator> listOfExits) {
    listOfInterest.ForEach((obj) => this.listOfInterests.Add(obj));
    listOfExits.ForEach((obj) => this.listOfExits.Add(obj));
    listOfInterests.ForEach((interest) => {
      FoodTray foodTray = interest.GetComponent<FoodTray>();
      if (foodTray != null)
        listOfFoodTrays.Add(foodTray);
      Debug.Log($"foodTray: {foodTray}");
    });
    SetGoal(goal);
  }
  public void AddListOfInterests(List<GameObject> listOfInterest) {
    listOfInterest.ForEach((obj) => this.listOfInterests.Add(obj));
    listOfInterests.ForEach((interest) => {
      FoodTray foodTray = interest.GetComponent<FoodTray>();
      if (foodTray != null)
        listOfFoodTrays.Add(foodTray);
    });
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
      if (listOfInterests.Count == 0) {
        SetGoal(GuestGoal.LEAVE);
        break;
      }

      rng = (int)Random.Range(0, listOfInterests.Count);
      foodTrayOfInterest = listOfFoodTrays[rng];
      if (foodTrayOfInterest != null) {
        // behaviour = behaviourFactory.GetGuestPathTowardGameObject(
        //     this.gameObject, foodTrayOfInterest.gameObject, moveSpeed,
        //     stopDistance);
        behaviour = behaviourFactory.GetGuestPathTowardNavmesh(
            this.gameObject, foodTrayOfInterest.gameObject, moveSpeed,
            stopDistance);
        GuestGoal transitionGoal = GuestGoal.GRAB_FOOD;
        currentTransition = transitionFactory.GetWhenCloseDistance(
            this, transitionGoal, foodTrayOfInterest.transform.position,
            stopDistance + CONDITION_MARGIN_OF_ERROR);
      } else {
        SetGoal(GuestGoal.LEAVE);
      }
      break;
    case GuestGoal.TALK:
      break;
    case GuestGoal.LEAVE:
      rng = (int)Random.Range(0, listOfExits.Count);
      Elevator elevator = listOfExits[rng];
      // behaviour = behaviourFactory.GetGuestPathTowardGameObject(
      //     this.gameObject, elevator.gameObject, moveSpeed, stopDistance);
      behaviour = behaviourFactory.GetGuestPathTowardNavmesh(
          this.gameObject, elevator.gameObject, moveSpeed, stopDistance);
      currentTransition = transitionFactory.GetWhenCloseDistance(
          this, GuestGoal.AT_ELEVATOR, elevator.transform.position,
          stopDistance + CONDITION_MARGIN_OF_ERROR);
      break;
    case GuestGoal.HUNGRY:
      break;
    case GuestGoal.GRAB_FOOD:
      behaviour = behaviourFactory.GetGuestGrabFood();
      currentTransition = transitionFactory.GetWhenGrabMuffin(
          this, GuestGoal.LEAVE, foodTrayOfInterest);
      break;
    case GuestGoal.AT_ELEVATOR:
      OnGuestLeave?.Invoke(this);
      Destroy();
      break;
    default:
      throw new System.Exception("Unkown goal: " + goal);
    }
  }
  public void Equip(GameObject obj) {
    obj.transform.SetParent(itemSlot.transform);
    // Snaps object in the item slot
    obj.transform.rotation = itemSlot.transform.rotation;
    obj.transform.position = itemSlot.transform.position;
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
    public Transition GetWhenGrabMuffin(Guest guest, GuestGoal goalTransition,
                                        ItemHolder<Muffin> foodHolder) {
      return new TransitionWhenGrabMuffin(guest, goalTransition, foodHolder);
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
  private class TransitionWhenGrabMuffin : Transition {
    private ItemHolder<Muffin> muffinTray;
    public TransitionWhenGrabMuffin(Guest guest, GuestGoal goalTransition,
                                    ItemHolder<Muffin> foodHolder)
        : base(guest, goalTransition) {
      muffinTray = foodHolder;
    }
    public override bool IsMet() {
      if (muffinTray.IsEmpty() ||
          Vector3.Distance(muffinTray.gameObject.transform.position,
                           guest.gameObject.transform.position) >
              guest.stopDistance)
        return false;
      Muffin muffin = muffinTray.RemoveItem();
      guest.Equip(muffin.gameObject);

      return true;
    }
  }
}
}

using UnityEngine;
using PlayerAction;
using Interactable;
using UI;
using ObjectDetection;
using System;
using System.Collections.Generic;
using ObjectPool;
using Entity;
using UnityEngine.Assertions;

namespace Player {
public delegate void PauseHandler();
public class Player : MonoBehaviour, IInteractor {
  public PauseHandler pauseHandler;

  [Header("Movement")]
  [SerializeField]
  private float baseSpeed;
  [SerializeField]
  private float sprintModifier;
  [Tooltip("Object to apply force to")]
  [SerializeField]
  private Rigidbody rb;

  [Header("Interaction")]
  [SerializeField]
  private ProgressBar progressBar; // TODO: Abstract UI somewhere else
  [SerializeField]
  private ParticleSystem buildingParticleSystem;
  [SerializeField]
  private ParticleSystem footprintParticleSystem;

  [Header("Handhold")]
  [SerializeField, Tooltip("position where the item would be holding")]
  private GameObject itemSlot;

  [Header("Sensory")]
  [SerializeField]
  private NotifyOnLeaveTrigger
      interactionRange; // Cancel the task when leaving the interaction range

  [Header("Input")]
  public KeyCode moveLeftKey;
  public KeyCode moveRightKey;
  public KeyCode moveUpKey;
  public KeyCode moveDownKey;
  public KeyCode interactKey;
  public KeyCode sprintKey;
  public KeyCode pickupKey;
  public KeyCode dropKey;
  public KeyCode pauseKey;

  private GameObject lastInteractedObject;
  private GameObject objectHolding; // null if player isn't holding anything
  private PoolObject objectPoolHolding;
  private bool isPlayingFootprint = false;
  private IDropItem droppableObject;

  // Ensures that each key is handled in the FixedUpdate
  private Queue<InputEvent> inputQueue = new Queue<InputEvent>();

  private float horizontal = 0.0f;
  private float vertical = 0.0f;
  private bool isRunning = false;

  private IHover hovering = null;

  public static int Count { get; private set; }

  private void Start() {
    Count++;
    rb = GetComponent<Rigidbody>();
    buildingParticleSystem.Stop();
  }
  private void OnDestroy() { Count--; }
  private void FixedUpdate() {
    // This fixes occasional residual inputs when user presses up the key
    // causing the horizontal/veritcal to be unbalanced
    // TODO: Find an easier way to stablize the input
    horizontal = Mathf.Clamp(horizontal, -1.0f, 1.0f);
    vertical = Mathf.Clamp(vertical, -1.0f, 1.0f);

    // Used to map user input to the xz-plane
    Vector3 h_vector = horizontal * Vector3.right;
    Vector3 v_vector = vertical * Vector3.forward;
    Vector3 sum_vector = h_vector + v_vector;

    // Movement implementation
    if ((sum_vector).sqrMagnitude > 0.1f) {
      // If the player is moving fast with external force, they have full
      // control of the direction instead.
      float moveSpeed = baseSpeed;
      if (isRunning)
        moveSpeed *= sprintModifier;
      rb.velocity =
          Math.Max(rb.velocity.magnitude, moveSpeed) * sum_vector.normalized;
      // TODO: Might be more fun using force to sprint instead
    }

    // Visual
    if ((sum_vector).sqrMagnitude > 0.1f) {
      if (!isPlayingFootprint)
        footprintParticleSystem.Play();
      isPlayingFootprint = true;
    } else {
      if (isPlayingFootprint)
        footprintParticleSystem.Stop();
      isPlayingFootprint = false;
      rb.velocity = new Vector3(0, rb.velocity.y, 0);
    }

    // Interact Button
    RaycastHit[] hit;
    float INTERACTION_RANGE = 3.0f;
    float PICKUP_RANGE = 3.0f;
    // NOTE: It might be better if we handle key presses in Update() instead
    // and having the logics here

    InputEvent inputEvent;
    hit = Physics.RaycastAll(transform.position, transform.forward,
                             INTERACTION_RANGE);
    IInteractable interactable = null;
    GameObject interactableGameObject = null;
    IHover hoverable = null;

    if (hit != null) {
      for (int i = 0; i < hit.Length; i++) {
        interactable = hit[i].collider.GetComponent<IInteractable>();
        Debug.DrawLine(transform.position,
                       hit[i].collider.gameObject.transform.position);
        if (interactable == null)
          continue;
        interactableGameObject = hit[i].collider.gameObject;

        hoverable = hit[i].collider.GetComponent<IHover>();
        if (hoverable == null)
          break;
        if (hovering == null) {
          hovering = hoverable;
          hovering.OnHoverEnter();
        }
        break;
      }
    }
    if (hoverable == null && hovering != null) {
      hovering.OnHoverExit();
      hovering = null;
    }

    while (inputQueue.Count > 0) {
      inputEvent = inputQueue.Dequeue();
      if (inputEvent.key == interactKey &&
          inputEvent.type == InputType.KEY_DOWN) {
        if (interactable != null) {
          interactable.Accept(this);
          lastInteractedObject =
              interactableGameObject; // Used for cancelling task
        } else {
          Debug.DrawLine(transform.position,
                         transform.position +
                             transform.forward * INTERACTION_RANGE,
                         Color.red);
        }
      }
      if (inputEvent.key == interactKey &&
          inputEvent.type == InputType.KEY_UP) {
        if (lastInteractedObject != null) {
          buildingParticleSystem.Stop();
          progressBar.CancelTask();
        }
      }
      if (inputEvent.key == dropKey && inputEvent.type == InputType.KEY_DOWN) {
        if (droppableObject == null)
          return;
        GameObject droppedItem;
        droppedItem = droppableObject.DropItem();
        droppedItem.transform.SetParent(null);
        droppableObject = null;
        objectHolding = null;
      }
      if (inputEvent.key == dropKey && inputEvent.type == InputType.KEY_UP) {
      }
      if (inputEvent.key == pickupKey &&
          inputEvent.type == InputType.KEY_DOWN) {
        hit = Physics.RaycastAll(transform.position, transform.forward,
                                 PICKUP_RANGE);
        if (hit != null) {
          for (int i = 0; i < hit.Length; i++) {
            IPickupItem pickupItem =
                hit[i].collider.GetComponent<IPickupItem>();
            Debug.DrawLine(transform.position,
                           hit[i].collider.gameObject.transform.position);
            if (pickupItem == null || objectHolding != null)
              continue;
            objectHolding = pickupItem.PickupItem();
            ParentObjToItemSlot(objectHolding);
            objectHolding.transform.position =
                itemSlot.transform.position +
                itemSlot.transform.forward * pickupItem.GetZOffset();
            if (pickupItem is IDropItem) {
              IDropItem droppable = pickupItem as IDropItem;
              droppableObject = droppable;
            }
            lastInteractedObject = objectHolding;

            break;
          }
        }
        if (inputEvent.key == pickupKey &&
            inputEvent.type == InputType.KEY_UP) {
        }
      }
    }
  }
  private void OnEnable() {
    interactionRange.objectLeftHandler += CheckThenCancelTask;
  }
  private void OnDisable() {
    interactionRange.objectLeftHandler -= CheckThenCancelTask;
  }
  // This is needed for a the NotifyOnLeaveTrigger since it gives a generic
  // GameObject We aren't sure if that gameobject is the player
  // Therefore we should check
  private void CheckThenCancelTask(GameObject obj) {
    if (obj != lastInteractedObject?.gameObject)
      return;
    buildingParticleSystem.Stop();
    progressBar
        .CancelTask(); // NOTE: UI shouldn't be handling the logic but I also
    // didn't wanted to add the progress update in this class
  }

  private void Update() {
    // NOTE: The reason why we use Update instead of FixedUpdate is because
    // fixed update doesn't run when the game is paused.
    if (Input.GetKeyDown(pauseKey)) {
      pauseHandler.Invoke();
    }
    InputEvent keyEvent;
    if (Input.GetKeyDown(interactKey)) {
      keyEvent = new InputEvent();
      keyEvent.key = interactKey;
      keyEvent.type = InputType.KEY_DOWN;
      inputQueue.Enqueue(keyEvent);
    }
    if (Input.GetKeyUp(interactKey)) {
      keyEvent = new InputEvent();
      keyEvent.key = interactKey;
      keyEvent.type = InputType.KEY_UP;
      inputQueue.Enqueue(keyEvent);
    }
    if (Input.GetKeyDown(dropKey)) {
      keyEvent = new InputEvent();
      keyEvent.key = dropKey;
      keyEvent.type = InputType.KEY_DOWN;
      inputQueue.Enqueue(keyEvent);
    }
    if (Input.GetKeyUp(dropKey)) {
      keyEvent = new InputEvent();
      keyEvent.key = dropKey;
      keyEvent.type = InputType.KEY_UP;
      inputQueue.Enqueue(keyEvent);
    }
    if (Input.GetKeyDown(pickupKey)) {
      keyEvent = new InputEvent();
      keyEvent.key = pickupKey;
      keyEvent.type = InputType.KEY_DOWN;
      inputQueue.Enqueue(keyEvent);
    }
    if (Input.GetKeyUp(pickupKey)) {
      keyEvent = new InputEvent();
      keyEvent.key = pickupKey;
      keyEvent.type = InputType.KEY_UP;
      inputQueue.Enqueue(keyEvent);
    }

    if (Input.GetKeyDown(moveLeftKey)) {
      horizontal += -1.0f;
    }
    if (Input.GetKeyUp(moveLeftKey)) {
      horizontal += 1.0f;
    }
    if (Input.GetKeyDown(moveRightKey)) {
      horizontal += 1.0f;
    }
    if (Input.GetKeyUp(moveRightKey)) {
      horizontal += -1.0f;
    }
    if (Input.GetKeyDown(moveUpKey)) {
      vertical += 1.0f;
    }
    if (Input.GetKeyUp(moveUpKey)) {
      vertical += -1.0f;
    }
    if (Input.GetKeyDown(moveDownKey)) {
      vertical += -1.0f;
    }
    if (Input.GetKeyUp(moveDownKey)) {
      vertical += 1.0f;
    }
    if (Input.GetKeyDown(sprintKey)) {
      isRunning = true;
    }
    if (Input.GetKeyUp(sprintKey)) {
      isRunning = false;
    }

    // Looks smoother when rotating in Update than FixedUpdate
    // Rotates the player based on player input
    //
    // Used to map user input to the xz-plane
    Vector3 h_vector = horizontal * Vector3.right;
    Vector3 v_vector = vertical * Vector3.forward;
    Vector3 sum_vector = h_vector + v_vector;
    if (sum_vector == Vector3.zero) {
      return;
    }
    transform.rotation = Quaternion.LookRotation(sum_vector.normalized);
  }
  public void Interact(UtensilBox util) {
    if (objectHolding != null)
      return;
    if (util.IsSorted()) {
      objectPoolHolding = util.GetGoldenSpoon();
      objectHolding = objectPoolHolding.gameObject;
      ParentObjToItemSlot(objectHolding);
      return;
    }
    Debug.Log("[Interact, Player] Player interacted with utensil box.");
    progressBar.finishProgressBarHandler += util.Sort;
    progressBar.finishProgressBarHandler +=
        DeactivateParticleSystem; // Otherwise, the particle will still be
                                  // active after winning
    progressBar.BeginTask(util.secondsToSort);
    buildingParticleSystem.Play();
    Debug.Log(gameObject + " Entered");
  }
  public void Interact(MuffinBox muffinBox) {
    Debug.Log("[Interact, Player] Player interacted with Muffin box.");
    if (objectHolding != null) {
      return;
    }
    // Pickup broken box
    if (muffinBox.IsBroken()) {
      return;
    }
    // Breaks the box when muffinbox is empty
    if (muffinBox.IsEmpty()) {
      Debug.Log("[Interact, Player] Player interacted with muffinbox.");
      progressBar.finishProgressBarHandler += muffinBox.Break;
      progressBar.finishProgressBarHandler +=
          DeactivateParticleSystem; // Otherwise, the particle will still be
                                    // active after winning
      progressBar.BeginTask(muffinBox.secondsToBreak);
      buildingParticleSystem.Play();
      return;
    }
    muffinBox.RemoveItem();
    // GameObject muffin = muffinBox.GetMuffin();
    PoolObject muffin = muffinBox.GetPoolMuffin();
    ParentObjToItemSlot(muffin.gameObject);
    objectHolding = muffin.gameObject;
    objectPoolHolding = muffin;
  }
  public void Interact(LandfillCan can) {
    if (objectHolding == null || can.IsFull())
      return;
    can.AddItem();
    if (objectPoolHolding != null)
      objectPoolHolding.Destroy();
    objectHolding.transform.SetParent(null);
    objectHolding.SetActive(false);
    objectHolding = null;
    objectPoolHolding = null;
  }
  public void Interact(FoodTray tray) {
    // BUG: We shouldn't allow utensils to be in the food tray
    if (objectHolding != null && !tray.IsFull()) {
      Food food = objectHolding.GetComponent<Food>();
      if (food == null)
        return;
      tray.AddItem(food);
      Debug.Log(tray.IsEmpty()); // TODO: remove
      // objectHolding.transform.SetParent(null);
      objectHolding = null;
      objectPoolHolding = null;
    } else if (objectHolding == null) {
      Food food = tray.RemoveItem();
      if (food == null)
        return;
      objectHolding = food.gameObject;
      droppableObject = tray;
      ParentObjToItemSlot(objectHolding);
    }
  }
  public void Interact(SpeedRack speedRack) {
    if (objectHolding != null && !speedRack.IsFull()) {
      FoodTray foodTray = objectHolding.GetComponent<FoodTray>();
      if (foodTray == null)
        return;
      speedRack.AddItem(foodTray);
      objectHolding = null;
      objectPoolHolding = null;
    } else if (objectHolding == null) {
      FoodTray foodTray = speedRack.RemoveItem();
      if (foodTray == null)
        return;
      objectHolding = foodTray.gameObject;
      droppableObject = foodTray;
      ParentObjToItemSlot(objectHolding);
    }
  }
  public void Interact(Elevator elevator) {}

  private void ParentObjToItemSlot(GameObject obj) {
    obj.transform.SetParent(itemSlot.transform);
    // Snaps object in the item slot
    obj.transform.rotation = itemSlot.transform.rotation;
    obj.transform.position = itemSlot.transform.position;
  }
  private void DeactivateParticleSystem() { buildingParticleSystem.Stop(); }
}

public enum InputType { KEY_DOWN, KEY_UP }
struct InputEvent {
  public KeyCode key;
  public InputType type;
}
}

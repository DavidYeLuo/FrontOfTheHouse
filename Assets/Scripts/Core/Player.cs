using UnityEngine;
using PlayerAction;
using Interactable;
using UI;
using ObjectDetection;
using System;
using System.Collections.Generic;
using ObjectPool;

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
  private ProgressBarUI progressBarUI; // TODO: Abstract UI somewhere else
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
  public KeyCode dropKey;
  public KeyCode pauseKey;

  private GameObject lastInteractedObject;
  private GameObject objectHolding; // null if player isn't holding anything
  private PoolObject objectPoolHolding;
  private bool isPlayingFootprint = false;
  private IDroppable droppableObject;
  // TODO: Add a class that handle objectPooling

  // Ensures that each key is handled in the FixedUpdate
  private Queue<InputEvent> inputQueue = new Queue<InputEvent>();

  private float horizontal = 0.0f;
  private float vertical = 0.0f;
  private bool isRunning = false;

  private void Start() {
    rb = GetComponent<Rigidbody>();
    buildingParticleSystem.Stop();
  }
  private void FixedUpdate() {

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
    }

    // Interact Button
    RaycastHit hit;
    float INTERACTION_RANGE = 3.0f;
    // NOTE: It might be better if we handle key presses in Update() instead
    // and having the logics here

    InputEvent inputEvent;
    while (inputQueue.Count > 0) {
      inputEvent = inputQueue.Dequeue();
      if (inputEvent.key == interactKey &&
          inputEvent.type == InputType.KEY_DOWN) {
        if (Physics.Raycast(transform.position, transform.forward, out hit,
                            INTERACTION_RANGE)) {
          Debug.DrawLine(transform.position,
                         transform.position + transform.forward * hit.distance,
                         Color.green);
          Debug.Log("[RayCast, Player] Player raycast hit something.");

          {
            IInteractable interactable =
                hit.collider.GetComponent<IInteractable>();
            if (interactable != null) {
              interactable.Accept(this);
              lastInteractedObject =
                  hit.collider.gameObject; // Used for cancelling task
            }
          }
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
          progressBarUI.CancelTask();
        }
      }
      if (inputEvent.key == dropKey && inputEvent.type == InputType.KEY_DOWN) {
        if (droppableObject == null)
          return;
        GameObject droppedItem;
        droppedItem = droppableObject.Drop();
        droppedItem.transform.SetParent(null);
        droppableObject = null;
        objectHolding = null;
      }
      if (inputEvent.key == dropKey && inputEvent.type == InputType.KEY_UP) {
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
    progressBarUI
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
      objectHolding = util.GetGoldenSpoon();
      ParentObjToItemSlot(objectHolding);
      return;
    }
    Debug.Log("[Interact, Player] Player interacted with utensil box.");
    progressBarUI.finishProgressBarHandler += util.Sort;
    void DeactivateParticleSystem() { buildingParticleSystem.Stop(); }
    progressBarUI.finishProgressBarHandler +=
        DeactivateParticleSystem; // Otherwise, the particle will still be
                                  // active after winning
    progressBarUI.BeginTask(this.gameObject, util.secondsToSort);
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
      Debug.Log("[Interact, Player] Pick up Broken Box");
      muffinBox.Itemize();
      ParentObjToItemSlot(muffinBox.gameObject);

      droppableObject = muffinBox;
      objectHolding = muffinBox.gameObject;

      return;
    }
    // Breaks the box when muffinbox is empty
    if (muffinBox.IsEmpty()) {
      Debug.Log("[Interact, Player] Player interacted with muffinbox.");
      progressBarUI.finishProgressBarHandler += muffinBox.Break;
      void DeactivateParticleSystem() { buildingParticleSystem.Stop(); }
      progressBarUI.finishProgressBarHandler +=
          DeactivateParticleSystem; // Otherwise, the particle will still be
                                    // active after winning
      progressBarUI.BeginTask(this.gameObject, muffinBox.secondsToBreak);
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
  }

  private void ParentObjToItemSlot(GameObject obj) {
    obj.transform.SetParent(itemSlot.transform);
    // Snaps object in the item slot
    obj.transform.position = itemSlot.transform.position;
    obj.transform.rotation = itemSlot.transform.rotation;
  }
}

public enum InputType { KEY_DOWN, KEY_UP }
struct InputEvent {
  public KeyCode key;
  public InputType type;
}
}

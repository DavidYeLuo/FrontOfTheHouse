using UnityEngine;
using PlayerAction;
using Interactable;
using UI;
using ObjectDetection;
using System;
using System.Collections.Generic;

namespace Player {
public delegate void PauseHandler();
public class Player : MonoBehaviour, IInteractor {
  public PauseHandler pauseHandler;

  [Header("Movement")]
  [SerializeField]
  private float force;
  [Tooltip("Object to apply force to")]
  [SerializeField]
  private Rigidbody rb;

  [Header("Interaction")]
  [SerializeField]
  private ProgressBarUI progressBarUI; // TODO: Abstract UI somewhere else
  [SerializeField]
  private GameObject buildingParticleSystem;

  [Header("Handhold")]
  [SerializeField]
  private GameObject goldenSpoonUtensil;

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
  public KeyCode pauseKey;

  private GameObject lastInteractedObject;

  // Ensures that each key is handled in the FixedUpdate
  private Queue<InputEvent> inputQueue = new Queue<InputEvent>();

  private float horizontal = 0.0f;
  private float vertical = 0.0f;

  private void Start() { rb = GetComponent<Rigidbody>(); }
  private void FixedUpdate() {

    // Used to map user input to the xz-plane
    Vector3 h_vector = horizontal * Vector3.right;
    Vector3 v_vector = vertical * Vector3.forward;

    rb.AddForce(
        force * (h_vector + v_vector),
        ForceMode.Force); // Force Implementation
                          // transform.position += force * (h_vector +
                          // v_vector); // Transform Movement Implementation

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
          buildingParticleSystem.SetActive(false);
          progressBarUI.CancelTask();
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
    buildingParticleSystem.SetActive(false);
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

    // Looks smoother when rotating in Update than FixedUpdate
    // Rotates the player based on player input
    //
    // Used to map user input to the xz-plane
    Vector3 h_vector = horizontal * Vector3.right;
    Vector3 v_vector = vertical * Vector3.forward;
    Vector3 sum_vector = h_vector + v_vector;
    if (sum_vector == Vector3.zero)
      return;
    transform.rotation = Quaternion.LookRotation(sum_vector.normalized);
  }
  public void Interact(UtensilBox util) {
    if (util.IsSorted()) {
      // TODO: Handle when user is holding something else
      goldenSpoonUtensil.SetActive(true);
      return;
    }
    Debug.Log("[Interact, Player] Player interacted with utensil box.");
    progressBarUI.finishProgressBarHandler += util.Sort;
    void DeactivateParticleSystem() { buildingParticleSystem.SetActive(false); }
    progressBarUI.finishProgressBarHandler +=
        DeactivateParticleSystem; // Otherwise, the particle will still be
                                  // active after winning
    progressBarUI.BeginTask(this.gameObject, util.secondsToSort);
    buildingParticleSystem.SetActive(true);
    Debug.Log(gameObject + " Entered");
  }
  public void Interact(LandfillCan can) {
    if (!goldenSpoonUtensil.activeSelf || can.IsFull())
      return;
    can.AddItem();
    goldenSpoonUtensil.SetActive(
        false); // TODO: Apply to other objects aswell. Currently, it only throw
                // away the golden spoon.
  }
}

public enum InputType { KEY_DOWN, KEY_UP }
struct InputEvent {
  public KeyCode key;
  public InputType type;
}
}

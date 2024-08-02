using UnityEngine;
using PlayerAction;
using ObjectDetection;
using System;
namespace Interactable {
public delegate void LandfillHandler(TrashState state);
public enum TrashState { EMPTY, FILLED, FULL }
public class LandfillCan : MonoBehaviour, IInteractable {
  public event LandfillHandler onStateChange;

  private TrashState state;

  [SerializeField]
  private int numItems;
  [SerializeField]
  private int maxCapacity;

  [Header("Aesthetic")]
  [SerializeField]
  private GameObject emptyStateObject;
  [SerializeField]
  private GameObject filledStateObject;
  [SerializeField]
  private GameObject fullStateObject;

  private GameObject currentStateObject;

  public int GetNumItems() { return numItems; }
  public int GetCapacity() { return maxCapacity; }
  public bool IsFull() { return numItems == maxCapacity; }
  public void SetMaxCapacity(int capacity) {
    this.maxCapacity = capacity;
    UpdateCapacity();
    UpdateItem();
    onStateChange?.Invoke(state);
  }
  public void AddItem() {
    numItems++;
    UpdateItem();
    onStateChange?.Invoke(state);
  }
  public void SetAmount(int num) {
    numItems = num;
    UpdateItem();
    onStateChange?.Invoke(state);
  }

  // Start is called before the first frame update
  private void Start() { UpdateItem(); }

  private void UpdateItem() {
    numItems = Math.Max(0, numItems);
    numItems = Math.Min(numItems, maxCapacity);

    currentStateObject?.SetActive(false);
    if (numItems == 0) {
      state = TrashState.EMPTY;
      currentStateObject = emptyStateObject;
    } else if (numItems == maxCapacity) {
      state = TrashState.FULL;
      currentStateObject = fullStateObject;
    } else {
      state = TrashState.FILLED;
      currentStateObject = filledStateObject;
    }
    currentStateObject.SetActive(true);
  }
  private void UpdateCapacity() { maxCapacity = Math.Max(0, maxCapacity); }

  public void Accept(IInteractor interactor) { interactor.Interact(this); }
}
}

using UnityEngine;
using PlayerAction;
using ObjectDetection;
using System;

namespace Interactable {
public delegate void MuffinBoxHandler(MuffinBoxState muffinBoxState);
public enum MuffinBoxState { EMPTY, FILLED, FULL }
public class MuffinBox : MonoBehaviour, IInteractable {
  public event MuffinBoxHandler muffinBoxHandler;
  private MuffinBoxState state;

  [SerializeField]
  private int numItems;
  [SerializeField]
  private int maxCapacity;

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
  public bool IsEmpty() { return numItems == 0; }
  public void SetMaxCapacity(int capacity) {
    this.maxCapacity = capacity;
    UpdateCapacity();
    UpdateItem();
    muffinBoxHandler?.Invoke(state);
  }
  public void AddItem() {
    numItems++;
    UpdateItem();
    muffinBoxHandler?.Invoke(state);
  }
  public void RemoveItem() {
    numItems--;
    UpdateItem();
    muffinBoxHandler?.Invoke(state);
  }
  public void SetAmount(int num) {
    numItems = num;
    UpdateItem();
    muffinBoxHandler?.Invoke(state);
  }

  private void Start() { UpdateItem(); }

  private void UpdateItem() {
    numItems = Math.Max(0, numItems);
    numItems = Math.Min(numItems, maxCapacity);

    currentStateObject?.SetActive(false);

    if (numItems == 0) {
      state = MuffinBoxState.EMPTY;
      currentStateObject = emptyStateObject;
    } else if (numItems == maxCapacity) {
      state = MuffinBoxState.FULL;
      currentStateObject = fullStateObject;
    } else {
      state = MuffinBoxState.FILLED;
      currentStateObject = filledStateObject;
    }
    currentStateObject.SetActive(true);
    // TODO: Add a better indication of how many cookies are in the box.
    // Maybe we can change the color closer to the empty box
  }
  private void UpdateCapacity() { maxCapacity = Math.Max(0, maxCapacity); }
  public void Accept(IInteractor interactor) { interactor.Interact(this); }
}
}

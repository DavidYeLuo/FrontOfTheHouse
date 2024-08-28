using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAction;
using System;
using UnityEngine.Assertions;
using Interactable;

namespace Interactable {
public delegate void ItemHolderHandler(ItemHolderState state);
public enum ItemHolderState { EMPTY, FILLED, FULL }
;
public class ItemHolder<T> : MonoBehaviour, IDroppable
    where T : Component {
  public event ItemHolderHandler foodHolderHandler;

  private ItemHolderState state;
  [SerializeField]
  private Collider thisCollider;

  [Header("Dependencies")]
  [SerializeField]
  private List<T> foodHolders;
  private List<T> itemLocations;

  [Header("Aesthetic")]

  private GameObject currentStateObject = null;

  public int GetNumItems() { return itemLocations.Count; }
  public bool IsFull() { return itemLocations.Count == foodHolders.Count; }
  public bool IsEmpty() { return itemLocations.Count == 0; }
  public void AddItem(T obj) {
    itemLocations.Add(obj);
    UpdateItem();
    foodHolderHandler?.Invoke(state);
  }
  public T TakeItem() {
    if (itemLocations.Count == 0)
      return default(T);
    int index = itemLocations.Count - 1;
    T ret = itemLocations[index];
    itemLocations.RemoveAt(index);
    UpdateItem();
    foodHolderHandler?.Invoke(state);
    return ret;
  }
  public void Itemize() { thisCollider.enabled = false; }

  private void Awake() { itemLocations = new List<T>(); }
  private void Start() { UpdateItem(); }

  private void UpdateItem() {
    if (currentStateObject != null)
      currentStateObject.SetActive(false);
    int numItems = itemLocations.Count;
    if (numItems == 0) {
      state = ItemHolderState.EMPTY;
    } else if (numItems == foodHolders.Count) {
      state = ItemHolderState.FULL;
    } else {
      state = ItemHolderState.FILLED;
    }
    // currentStateObject.SetActive(true);
    // TODO: Update the look of the tray as we add item
    for (int i = 0; i < itemLocations.Count; i++) {
      itemLocations[i].transform.SetParent(foodHolders[i].transform);
      itemLocations[i].transform.position = foodHolders[i].transform.position;
      itemLocations[i].transform.rotation = foodHolders[i].transform.rotation;
    }
  }
  public GameObject Drop() {
    this.thisCollider.enabled = true;
    return this.gameObject;
  }
}
}

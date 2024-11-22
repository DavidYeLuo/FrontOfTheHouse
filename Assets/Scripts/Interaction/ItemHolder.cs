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
[RequireComponent(typeof(Rigidbody))]
public class ItemHolder<T> : MonoBehaviour, IDropItem
    where T : Component {
  public event ItemHolderHandler foodHolderHandler;

  private ItemHolderState state;
  [SerializeField]
  protected Collider thisCollider;

  [Header("Dependencies")]
  [SerializeField]
  private List<GameObject> itemPlacement;
  private List<T> itemList;
  /// Should only be used as accessor
  /// Mutating the state of the list isn't recommeded
  protected List<T> ItemList {
    get { return itemList; }
  }

  [Header("Aesthetic")]

  private GameObject currentStateObject = null;
  protected Rigidbody rb;

  public int GetNumItems() { return itemList.Count; }
  public bool IsFull() { return itemList.Count == itemPlacement.Count; }
  public bool IsEmpty() { return itemList.Count == 0; }
  public void AddItem(T obj) {
    itemList.Add(obj);
    UpdateItem();
    foodHolderHandler?.Invoke(state);
  }
  public T RemoveItem() {
    if (itemList.Count == 0)
      return null;
    T ret = itemList[itemList.Count - 1];
    itemList.Remove(ret);
    return ret;
  }
  public T RemoveItem(T item) {
    if (itemList.Contains(item)) {
      itemList.Remove(item);
      return item;
    }
    return null;
  }
  public ItemHolderState GetState() { return state; }
  // LIFO - Returns the last item the player inserted
  public T PeekTopItem() {
    if (itemList.Count == 0) {
      return default(T);
    }
    return itemList[itemList.Count - 1];
  }
  public T TakeItem() {
    if (itemList.Count == 0)
      return default(T);
    int index = itemList.Count - 1;
    T ret = itemList[index];
    itemList.RemoveAt(index);
    UpdateItem();
    foodHolderHandler?.Invoke(state);
    return ret;
  }
  public void Itemize() { thisCollider.enabled = false; }

  private void Awake() {
    itemList = new List<T>();
    rb = GetComponent<Rigidbody>();
  }
  private void Start() { UpdateItem(); }

  private void UpdateItem() {
    if (currentStateObject != null)
      currentStateObject.SetActive(false);
    int numItems = itemList.Count;
    if (numItems == 0) {
      state = ItemHolderState.EMPTY;
    } else if (numItems == itemPlacement.Count) {
      state = ItemHolderState.FULL;
    } else {
      state = ItemHolderState.FILLED;
    }
    // currentStateObject.SetActive(true);
    // TODO: Update the look of the tray as we add item
    for (int i = 0; i < itemList.Count; i++) {
      itemList[i].transform.SetParent(itemPlacement[i].transform);
      itemList[i].transform.position = itemPlacement[i].transform.position;
      itemList[i].transform.rotation = itemPlacement[i].transform.rotation;
    }
  }
  public virtual GameObject DropItem() {
    rb.isKinematic = false;
    return this.gameObject;
  }
  // public void Accept(IInteractor interactor) { interactor.Interact(this); }
}
}

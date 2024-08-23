using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerAction;
using System;
using UnityEngine.Assertions;

namespace Interactable {
public delegate void FoodTrayHandler(FoodTrayState state);
public enum FoodTrayState { EMPTY, FILLED, FULL }
public class FoodTray : MonoBehaviour, IInteractable {
  public event FoodTrayHandler foodTrayHandler;

  private FoodTrayState state;

  [Header("Config")]
  [SerializeField]
  private int foodMaxStackHeight = 1;

  [Header("Dependencies")]
  [SerializeField]
  private List<GameObject> foodHolders;
  private List<GameObject> trayItems;

  [Header("Aesthetic")]

  private GameObject currentStateObject = null;

  public int GetNumItems() { return trayItems.Count; }
  public bool IsFull() { return trayItems.Count == foodHolders.Count; }
  public bool IsEmpty() { return trayItems.Count == 0; }
  public void AddItem(GameObject obj) {
    trayItems.Add(obj);
    UpdateItem();
    foodTrayHandler?.Invoke(state);
  }
  public GameObject TakeItem() {
    if (trayItems.Count == 0)
      return null;
    int index = trayItems.Count - 1;
    GameObject ret = trayItems[index];
    trayItems.RemoveAt(index);
    UpdateItem();
    foodTrayHandler?.Invoke(state);
    return ret;
  }

  private void Awake() { trayItems = new List<GameObject>(); }
  private void Start() { UpdateItem(); }

  private void UpdateItem() {
    currentStateObject?.SetActive(false);
    int numItems = trayItems.Count;
    if (numItems == 0) {
      state = FoodTrayState.EMPTY;
    } else if (numItems == foodHolders.Count) {
      state = FoodTrayState.FULL;
    } else {
      state = FoodTrayState.FILLED;
    }
    // currentStateObject.SetActive(true);
    // TODO: Update the look of the tray as we add item
    for (int i = 0; i < trayItems.Count; i++) {
      trayItems[i].transform.SetParent(foodHolders[i].transform);
      trayItems[i].transform.position = foodHolders[i].transform.position;
      trayItems[i].transform.rotation = foodHolders[i].transform.rotation;
    }
  }
  public void Accept(IInteractor interactor) { interactor.Interact(this); }
}
}

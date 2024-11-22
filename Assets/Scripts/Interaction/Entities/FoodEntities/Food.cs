using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactable;
using Entity;
using System;

namespace Interactable {
[Serializable]
public class Food : MonoBehaviour {
  [SerializeField]
  private FoodInfo foodInfo;
  public string Name {
    get { return foodInfo.foodName; }
  }
  public List<FoodCategory> FoodCategories {
    get { return foodInfo.foodCategories; }
  }
  public FoodID ID { get => foodInfo.foodID; }

  public bool ContainsCategory(List<FoodCategory> categories) {
    // Quick and dirty comparison O(N^2)
    foreach (FoodCategory category in categories) {
      foreach (FoodCategory myFoodCategory in foodInfo.foodCategories) {
        if (category == myFoodCategory)
          return true;
      }
    }
    return false;
  }
  public bool Is(FoodID id) { return id == foodInfo.foodID; }
}
}

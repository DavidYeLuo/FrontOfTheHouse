using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactable {
public enum FoodCategory { Muffin }

[CreateAssetMenu(fileName = "FoodInfo", menuName = "Food/FoodInfo")]
public class FoodInfo : ScriptableObject {
  public string foodName;
  public List<FoodCategory> foodCategories;
}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactable {
public enum FoodTrayID { BlueberryMuffinTray }
public enum FoodTrayCategory { MuffinTray }

[CreateAssetMenu(fileName = "FoodTrayInfo",
                 menuName = "ItemHolder/FoodTrayInfo")]
public class FoodTrayInfo : ScriptableObject {
  public string foodTrayName;
  public FoodTrayID foodTrayId;
  public List<FoodTrayCategory> foodTrayCategories;
}
}
